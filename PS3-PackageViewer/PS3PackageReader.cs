namespace PS3_PackageViewer
{
   using System;
   using System.Collections.Generic;
   using System.Diagnostics;
   using System.IO;
   using System.Linq;
   using System.Security.Cryptography;
   using System.Text;

   public class PS3PackageReader
   {
      private static readonly byte[] MAGIC_PKG = new byte[] { 0x7F, 0x50, 0x4B, 0x47 };
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
         public byte[] magic;
         public byte[] pkg_revision;
         public byte[] pkg_type;
         public byte[] pkg_meta_data_offset;
         public byte[] pkg_meta_data_count;
         public byte[] pkg_meta_data_size;
         public byte[] item_count;
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
               return this.magic.SequenceEqual(MAGIC_PKG);
            }
         }
      }

      public struct File
      {
         public string Name;
         public string FolderName;
         public uint Offset;
         public uint Size;
         public byte Type;
         public byte Unknown;
      }

      public class Package
      {
         public string ContentId
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

      public Package Read(string filePath)
      {
         if (string.IsNullOrEmpty(filePath)) {
            throw new ArgumentNullException(nameof(filePath));
         }

         this._filePath = filePath;
         if (Path.GetExtension(filePath) != ".pkg") {
            throw new InvalidOperationException("File error");
         }

         FileStream cipherStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
         BinaryReader cipherReader = new BinaryReader(cipherStream, Encoding.ASCII, true);

         this._header = this.ReaderHeader(cipherReader);
         if (!this._header.IsValid || this._header.pkg_type[0] != 0x01) {
            throw new InvalidDataException("This package(PKG) is not supported.");
         }

         return this.ReaderContent(cipherStream, cipherReader);
      }

      private Package ReaderContent(FileStream cipherStream, BinaryReader cipherReader)
      {
         Package package = new Package(this._header.contentid);
         MemoryStream decipherStream = this.ReaderContentData(cipherStream);
         BinaryReader decipherReader = new BinaryReader(decipherStream);

         uint index = 0;

         byte[] fileTable = new byte[320000];
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
               Type = fileTable[index + 27]
            };

            string contentId = package.ContentId;

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
               contentId = contentId + "\\" + filename.Remove(removeIndex);
            }

            file.FolderName = contentId;
            file.Name = filename;
            package.Files.Add(file);

            // Increment
            index += 32;
         }

         return package;
      }

      private MemoryStream ReaderContentData(FileStream fileStream)
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

         bufferStream.Flush();
         return bufferStream;
      }

      private Header ReaderHeader(BinaryReader reader)
      {
         Header header = new Header();

         // 読み取り開始位置を最初にする
         reader.BaseStream.Seek(0, SeekOrigin.Begin);

         // magic
         // Offset: 0x00, Size: 0x04
         header.magic = reader.ReadBytes(0x04);

         // pkg_revision
         // Offset: 0x04, Size: 0x02
         header.pkg_revision = reader.ReadBytes(0x02);
         Array.Reverse(header.pkg_revision);

         // pkg_type
         // Offset: 0x06, Size: 0x02
         header.pkg_type = reader.ReadBytes(0x02);
         Array.Reverse(header.pkg_type);

         // pkg_meta_data_offset
         // Offset: 0x08, Size: 0x04
         header.pkg_meta_data_offset = reader.ReadBytes(0x04);
         Array.Reverse(header.pkg_meta_data_offset);

         // pkg_meta_data_count
         // Offset: 0x0C, Size: 0x04
         header.pkg_meta_data_count = reader.ReadBytes(0x04);
         Array.Reverse(header.pkg_meta_data_count);

         // pkg_meta_data_size
         // Offset: 0x10, Size: 0x04
         header.pkg_meta_data_size = reader.ReadBytes(0x04);
         Array.Reverse(header.pkg_meta_data_count);

         // item_count
         // Offset: 0x14, Size: 0x04
         header.item_count = reader.ReadBytes(0x04);
         Array.Reverse(header.item_count);

         // total_size
         // Offset: 0x18, Size: 0x08
         byte[] total_size = reader.ReadBytes(0x08);
         Array.Reverse(total_size);
         header.total_size = BitConverter.ToUInt64(total_size, 0);

         // data_offset
         // Offset: 0x18, Size: 0x08
         byte[] data_offset = reader.ReadBytes(0x08);
         Array.Reverse(data_offset);
         header.data_offset = BitConverter.ToUInt64(data_offset, 0);

         // data_size
         // Offset: 0x18, Size: 0x08
         byte[] data_size = reader.ReadBytes(0x08);
         Array.Reverse(data_size);
         header.data_size = BitConverter.ToUInt64(data_size, 0);

         // contentid
         // Offset: 0x30, Size: 0x24
         header.contentid = Encoding.UTF8.GetString(reader.ReadBytes(0x24));

         // padding
         // Offset: 0x54, Size: 0x0C
         header.padding = reader.ReadBytes(0x0C);
         Array.Reverse(header.padding);

         // digest
         // Offset: 0x60, Size: 0x10
         header.digest = reader.ReadBytes(0x10);
         Array.Reverse(header.digest);

         // pkg_data_riv
         // Offset: 0x70, Size: 0x10
         header.pkg_data_riv = reader.ReadBytes(0x10);

         // pkg_header_digest
         // Offset: 0x80, Size: 0x40
         header.pkg_header_digest = reader.ReadBytes(0x40);

         return header;
      }

      private byte[] ToByteArray(string hexString)
      {
         int pos = 0;
         byte[] bytes = new byte[hexString.Length >> 1];
         for (int i = 0; i < hexString.Length; i = i + 2) {
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
         if (size % 16 > 0)
            size = ((size / 16) + 1) * 16;

         byte[] key = new byte[size];
         byte[] xor = new byte[size];
         byte[] data = new byte[size];
         byte[] dataRiv = new byte[this._header.pkg_data_riv.Length];

         stream.Seek(relativeOffset + startOffset, SeekOrigin.Begin);
         data = reader.ReadBytes(size);

         Array.Copy(this._header.pkg_data_riv, dataRiv, this._header.pkg_data_riv.Length);
         for (int pos = 0; pos < relativeOffset; pos += 16) {
            IncrementArray(ref dataRiv, this._header.pkg_data_riv.Length - 1);
         }

         for (int pos = 0; pos < size; pos += 16) {
            Array.Copy(dataRiv, 0, key, pos, this._header.pkg_data_riv.Length);
            IncrementArray(ref dataRiv, this._header.pkg_data_riv.Length - 1);
         }

         xor = RijndaelEncrypt(key, CipherMode.ECB, PaddingMode.None);
         return XOR(data, 0, xor.Length, xor);
      }
   }
}
