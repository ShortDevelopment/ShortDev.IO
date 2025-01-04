using System;
using System.IO;

namespace ShortDev.IO.Input;

public readonly struct StreamInputBuffer(Stream stream) : IReadOnlyEndianBuffer
{
    readonly Stream _stream = stream;

    public readonly void ReadBytes(scoped Span<byte> buffer)
        => _stream.ReadExactly(buffer);

    public readonly ReadOnlySpan<byte> ReadBytes(int length)
    {
        Span<byte> buffer = new byte[length];
        _stream.ReadExactly(buffer);
        return buffer;
    }

    public readonly byte ReadByte()
    {
        var buffer = _stream.ReadByte();
        if (buffer == -1)
            throw new EndOfStreamException();

        return (byte)buffer;
    }

    public readonly ReadOnlySpan<byte> Span
        => throw new InvalidOperationException("Not supported by stream");

    public readonly long Length
        => _stream.Length;

    public readonly long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    public readonly bool IsAtEnd
        => Length - Position <= 0;

    public readonly ReadOnlySpan<byte> ReadToEnd()
        => ReadBytes((int)(Length - Position));
}
