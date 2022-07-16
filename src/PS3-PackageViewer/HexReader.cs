
namespace PS3_PackageViewer
{
   using System;
   using System.IO;
   using System.Text;

   public class HexReader
   {
      public enum Endianness
      {
         Big,
         Little
      }

      public long Position
      {
         get {
            return this.BaseStream.Position;
         }
         set {
            this.BaseStream.Position = value;
         }
      }

      private long _beforePosition;
      public long BeforePosition
      {
         get {
            return this._beforePosition;
         }
      }

      public Endianness Endian { get; set; } = Endianness.Little;

      public Stream BaseStream { get; }

      /// <summary>
      /// Endianness = Little
      /// </summary>
      /// <param name="stream">Stream</param>
      public HexReader(Stream stream) : this(stream, Endianness.Little)
      { }

      /// <summary>
      /// Endianness = Little
      /// </summary>
      /// <param name="stream">Stream</param>
      public HexReader(FileStream fileStream) : this(fileStream, Endianness.Little)
      { }

      public HexReader(FileStream fileStream, Endianness Endian)
      {
         fileStream.Seek(0, SeekOrigin.Begin);
         this.BaseStream = fileStream;
         this.Endian = Endian;
         this._beforePosition = fileStream.Position;
      }

      public HexReader(Stream stream, Endianness Endian)
      {
         stream.Seek(0, SeekOrigin.Begin);
         this.BaseStream = stream;
         this.Endian = Endian;
         this._beforePosition = stream.Position;
      }

      public sbyte ReadSByte()
      {
         return this.ReadSByte(this.Position);
      }

      public sbyte ReadSByte(long offset)
      {
         return (sbyte)this.ReadCore(1, offset)[0];
      }

      public byte ReadByte()
      {
         return this.ReadCore(1)[0];
      }

      public byte ReadByte(long offset)
      {
         return this.ReadCore(1, offset)[0];
      }

      public byte[] ReadBytes(int length)
      {
         return this.ReadCore(length);
      }

      public byte[] ReadBytes(int length, Endianness endian)
      {
         return ReadCore(length, endian: endian, priorityArgument: true);
      }

      public byte[] ReadBytes(int length, long offset, Endianness endian = Endianness.Little)
      {
         return ReadCore(length, offset, endian, true);
      }

      public short ReadShort()
      {
         return BitConverter.ToInt16(this.ReadCore(2), 0);
      }

      public short ReadShort(long offset)
      {
         return BitConverter.ToInt16(this.ReadCore(2, offset), 0);
      }

      public ushort ReadUShort()
      {
         return BitConverter.ToUInt16(this.ReadCore(2), 0);
      }

      public ushort ReadUShort(Endianness endian)
      {
         return BitConverter.ToUInt16(this.ReadCore(2, endian: endian), 0);
      }

      public ushort ReadUShort(long offset)
      {
         return BitConverter.ToUInt16(this.ReadBytes(2, offset), 0);
      }

      public int ReadInt32()
      {
         return BitConverter.ToInt32(this.ReadCore(4), 0);
      }

      public int ReadInt32(long offset)
      {
         return BitConverter.ToInt32(this.ReadCore(4, offset), 0);
      }

      public uint ReadUInt32()
      {
         return BitConverter.ToUInt32(this.ReadCore(4), 0);
      }

      public uint ReadUInt32(long offset)
      {
         return BitConverter.ToUInt32(this.ReadCore(4, offset), 0);
      }

      public long ReadInt64()
      {
         return BitConverter.ToInt64(this.ReadCore(8), 0);
      }

      public long ReadInt64(long offset)
      {
         return BitConverter.ToInt64(this.ReadCore(8, offset), 0);
      }

      public ulong ReadUInt64()
      {
         return BitConverter.ToUInt64(this.ReadCore(8), 0);
      }

      public ulong ReadUInt64(long offset)
      {
         return BitConverter.ToUInt64(this.ReadCore(8, offset), 0);
      }

      public float ReadFloat(long offset)
      {
         return BitConverter.ToSingle(this.ReadCore(4, offset), 0);
      }

      public string ReadString(int length)
      {
         byte[] buffer = this.ReadCore(length, priorityArgument: true);
         return Encoding.ASCII.GetString(buffer);
      }

      public string ReadString(int length, long offset)
      {
         byte[] buffer = this.ReadCore(length, offset, priorityArgument: true);
         return Encoding.ASCII.GetString(buffer);
      }


      private byte[] ReadCore(int length, long offset = -1, Endianness endian = Endianness.Little, bool priorityArgument = false)
      {
         byte[] buffer = new byte[length];
         if (offset >= 0) {
            this.BaseStream.Position = offset;
         }
         this._beforePosition = this.BaseStream.Position;
         this.BaseStream.Read(buffer, 0, buffer.Length);
         if (this.Endian == Endianness.Big && !priorityArgument || endian == Endianness.Big) {
            Array.Reverse(buffer, 0, buffer.Length);
         }

         return buffer;
      }
   }
}
