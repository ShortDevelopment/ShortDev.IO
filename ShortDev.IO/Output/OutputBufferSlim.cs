using DotNext.Buffers;
using System;

namespace ShortDev.IO.Output;

public readonly ref struct OutputBufferSlim(BufferWriterSlim<byte> writer) : IEndianBuffer
{
    readonly BufferWriterSlim<byte> _writer = writer;

    public void Write(byte value)
        => _writer.Write([value]);

    public void Write(ReadOnlySpan<byte> buffer)
        => _writer.Write(buffer);

    public long Size
        => _writer.WrittenCount;

    public ReadOnlySpan<byte> WrittenSpan
        => _writer.WrittenSpan;

    public void Clear()
        => _writer.Clear();

    public void Advance(int count) => _writer.Advance(count);
    public Memory<byte> GetMemory(int sizeHint) => throw new NotImplementedException();
    public Span<byte> GetSpan(int sizeHint) => _writer.GetSpan(sizeHint);

    public void Dispose()
        => _writer.Dispose();
}
