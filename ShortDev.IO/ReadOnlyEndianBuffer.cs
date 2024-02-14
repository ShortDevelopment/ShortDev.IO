using System;
using System.IO;

namespace ShortDev.IO;

public ref struct ReadOnlyEndianBuffer
{
    int _bufferPosition = 0;

    readonly ReadOnlySpan<byte> _buffer;
    readonly Stream? _stream;

    public ReadOnlyEndianBuffer(ReadOnlySpan<byte> buffer)
        => _buffer = buffer;

    public ReadOnlyEndianBuffer(Stream stream)
        => _stream = stream;

    public void ReadBytes(scoped Span<byte> buffer)
    {
        if (_stream != null)
        {
            _stream.ReadExactly(buffer);
            return;
        }

        _buffer.Slice(_bufferPosition, buffer.Length).CopyTo(buffer);
        _bufferPosition += buffer.Length;
    }

    public ReadOnlySpan<byte> ReadBytes(int length)
    {
        if (_stream != null)
        {
            Span<byte> buffer = new byte[length];
            _stream.ReadExactly(buffer);
            return buffer;
        }

        var slice = _buffer.Slice(_bufferPosition, length);
        _bufferPosition += length;
        return slice;
    }

    public byte ReadByte()
    {
        if (_stream != null)
        {
            var buffer = _stream.ReadByte();
            if (buffer == -1)
                throw new EndOfStreamException();

            return (byte)buffer;
        }

        return _buffer[_bufferPosition++];
    }

    public readonly ReadOnlySpan<byte> AsSpan()
    {
        if (_stream != null)
            throw new InvalidOperationException("Not supported by stream");

        return _buffer;
    }

    public readonly long Length
        => _stream?.Length ?? _buffer.Length;

    public long Position
    {
        readonly get => _stream?.Position ?? _bufferPosition;
        set
        {
            if (_stream != null)
            {
                _stream.Position = value;
                return;
            }

            if (value < 0 || value >= Length)
                throw new ArgumentOutOfRangeException(nameof(value));

            _bufferPosition = (int)value;
        }
    }

    public readonly bool IsAtEnd
        => Length - Position <= 0;

    public ReadOnlySpan<byte> ReadToEnd()
        => ReadBytes((int)(Length - Position));
}
