﻿
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

      private long _lastOffset;

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
         this._lastOffset = stream.Position;
      }

      public sbyte ReadSByte()
      {
         return this.ReadSByte(this.Position);
      }

      public sbyte ReadSByte(long offset)
      {
         return (sbyte)this.ReadBytes(1, offset)[0];
      }

      public byte ReadByte()
      {
         return this.ReadByte(this.Position);
      }

      public byte ReadByte(long offset)
      {
         return this.ReadBytes(1, offset)[0];
      }

      public byte[] ReadBytes(int length)
      {
         return ReadBytes(length, this._lastOffset);
      }

      public byte[] ReadBytes(int length, long position = -1)
      {
         if (position >= 0) {
            this.BaseStream.Position = position;
         }
         byte[] buffer = new byte[length];
         this.BaseStream.Read(buffer, 0, buffer.Length);
         this._lastOffset = this.BaseStream.Position;
         if (this.Endian == Endianness.Big) {
            Array.Reverse(buffer, 0, buffer.Length);
         }

         return buffer;
      }

      public short ReadShort()
      {
         return BitConverter.ToInt16(this.ReadBytes(2, this._lastOffset), 0);
      }

      public short ReadShort(long offset)
      {
         return BitConverter.ToInt16(this.ReadBytes(2, offset), 0);
      }

      public ushort ReadUShort()
      {
         return BitConverter.ToUInt16(this.ReadBytes(2, this._lastOffset), 0);
      }

      public ushort ReadUShort(long offset)
      {
         return BitConverter.ToUInt16(this.ReadBytes(2, offset), 0);
      }

      public int ReadInt32()
      {
         return BitConverter.ToInt32(this.ReadBytes(4, this._lastOffset), 0);
      }

      public int ReadInt32(long offset)
      {
         return BitConverter.ToInt32(this.ReadBytes(4, offset), 0);
      }

      public uint ReadUInt32()
      {
         return BitConverter.ToUInt32(this.ReadBytes(4, this._lastOffset), 0);
      }

      public uint ReadUInt32(long offset)
      {
         return BitConverter.ToUInt32(this.ReadBytes( 4, offset), 0);
      }

      public long ReadInt64()
      {
         return BitConverter.ToInt64(this.ReadBytes(8, this._lastOffset), 0);
      }

      public long ReadInt64(long offset)
      {
         return BitConverter.ToInt64(this.ReadBytes(8, offset), 0);
      }

      public ulong ReadUInt64()
      {
         return BitConverter.ToUInt64(this.ReadBytes(8, this._lastOffset), 0);
      }

      public ulong ReadUInt64(long offset)
      {
         return BitConverter.ToUInt64(this.ReadBytes(8, offset), 0);
      }

      public float ReadFloat(long offset)
      {
         return BitConverter.ToSingle(this.ReadBytes(4, offset), 0);
      }

      public string ReadString(int length)
      {
         byte[] buffer = this.ReadBytes(length);
         return Encoding.ASCII.GetString(buffer);
      }

      public string ReadString(int length, long offset)
      {
         byte[] buffer = this.ReadBytes(length, offset);
         return Encoding.ASCII.GetString(buffer);
      }
   }
}
