using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace ShortDev.IO.Output;

internal ref struct CalcSizeWriter() : IEndianWriter
{
    public Encoding DefaultEncoding { get; init; } = Encoding.UTF8;
    public bool UseLittleEndian { get; } = true;


    ulong _size;
    public readonly ulong WrittenBinarySize => _size;


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(in T value) where T : IBinaryWritable, allows ref struct
        => value.Write(ref this);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(scoped ReadOnlySpan<byte> buffer)
        => _size += (ulong)buffer.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(sbyte value)
        => _size += 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(byte value)
        => _size += 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(short value)
        => _size += sizeof(short);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ushort value)
        => _size += sizeof(ushort);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(int value)
        => _size += sizeof(int);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(uint value)
        => _size += sizeof(uint);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(long value)
        => _size += sizeof(long);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(ulong value)
        => _size += sizeof(ulong);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Write(Half value)
        => _size += (ulong)sizeof(Half);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(float value)
        => _size += sizeof(float);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write(double value)
        => _size += sizeof(double);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Write(Guid value)
        => _size += (ulong)sizeof(Guid);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteWithLength(string value, Encoding? encoding = null)
    {
        encoding ??= DefaultEncoding;

        _size += sizeof(ushort); // for length
        var length = encoding.GetByteCount(value);
        _size += (ulong)length;

        _size += 1; // for null terminator
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteWithLength(scoped ReadOnlySpan<byte> value)
    {
        _size += sizeof(ushort); // for length
        _size += (ulong)value.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly void Dispose() { }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Advance(int count)
        => _size += (ulong)count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Memory<byte> GetMemory(int sizeHint = 0)
        => throw new NotImplementedException("SizeWriter does not support GetMemory.");

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly Span<byte> GetSpan(int sizeHint = 0)
        => throw new NotImplementedException("SizeWriter does not support GetSpan.");
}
