using ShortDev.IO.ValueStream;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Text;

namespace ShortDev.IO.Output;

public ref struct EndianWriter<TStream>(Endianness endianness) : IEndianWriter where TStream : struct, IValueOutputStream, IBufferWriter<byte>, allows ref struct
{
    public Encoding DefaultEncoding { get; init; } = Encoding.UTF8;

    public readonly bool UseLittleEndian { get; } = endianness == Endianness.LittleEndian;
    public required TStream Stream;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(in T value) where T : IBinaryWritable, allows ref struct
        => value.Write(ref this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped ReadOnlySpan<byte> buffer)
        => Stream.Write(buffer);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(sbyte value)
        => Stream.Write([unchecked((byte)value)]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte value)
        => Stream.Write([value]);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(short value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(short)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteInt16LittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteInt16BigEndian(buffer, value);

        Write(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ushort value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteUInt16LittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteUInt16BigEndian(buffer, value);

        Write(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(int value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteInt32LittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteInt32BigEndian(buffer, value);

        Write(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(uint value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteUInt32LittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteUInt32BigEndian(buffer, value);

        Write(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(long value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(long)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteInt64LittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteInt64BigEndian(buffer, value);

        Write(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ulong value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteUInt64LittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteUInt64BigEndian(buffer, value);

        Write(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Write(Half value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(Half)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteHalfLittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteHalfBigEndian(buffer, value);

        Write(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(float value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(float)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteSingleLittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteSingleBigEndian(buffer, value);

        Write(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(double value)
    {
        Span<byte> buffer = stackalloc byte[sizeof(double)];

        if (UseLittleEndian)
            BinaryPrimitives.WriteDoubleLittleEndian(buffer, value);
        else
            BinaryPrimitives.WriteDoubleBigEndian(buffer, value);

        Write(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(Guid value)
    {
        var buffer = Stream.GetSpan(16);
        value.TryWriteBytes(buffer);
        Stream.Advance(16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteWithLength(scoped ReadOnlySpan<byte> value)
    {
        Write((ushort)value.Length);
        Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
        => Stream.Dispose();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
        => Stream.Advance(count);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<byte> GetMemory(int sizeHint = 0)
        => Stream.GetMemory(sizeHint);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

    public static EndianWriter<SpanStream> Create(Endianness endianness, Span<byte> buffer)
    {
        return new(endianness)
        {
            Stream = new(buffer)
        };
    }

    public static ulong CalcBinarySize<T>(in T value) where T : IBinaryWritable
    {
        CalcSizeWriter writer = new();
        value.Write(ref writer);
        return writer.WrittenBinarySize;
    }
}
