namespace PS3_PackageViewer
{
   using System;
   using System.Collections.Generic;
   using System.Diagnostics;
   using System.IO;
   using System.Linq;
   using System.Security.Cryptography;
   using System.Text;
   using System.Threading.Tasks;

   public class PS3PackageReader
   {
      private const int MULTIPLIER_SIZE = 65536;

      /// <summary>
      /// NPDRM PKG AES Keys
      /// <see href="https://www.psdevwiki.com/ps3/PKG_files#Header"/>
      /// </summary>
      public const string NPDRM_PKG_PS3_AES_KEY = "2E7B71D7C9C9A14EA3221F188828B8F8";

      /// <summary>
      /// Package Header
      /// <see href="https://www.psdevwiki.com/ps3/PKG_files#Header"/>
      /// </summary>
      public class Header
      {
         public string magic;
         public ushort pkg_revision;
         public ushort pkg_type;
         public uint pkg_meta_data_offset;
         public uint pkg_meta_data_count;
         public uint pkg_meta_data_size;
         public uint item_count;
         public ulong total_size;
         public ulong data_offset;
         public ulong data_size;
         public string contentid;
         public byte[] padding;
         public byte[] digest;
         public byte[] pkg_data_riv;
         public byte[] pkg_header_digest;

         public bool IsValid
         {
            get {
               Trace.WriteLine(this.magic.Remove(0, 1));
               return this.magic.Remove(0, 1).SequenceEqual("PKG");
            }
         }
      }

      public enum FileType
      {
         Folder,
         File,
         Image,
         Unknown,
      }

      public struct File
      {
         public string Name;
         public string FolderName;
         public uint Offset;
         public uint Size;
         public FileType Type;
         public byte Unknown;
      }

      public class Package
      {
         public string TitleId
         {
            get {
               if (this.FileName == null) {
                  return "Unknown_ContentId";
               }

               string[] split = this.FileName.Split('-');
               return split[1].Replace("_00", "").Trim();
            }
         }

         public string FileName { get; }

         public List<File> Files { get; private set; } = new List<File>();

         public Package(string contentId)
         {
            this.FileName = contentId;
         }
      }

      private readonly byte[] npdrm_pkg_ps3_aes_key;
      private string _filePath;
      private Header _header;

      public PS3PackageReader()
      {
         this.npdrm_pkg_ps3_aes_key = this.ToByteArray(NPDRM_PKG_PS3_AES_KEY);
      }

      public async Task<Package> ReadAsync(string filePath)
      {
         if (string.IsNullOrEmpty(filePath)) {
            throw new ArgumentNullException(nameof(filePath));
         }

         this._filePath = filePath;
         if (Path.GetExtension(filePath) != ".pkg") {
            throw new InvalidOperationException("Invalid File.");
         }

         FileStream cipherStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
         BinaryReader cipherReader = new BinaryReader(cipherStream, Encoding.ASCII, true);
         HexReader hexReader = new HexReader(cipherStream, HexReader.Endianness.Big);
         this._header = this.ReaderHeader(hexReader);
         if (!this._header.IsValid || this._header.pkg_type != 1) {
            throw new InvalidDataException("This package(PKG) is not supported.");
         }

         return await this.ReaderContent(cipherStream, cipherReader);
      }

      private async Task<Package> ReaderContent(FileStream cipherStream, BinaryReader cipherReader)
      {
         Package package = new Package(this._header.contentid);
         MemoryStream decipherStream = await this.ReaderContentData(cipherStream);
         BinaryReader decipherReader = new BinaryReader(decipherStream);

         uint index = 0;

         byte[] fileTable = new byte[370000];
         byte[] size = new byte[4];
         byte[] offset = new byte[4];
         byte[] nameSize = new byte[4];
         byte[] nameOffset = new byte[4];
         byte[] firstFileOffset = new byte[4];
         byte[] firstNameOffset = new byte[4];

         decipherStream.Seek(0, SeekOrigin.Begin);
         fileTable = decipherReader.ReadBytes(fileTable.Length);

         Array.Copy(fileTable, 0, firstNameOffset, 0, firstNameOffset.Length);
         Array.Reverse(firstNameOffset);
         uint firstNameSize = BitConverter.ToUInt32(firstNameOffset, 0) / 32;

         Array.Copy(fileTable, 12, firstFileOffset, 0, firstFileOffset.Length);
         Array.Reverse(firstFileOffset);
         decipherStream.Seek(0, SeekOrigin.Begin);
         fileTable = decipherReader.ReadBytes((int)BitConverter.ToUInt32(firstFileOffset, 0));

         for (int i = 0; i < (int)firstNameSize; i++) {
            File file = new File
            {
               Unknown = fileTable[index + 24],
               Type = this.GetFileType(fileTable[index + 27])
            };

            string titleId = package.TitleId;

            Array.Copy(fileTable, index + 12, offset, 0, offset.Length);
            Array.Reverse(offset);
            file.Offset = BitConverter.ToUInt32(offset, 0);

            Array.Copy(fileTable, index + 20, size, 0, size.Length);
            Array.Reverse(size);
            file.Size = BitConverter.ToUInt32(size, 0);

            Array.Copy(fileTable, index, nameOffset, 0, nameOffset.Length);
            Array.Reverse(nameOffset);
            uint extraNameOffset = BitConverter.ToUInt32(nameOffset, 0);

            Array.Copy(fileTable, index + 4, nameSize, 0, nameSize.Length);
            Array.Reverse(nameSize);
            uint extraNameSize = BitConverter.ToUInt32(nameSize, 0);

            byte[] name = new byte[extraNameSize];
            Array.Copy(fileTable, (int)extraNameOffset, name, 0, extraNameSize);

            byte[] data = Decryption((int)extraNameSize, (long)this._header.data_offset, (long)extraNameOffset, cipherStream, cipherReader);
            Array.Copy(data, 0, name, 0, extraNameSize);
            string filename = Encoding.ASCII.GetString(name);

            filename = filename.Replace('/', '\\');
            if (filename.Contains("\\")) {
               int removeIndex = filename.IndexOf('\\');
               titleId = titleId + "\\" + filename.Remove(removeIndex);
            }

            file.FolderName = titleId;
            file.Name = filename;
            package.Files.Add(file);

            // Increment
            index += 32;
         }

         return package;
      }

      private FileType GetFileType(byte fileType)
      {
         switch (fileType) {
            case 0x04:
               return FileType.Folder;
         }

         return FileType.Unknown;
      }

      private async Task<MemoryStream> ReaderContentData(FileStream fileStream)
      {
         if (this._header == null) {
            throw new NullReferenceException("パッケージヘッダーが初期化されていません");
         }

         MemoryStream bufferStream = new MemoryStream();
         ulong precision = (ulong)Math.Floor(this._header.data_size / (double)(this.npdrm_pkg_ps3_aes_key.Length * MULTIPLIER_SIZE));
         ulong dataSize = this._header.data_size % (ulong)(this.npdrm_pkg_ps3_aes_key.Length * MULTIPLIER_SIZE);

         if (dataSize > 0) {
            precision += 1;
         }

         byte[] riv = new byte[16];
         Array.Copy(this._header.pkg_data_riv, riv, this._header.pkg_data_riv.Length);

         int encryptSize = this.npdrm_pkg_ps3_aes_key.Length * MULTIPLIER_SIZE;
         fileStream.Seek((uint)this._header.data_offset, SeekOrigin.Begin);
         BinaryReader reader = new BinaryReader(fileStream);

         for (ulong i = 0; i < precision; i++) {
            byte[] encryptData = reader.ReadBytes(encryptSize);
            byte[] key = new byte[encryptData.Length];

            for (int pos = 0; pos < encryptData.Length; pos += this.npdrm_pkg_ps3_aes_key.Length) {
               Array.Copy(riv, 0, key, pos, this._header.pkg_data_riv.Length);
               IncrementArray(ref riv, this._header.pkg_data_riv.Length - 1);
            }

            byte[] encrypted = this.RijndaelEncrypt(key);
            byte[] decryptData = XOR(encryptData, 0, encrypted.Length, encrypted);

            bufferStream.Write(decryptData, 0, decryptData.Length);
         }

         await bufferStream.FlushAsync();
         return bufferStream;
      }

      private Header ReaderHeader(HexReader reader)
      {
         return new Header
         {
            // magic
            // Offset: 0x00, Size: 0x04
            magic = reader.ReadString(4),

            // pkg_revision
            // Offset: 0x04, Size: 0x02
            pkg_revision = reader.ReadUShort(),

            // pkg_type
            // Offset: 0x06, Size: 0x02
            pkg_type = reader.ReadUShort(),

            // pkg_meta_data_offset
            // Offset: 0x08, Size: 0x04
            pkg_meta_data_offset = reader.ReadUInt32(),

            // pkg_meta_data_count
            // Offset: 0x0C, Size: 0x04
            pkg_meta_data_count = reader.ReadUInt32(),

            // pkg_meta_data_size
            // Offset: 0x10, Size: 0x04
            pkg_meta_data_size = reader.ReadUInt32(),

            // item_count
            // Offset: 0x14, Size: 0x04
            item_count = reader.ReadUInt32(),

            // total_size
            // Offset: 0x18, Size: 0x08
            total_size = reader.ReadUInt64(),

            // data_offset
            // Offset: 0x20, Size: 0x08
            data_offset = reader.ReadUInt64(),

            // data_size
            // Offset: 0x28, Size: 0x08
            data_size = reader.ReadUInt64(),

            // contentid
            // Offset: 0x30, Size: 0x24
            contentid = Encoding.UTF8.GetString(reader.ReadBytes(0x24)),

            // padding
            // Offset: 0x54, Size: 0x0C
            padding = reader.ReadBytes(0x0C, 0x54),

            // digest
            // Offset: 0x60, Size: 0x10
            digest = reader.ReadBytes(0x10, HexReader.Endianness.Little),

            // pkg_data_riv
            // Offset: 0x70, Size: 0x10
            pkg_data_riv = reader.ReadBytes(0x10 , HexReader.Endianness.Little),

            // pkg_header_digest
            // Offset: 0x80, Size: 0x40
            pkg_header_digest = reader.ReadBytes(0x40, HexReader.Endianness.Little),
         };
      }

      private byte[] ToByteArray(string hexString)
      {
         int pos = 0;
         byte[] bytes = new byte[hexString.Length >> 1];
         for (int i = 0; i < hexString.Length; i += 2) {
            bytes[pos] = Convert.ToByte(hexString.Substring(i, 2), 16);
            pos++;
         }

         return bytes;
      }

      private byte[] RijndaelEncrypt(byte[] inputArray, CipherMode cipherMode = CipherMode.ECB, PaddingMode paddingMode = PaddingMode.None)
      {
         using (RijndaelManaged rijndael = new RijndaelManaged
         {
            Mode = cipherMode,
            Padding = paddingMode,
         }) {
            using (ICryptoTransform cryptoTransform = rijndael.CreateEncryptor(this.npdrm_pkg_ps3_aes_key, this.npdrm_pkg_ps3_aes_key))
            using (MemoryStream memoryEncrypt = new MemoryStream())
            using (CryptoStream cryptoEncrypt = new CryptoStream(memoryEncrypt, cryptoTransform, CryptoStreamMode.Write)) {
               cryptoEncrypt.Write(inputArray, 0, inputArray.Length);
               cryptoEncrypt.FlushFinalBlock();

               return memoryEncrypt.ToArray();
            }
         }
      }

      private bool IncrementArray(ref byte[] sourceArray, int position)
      {
         if (sourceArray[position] != 0xFF) {
            sourceArray[position] += 0x01;
            return true;
         }

         if (position != 0 && IncrementArray(ref sourceArray, position - 1)) {
            sourceArray[position] = 0x00;
            return true;
         }

         return false;
      }

      private byte[] XOR(byte[] sourceArray, int position, int length, byte[] key)
      {
         if (sourceArray.Length < (position + length) || (length % key.Length) != 0) {
            return null;
         }

         int precision = length / key.Length;
         byte[] result = new byte[length];
         for (int i = 0; i < precision; i++) {
            for (int pos = 0; pos < key.Length; pos++) {
               result[(i * key.Length) + pos] += (byte)(sourceArray[position + (i * key.Length) + pos] ^ key[pos]);
            }
         }

         return result;
      }

      private byte[] Decryption(int dataSize, long startOffset, long relativeOffset, Stream stream, BinaryReader reader)
      {
         int size = dataSize;
         if (size % 16 > 0) {
            size = ((size / 16) + 1) * 16;
         }

         byte[] key = new byte[size];
         byte[] riv = new byte[this._header.pkg_data_riv.Length];

         stream.Seek(relativeOffset + startOffset, SeekOrigin.Begin);
         byte[] data = reader.ReadBytes(size);

         Array.Copy(this._header.pkg_data_riv, riv, this._header.pkg_data_riv.Length);
         for (int pos = 0; pos < relativeOffset; pos += 16) {
            IncrementArray(ref riv, this._header.pkg_data_riv.Length - 1);
         }

         for (int pos = 0; pos < size; pos += 16) {
            Array.Copy(riv, 0, key, pos, this._header.pkg_data_riv.Length);
            IncrementArray(ref riv, this._header.pkg_data_riv.Length - 1);
         }

         byte[] xor = RijndaelEncrypt(key, CipherMode.ECB, PaddingMode.None);
         return XOR(data, 0, xor.Length, xor);
      }
   }
}
