using System;
using System.Buffers;
using System.Diagnostics.CodeAnalysis;

namespace ShortDev.IO.Output;

public readonly struct EndianBuffer : IBufferWriter<byte>
{
    readonly ArrayBufferWriter<byte> _writer;

    public EndianBuffer()
        => _writer = new();

    public EndianBuffer(int initialCapacity)
        => _writer = new(initialCapacity);

    public EndianBuffer(ReadOnlySpan<byte> initialData)
    {
        _writer = new(initialData.Length);
        _writer.Write(initialData);
    }

    [SuppressMessage("Style", "IDE0302:Simplify collection initialization", Justification = "Seems like this allocates a new array instead of using stackalloc!")]
    public void Write(byte value)
    {
        Span<byte> buffer = stackalloc byte[1];
        buffer[0] = value;
        _writer.Write(buffer);
    }

    public void Write(ReadOnlySpan<byte> buffer)
        => _writer.Write(buffer);

    public int Size
        => _writer.WrittenCount;

    public ReadOnlySpan<byte> AsSpan()
        => _writer.WrittenSpan;

    public Span<byte> AsWriteableSpan()
        => _writer.WrittenSpan.AsSpanUnsafe();

    public ReadOnlyMemory<byte> AsMemory()
        => _writer.WrittenMemory;

    public byte[] ToArray()
        => _writer.WrittenMemory.ToArray();

    public void Clear()
        => _writer.Clear();

    public void Advance(int count) => _writer.Advance(count);
    public Memory<byte> GetMemory(int sizeHint) => _writer.GetMemory(sizeHint);
    public Span<byte> GetSpan(int sizeHint) => _writer.GetSpan(sizeHint);
}
