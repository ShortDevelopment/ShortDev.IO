using System;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace ShortDev.IO.Input;

public ref struct EndianReader(Endianness endianness, ReadOnlyEndianBuffer buffer)
{
    static readonly Encoding DefaultEncoding = Encoding.UTF8;

    public readonly bool UseLittleEndian = endianness == Endianness.LittleEndian;
    public ReadOnlyEndianBuffer Buffer = buffer;

    public ReadOnlySpan<byte> ReadToEnd()
        => Buffer.ReadToEnd();

    public ReadOnlySpan<byte> ReadBytes(int length)
        => Buffer.ReadBytes(length);

    public void ReadBytes(scoped Span<byte> buffer)
        => Buffer.ReadBytes(buffer);

    public byte ReadByte()
        => Buffer.ReadByte();

    public short ReadInt16()
    {
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadInt16LittleEndian(buffer);
        else
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
    }

    public ushort ReadUInt16()
    {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
        else
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
    }

    public int ReadInt32()
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadInt32LittleEndian(buffer);
        else
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
    }

    public uint ReadUInt32()
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
        else
            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
    }

    public long ReadInt64()
    {
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadInt64LittleEndian(buffer);
        else
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
    }

    public ulong ReadUInt64()
    {
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadUInt64LittleEndian(buffer);
        else
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
    }

    public float ReadSingle()
    {
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadSingleLittleEndian(buffer);
        else
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
    }

    public double ReadDouble()
    {
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadDoubleLittleEndian(buffer);
        else
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
    }

    public Guid ReadGuid()
    {
        Span<byte> buffer = stackalloc byte[16];
        ReadBytes(buffer);
        return new(buffer);
    }

    public string ReadStringWithLength()
        => ReadStringWithLength(DefaultEncoding);

    public string ReadStringWithLength(Encoding encoding)
    {
        var result = encoding.GetString(ReadBytesWithLength());
        ReadByte(); // Zero byte
        return result;
    }

    public ReadOnlySpan<byte> ReadBytesWithLength()
    {
        var length = ReadUInt16();
        return ReadBytes(length);
    }

    public static EndianReader Create(Endianness endianness, Stream stream)
        => new(endianness, new(stream));

    public static EndianReader Create(Endianness endianness, ReadOnlySpan<byte> buffer)
        => new(endianness, new(buffer));
}
