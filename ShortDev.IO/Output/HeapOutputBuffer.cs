using DotNext.Buffers;
using System;
using System.Buffers;

namespace ShortDev.IO.Output;

public readonly struct HeapOutputBuffer(PoolingArrayBufferWriter<byte> writer) : IEndianBuffer
{
    readonly PoolingArrayBufferWriter<byte> _writer = writer;

    public void Write(byte value)
        => _writer.Write([value]);

    public void Write(ReadOnlySpan<byte> buffer)
        => _writer.Write(buffer);

    public long Size
        => _writer.WrittenCount;

    public ReadOnlySpan<byte> WrittenSpan
        => _writer.WrittenMemory.Span;

    public ReadOnlyMemory<byte> WrittenMemory
        => _writer.WrittenMemory;

    public void Clear()
        => _writer.Clear();

    public void Advance(int count) => _writer.Advance(count);
    public Memory<byte> GetMemory(int sizeHint) => _writer.GetMemory(sizeHint);
    public Span<byte> GetSpan(int sizeHint) => _writer.GetSpan(sizeHint);

    public void Dispose()
        => _writer.Dispose();
}
