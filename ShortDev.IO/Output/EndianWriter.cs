using ShortDev.IO.ValueStream;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace ShortDev.IO.Output;

public ref struct EndianWriter<TStream>(Endianness endianness) : IEndianWriter where TStream : struct, IValueOutputStream, IBufferWriter<byte>, allows ref struct
{
    public Encoding DefaultEncoding { get; init; } = Encoding.UTF8;

    public readonly bool UseLittleEndian { get; } = endianness == Endianness.LittleEndian;
    public required TStream Stream;

    public void Write<T>(T value) where T : IBinaryWritable
        => value.Write(ref this);

    public void Write(scoped ReadOnlySpan<byte> buffer)
        => Stream.Write(buffer);

    public void Write(sbyte value)
        => Stream.Write([unchecked((byte)value)]);

    public void Write(byte value)
        => Stream.Write([value]);

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

    public unsafe void Write(Half value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(Half)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteHalfLittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteHalfBigEndian(buffer, value);

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

    public void Write(Guid value)
    {
        var buffer = Stream.GetSpan(16);
        value.TryWriteBytes(buffer);
        Stream.Advance(16);
    }

    public void WriteWithLength(string value, Encoding? encoding = null)
    {
        encoding ??= DefaultEncoding;

        var length = encoding.GetByteCount(value);
        Write((ushort)length);

        var buffer = Stream.GetSpan(length);
        encoding.GetBytes(value, buffer);
        Stream.Advance(length);

        Write((byte)0);
    }

    public void WriteWithLength(ReadOnlySpan<byte> value)
    {
        Write((ushort)value.Length);
        Write(value);
    }

    public void Dispose()
        => Stream.Dispose();

    public void Advance(int count)
        => Stream.Advance(count);

    public Memory<byte> GetMemory(int sizeHint = 0)
        => Stream.GetMemory(sizeHint);

    public Span<byte> GetSpan(int sizeHint = 0)
        => Stream.GetSpan(sizeHint);
}

public static class EndianWriter
{
    public static EndianWriter<HeapOutputStream> Create(Endianness endianness, ArrayPool<byte> pool)
    {
        return new(endianness)
        {
            Stream = new()
            {
                Writer = new(pool)
            }
        };
    }

    public static EndianWriter<HeapOutputStream> Create(Endianness endianness, ArrayPool<byte> pool, int initialCapacity)
    {
        return new(endianness)
        {
            Stream = new()
            {
                Writer = new(pool)
                {
                    Capacity = initialCapacity
                }
            }
        };
    }
}
