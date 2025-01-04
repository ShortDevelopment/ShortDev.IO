#if STREAM_OUTPUT
using Microsoft.IO;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace ShortDev.IO.Output;

public readonly struct StreamOutputBuffer(RecyclableMemoryStream writer) : IEndianBuffer
{
    readonly RecyclableMemoryStream _writer = writer;

    public void Write(byte value)
        => _writer.Write([value]);

    public void Write(ReadOnlySpan<byte> buffer)
        => _writer.Write(buffer);

    public long Size
        => _writer.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ArraySegment<byte> AsArraySegment()
    {
        if (!_writer.TryGetBuffer(out var buffer))
            throw new InvalidOperationException("Cannot get a single buffer");

        return buffer;
    }

    public Span<byte> WrittenSpan
        => AsArraySegment();

    public Memory<byte> WrittenMemory
        => AsArraySegment();

    public void Clear()
        => _writer.SetLength(0);

    public void CopyTo(Stream destination)
        => _writer.CopyTo(destination);

    public void Advance(int count) => _writer.Advance(count);
    public Memory<byte> GetMemory(int sizeHint) => _writer.GetMemory(sizeHint);
    public Span<byte> GetSpan(int sizeHint) => _writer.GetSpan(sizeHint);

    public void Dispose()
        => _writer.Dispose();
}
#endif