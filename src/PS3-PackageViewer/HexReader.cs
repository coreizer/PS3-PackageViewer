
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

      public HexReader(Stream stream, Endianness Endian)
      {
         stream.Seek(0, SeekOrigin.Begin);
         this.BaseStream = stream;
         this.Endian = Endian;
      }

      public sbyte ReadSByte()
      {
         return this.ReadSByte(this.Position);
      }

      public sbyte ReadSByte(long offset)
      {
         return (sbyte)this.ReadByte(offset);
      }

      public byte ReadByte()
      {
         return this.ReadByte(this.Position);
      }

      public byte ReadByte(long offset)
      {
         byte[] buffer = new byte[1];
         this.BaseStream.Position = offset;
         this.BaseStream.Read(buffer, 0, buffer.Length);
         return buffer[0];
      }

      public byte[] ReadBytes(int length)
      {
         return ReadBytes(this.BaseStream.Position, length);
      }

      public byte[] ReadBytes(long offset, int length)
      {
         byte[] buffer = new byte[length];
         this.BaseStream.Position = offset;
         this.BaseStream.Read(buffer, 0, buffer.Length);
         if (this.Endian == Endianness.Big) {
            Array.Reverse(buffer, 0, buffer.Length);
         }

         return buffer;
      }

      public short ReadShort()
      {
         return BitConverter.ToInt16(this.ReadBytes(this.BaseStream.Position, 2), 0);
      }

      public short ReadShort(long offset)
      {
         return BitConverter.ToInt16(this.ReadBytes(offset, 2), 0);
      }

      public ushort ReadUShort()
      {
         return BitConverter.ToUInt16(this.ReadBytes(this.BaseStream.Position, 2), 0);
      }

      public ushort ReadUShort(long offset)
      {
         return BitConverter.ToUInt16(this.ReadBytes(offset, 2), 0);
      }

      public int ReadInt32()
      {
         return BitConverter.ToInt32(this.ReadBytes(this.BaseStream.Position, 4), 0);
      }

      public int ReadInt32(long offset)
      {
         return BitConverter.ToInt32(this.ReadBytes(offset, 4), 0);
      }

      public uint ReadUInt32()
      {
         return BitConverter.ToUInt32(this.ReadBytes(this.BaseStream.Position, 4), 0);
      }

      public uint ReadUInt32(long offset)
      {
         return BitConverter.ToUInt32(this.ReadBytes(offset, 4), 0);
      }

      public long ReadInt64()
      {
         return BitConverter.ToInt64(this.ReadBytes(this.BaseStream.Position, 8), 0);
      }

      public long ReadInt64(long offset)
      {
         return BitConverter.ToInt64(this.ReadBytes(offset, 8), 0);
      }

      public ulong ReadUInt64()
      {
         return BitConverter.ToUInt64(this.ReadBytes(this.BaseStream.Position, 8), 0);
      }

      public ulong ReadUInt64(long offset)
      {
         return BitConverter.ToUInt64(this.ReadBytes(offset, 8), 0);
      }

      public float ReadFloat(long offset)
      {
         return BitConverter.ToSingle(this.ReadBytes(offset, 4), 0);
      }

      public string ReadString(int length)
      {
         byte[] buffer = new byte[length];
         this.BaseStream.Read(buffer, 0, buffer.Length);
         return Encoding.ASCII.GetString(buffer);
      }

      public string ReadString(long offset, int length)
      {
         byte[] buffer = new byte[length];
         this.BaseStream.Position = offset;
         this.BaseStream.Read(buffer, 0, buffer.Length);
         return Encoding.ASCII.GetString(buffer);
      }
   }
}
