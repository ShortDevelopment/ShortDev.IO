using System;
using System.Threading.Tasks;

namespace ShortDev.IO.ValueStream;

public ref struct FixedReadOnlyStackStream(ReadOnlySpan<byte> buffer) : IValueInputStream, IValueStreamPosition
{
    readonly ReadOnlySpan<byte> _buffer = buffer;

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

    public readonly void Dispose() { }
}
