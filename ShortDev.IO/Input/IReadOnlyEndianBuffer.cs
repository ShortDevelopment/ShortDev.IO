using System;

namespace ShortDev.IO.Input;

public interface IReadOnlyEndianBuffer
{
    byte ReadByte();
    void ReadBytes(scoped Span<byte> buffer);

    ReadOnlySpan<byte> ReadBytes(int length);
    ReadOnlySpan<byte> ReadToEnd();

    long Length { get; }
    long Position { get; set; }
    bool IsAtEnd { get; }
}
