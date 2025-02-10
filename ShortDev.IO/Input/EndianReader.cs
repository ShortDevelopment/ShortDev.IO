using ShortDev.IO.Buffers;
using ShortDev.IO.ValueStream;
using System;
using System.Buffers;
using System.Buffers.Binary;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace ShortDev.IO.Input;

public ref struct EndianReader<TStream>(Endianness endianness) : IEndianReader where TStream : struct, IValueInputStream, allows ref struct
{
    public Encoding DefaultEncoding { get; init; } = Encoding.UTF8;

    public readonly bool UseLittleEndian { get; } = endianness == Endianness.LittleEndian;

    public required TStream Stream;

    public T Read<T>() where T : IBinaryParsable<T>
        => T.Parse(ref this);

    public void ReadBytes(scoped Span<byte> buffer)
        => Stream.Read(buffer);

    public bool TryReadSlice(int length, out ReadOnlySpan<byte> slice)
        => Stream.TryReadSlice(length, out slice);

    public sbyte ReadInt8()
    {
        byte value = 0;
        ReadBytes(new(ref value));
        return unchecked((sbyte)value);
    }

    public byte ReadUInt8()
    {
        byte value = 0;
        ReadBytes(new(ref value));
        return value;
    }

    [SkipLocalsInit]
    public short ReadInt16()
    {
        Span<byte> buffer = stackalloc byte[sizeof(short)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadInt16LittleEndian(buffer);
        else
            return BinaryPrimitives.ReadInt16BigEndian(buffer);
    }

    [SkipLocalsInit]
    public ushort ReadUInt16()
    {
        Span<byte> buffer = stackalloc byte[sizeof(ushort)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadUInt16LittleEndian(buffer);
        else
            return BinaryPrimitives.ReadUInt16BigEndian(buffer);
    }

    [SkipLocalsInit]
    public int ReadInt32()
    {
        Span<byte> buffer = stackalloc byte[sizeof(int)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadInt32LittleEndian(buffer);
        else
            return BinaryPrimitives.ReadInt32BigEndian(buffer);
    }

    [SkipLocalsInit]
    public uint ReadUInt32()
    {
        Span<byte> buffer = stackalloc byte[sizeof(uint)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadUInt32LittleEndian(buffer);
        else
            return BinaryPrimitives.ReadUInt32BigEndian(buffer);
    }

    [SkipLocalsInit]
    public long ReadInt64()
    {
        Span<byte> buffer = stackalloc byte[sizeof(long)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadInt64LittleEndian(buffer);
        else
            return BinaryPrimitives.ReadInt64BigEndian(buffer);
    }

    [SkipLocalsInit]
    public ulong ReadUInt64()
    {
        Span<byte> buffer = stackalloc byte[sizeof(ulong)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadUInt64LittleEndian(buffer);
        else
            return BinaryPrimitives.ReadUInt64BigEndian(buffer);
    }

    [SkipLocalsInit]
    public unsafe Half ReadHalf()
    {
        Span<byte> buffer = stackalloc byte[sizeof(Half)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadHalfLittleEndian(buffer);
        else
            return BinaryPrimitives.ReadHalfBigEndian(buffer);
    }

    [SkipLocalsInit]
    public float ReadSingle()
    {
        Span<byte> buffer = stackalloc byte[sizeof(float)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadSingleLittleEndian(buffer);
        else
            return BinaryPrimitives.ReadSingleBigEndian(buffer);
    }

    [SkipLocalsInit]
    public double ReadDouble()
    {
        Span<byte> buffer = stackalloc byte[sizeof(double)];
        ReadBytes(buffer);

        if (UseLittleEndian)
            return BinaryPrimitives.ReadDoubleLittleEndian(buffer);
        else
            return BinaryPrimitives.ReadDoubleBigEndian(buffer);
    }

    [SkipLocalsInit]
    public Guid ReadGuid()
    {
        Span<byte> buffer = stackalloc byte[16];
        ReadBytes(buffer);
        return new(buffer);
    }

    public string ReadString(int byteSize, Encoding? encoding = null)
    {
        encoding ??= DefaultEncoding;

        var rentedBuffer = ArrayPool<byte>.Shared.Rent(byteSize);
        try
        {
            var buffer = rentedBuffer.AsSpan()[..byteSize];

            ReadBytes(buffer);
            ReadUInt8(); // Zero byte
            return encoding.GetString(buffer);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rentedBuffer);
        }
    }

    public string ReadStringWithLength(Encoding? encoding = null)
    {
        var byteSize = ReadUInt16();
        return ReadString(byteSize, encoding);
    }

    public void Dispose()
        => Stream.Dispose();
}

public static class EndianReader
{
    public static EndianReader<StreamWrapperStream> FromStream(Endianness endianness, Stream stream)
        => new(endianness) { Stream = new(stream) };

    public static EndianReader<FixedReadOnlyStackStream> FromSpan(Endianness endianness, ReadOnlySpan<byte> buffer)
        => new(endianness) { Stream = new(buffer) };

    public static EndianReader<FixedReadOnlyHeapStream> FromMemory(Endianness endianness, ReadOnlyMemory<byte> buffer)
        => new(endianness) { Stream = new(buffer) };

    public static EndianReader<FixedReadOnlyHeapStream> FromMemory(Endianness endianness, PooledMemory<byte> buffer)
        => new(endianness) { Stream = new(buffer.Memory, buffer.Dispose) };
}
