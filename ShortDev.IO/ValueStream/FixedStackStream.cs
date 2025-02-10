using System;
using System.Buffers;
using System.Threading.Tasks;

namespace ShortDev.IO.ValueStream;

public ref struct FixedStackStream(Span<byte> buffer) : IValueInputStream, IValueOutputStream, IBufferWriter<byte>, IValueStreamPosition
{
    readonly Span<byte> _buffer = buffer;

    public readonly long Length { get; } = buffer.Length;

    long _position;
    public long Position
    {
        readonly get => _position;
        set => Extensions.SetPosition(ref _position, Length, value);
    }

    public void Read(scoped Span<byte> buffer)
        => _buffer.Read(buffer, ref _position);

    public ReadOnlySpan<byte> ReadSlice(int length)
        => _buffer.ReadSlice(length, ref _position);

    public bool TryReadSlice(int length, out ReadOnlySpan<byte> slice)
    {
        slice = _buffer.ReadSlice(length, ref _position);
        return true;
    }

    ValueTask IValueInputStream.ReadAsync(Memory<byte> buffer)
    {
        _buffer.Read(buffer.Span, ref _position);
        return ValueTask.CompletedTask;
    }

    public void Write(ReadOnlySpan<byte> buffer)
        => _buffer.Write(buffer, ref _position);

    ValueTask IValueOutputStream.WriteAsync(ReadOnlyMemory<byte> buffer)
    {
        _buffer.Write(buffer.Span, ref _position);
        return ValueTask.CompletedTask;
    }

    public readonly Span<byte> GetSpan(int sizeHint = 0)
    {
        if (sizeHint <= 0)
            return _buffer[(int)_position..];

        return _buffer.Slice((int)_position, sizeHint);
    }

    public readonly Memory<byte> GetMemory(int sizeHint = 0)
        => throw new NotImplementedException($"{nameof(FixedStackStream)} cannot use heap memory");

    public void Advance(int count)
        => Position += count;

    public readonly void Dispose() { }
}
