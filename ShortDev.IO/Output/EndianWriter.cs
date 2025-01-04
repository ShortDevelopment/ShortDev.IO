using DotNext.Buffers;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Text;

namespace ShortDev.IO.Output;

public readonly ref struct EndianWriter(Endianness endianness, HeapOutputBuffer buffer) : IEndianWriter
{
    static readonly Encoding DefaultEncoding = Encoding.UTF8;

    public readonly bool UseLittleEndian { get; } = endianness == Endianness.LittleEndian;
    public readonly HeapOutputBuffer Buffer { get; } = buffer;

    public void Clear()
        => Buffer.Clear();

    public void Write(ReadOnlySpan<byte> buffer)
        => Buffer.Write(buffer);

    public void Write(sbyte value)
        => Buffer.Write(unchecked((byte)value));

    public void Write(byte value)
        => Buffer.Write(value);

    public void Write(short value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(short)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteInt16BigEndian(buffer, value);

        Write(buffer);
    }

    public void Write(ushort value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteUInt16BigEndian(buffer, value);

        Write(buffer);
    }

    public void Write(int value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteInt32BigEndian(buffer, value);

        Write(buffer);
    }

    public void Write(uint value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);

        Write(buffer);
    }

    public void Write(long value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(long)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteInt64BigEndian(buffer, value);

        Write(buffer);
    }

    public void Write(ulong value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteUInt64LittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteUInt64BigEndian(buffer, value);

        Write(buffer);
    }

    public void Write(float value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(float)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteSingleLittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteSingleBigEndian(buffer, value);

        Write(buffer);
    }

    public void Write(double value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(double)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteDoubleBigEndian(buffer, value);

        Write(buffer);
    }

    public void WriteWithLength(string value, Encoding? encoding = null)
    {
        encoding ??= DefaultEncoding;

        var length = encoding.GetByteCount(value);
        Write((ushort)length);

        var buffer = Buffer.GetSpan(length);
        encoding.GetBytes(value, buffer);
        Buffer.Advance(length);

        Write((byte)0);
    }

    public void WriteWithLength(ReadOnlySpan<byte> value)
    {
        Write((ushort)value.Length);
        Write(value);
    }

    public void CopyTo(Stream destination)
    {
        destination.Write(Buffer.WrittenSpan);
        destination.Flush();
    }

    public void Dispose()
        => Buffer.Dispose();

    public static EndianWriter Create(Endianness endianness, ArrayPool<byte> pool)
        => new(endianness, new HeapOutputBuffer(new(pool)));

    public static EndianWriter Create(Endianness endianness, ArrayPool<byte> pool, int initialCapacity)
    {
        PoolingArrayBufferWriter<byte> writer = new(pool)
        {
            Capacity = initialCapacity
        };
        return new(endianness, new HeapOutputBuffer(writer));
    }
}
