using System;
using System.Threading.Tasks;

namespace ShortDev.IO.ValueStream;

public ref struct FixedReadOnlyHeapStream(ReadOnlyMemory<byte> buffer, Action? disposeAction = null) : IValueInputStream, IValueStreamPosition
{
    readonly ReadOnlyMemory<byte> _buffer = buffer;

    public long Length { get; } = buffer.Length;

    private long _position;
    public long Position
    {
        readonly get => _position;
        set => Extensions.SetPosition(ref _position, Length, value);
    }

    public void Read(Span<byte> buffer)
        => _buffer.Span.Read(buffer, ref _position);

    public ReadOnlySpan<byte> ReadSlice(int length)
        => _buffer.Span.ReadSlice(length, ref _position);

    public bool TryReadSlice(int length, out ReadOnlySpan<byte> slice)
    {
        slice = _buffer.ReadSlice(length, ref _position).Span;
        return true;
    }

    ValueTask IValueInputStream.ReadAsync(Memory<byte> buffer)
    {
        _buffer.Span.Read(buffer.Span, ref _position);
        return ValueTask.CompletedTask;
    }

    public readonly void Dispose()
        => disposeAction?.Invoke();
}
