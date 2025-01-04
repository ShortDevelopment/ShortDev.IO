using System;

namespace ShortDev.IO.Input;

public ref struct SpanInputBuffer(ReadOnlySpan<byte> buffer) : IReadOnlyEndianBuffer
{
    int _bufferPosition = 0;

    readonly ReadOnlySpan<byte> _buffer = buffer;

    public void ReadBytes(scoped Span<byte> buffer)
    {
        _buffer.Slice(_bufferPosition, buffer.Length).CopyTo(buffer);
        _bufferPosition += buffer.Length;
    }

    public ReadOnlySpan<byte> ReadBytes(int length)
    {
        var slice = _buffer.Slice(_bufferPosition, length);
        _bufferPosition += length;
        return slice;
    }

    public byte ReadByte()
        => _buffer[_bufferPosition++];

    public readonly ReadOnlySpan<byte> Span
        => _buffer;

    public readonly long Length
        => _buffer.Length;

    public long Position
    {
        readonly get => _bufferPosition;
        set
        {
            if (value < 0 || value > Length)
                throw new ArgumentOutOfRangeException(nameof(value));

            _bufferPosition = (int)value;
        }
    }

    public readonly bool IsAtEnd
        => Length - Position <= 0;

    public ReadOnlySpan<byte> ReadToEnd()
        => ReadBytes((int)(Length - Position));
}
