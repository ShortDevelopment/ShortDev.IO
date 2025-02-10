using DotNext.Buffers;
using ShortDev.IO;
using System;
using System.Buffers;
using System.Threading.Tasks;

namespace ShortDev.IO.ValueStream;

public readonly struct HeapOutputStream : IValueOutputStream, IBufferWriter<byte>, IValueStreamPosition
{
    public required PoolingArrayBufferWriter<byte> Writer { get; init; }

    public long Length => Writer.WrittenCount;

    public long Position
    {
        get => Writer.WrittenCount;
        set => throw new NotImplementedException($"{nameof(HeapOutputStream)} cannot seek");
    }

    public void Write(scoped ReadOnlySpan<byte> buffer)
        => Writer.Write(buffer);

    ValueTask IValueOutputStream.WriteAsync(ReadOnlyMemory<byte> buffer)
    {
        Writer.Write(buffer.Span);
        return ValueTask.CompletedTask;
    }

    public ReadOnlySpan<byte> WrittenSpan
        => Writer.WrittenMemory.Span;

    public ReadOnlyMemory<byte> WrittenMemory
        => Writer.WrittenMemory;

    public void Advance(int count) => Writer.Advance(count);
    public Memory<byte> GetMemory(int sizeHint) => Writer.GetMemory(sizeHint);
    public Span<byte> GetSpan(int sizeHint) => Writer.GetSpan(sizeHint);

    public void Dispose()
        => Writer.Dispose();
}
