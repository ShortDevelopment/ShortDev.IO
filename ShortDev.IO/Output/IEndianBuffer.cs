using System;
using System.Buffers;

namespace ShortDev.IO.Output;

public interface IEndianBuffer : IBufferWriter<byte>, IDisposable
{
    void Write(byte value);
    void Write(ReadOnlySpan<byte> buffer);
    long Size { get; }

    ReadOnlySpan<byte> WrittenSpan { get; }

    void Clear();
}
