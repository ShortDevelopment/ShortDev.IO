using System;
using System.IO;
using System.Threading.Tasks;

namespace ShortDev.IO.ValueStream;

public readonly struct StreamWrapperStream(Stream stream) : IValueInputStream, IValueOutputStream, IValueStreamPosition
{
    readonly Stream _stream = stream;

    public long Length => _stream.Length;

    public long Position
    {
        get => _stream.Position;
        set => _stream.Position = value;
    }

    public void Skip(int count)
        => Position += count;

    public void Read(Span<byte> buffer)
        => _stream.ReadExactly(buffer);

    public ValueTask ReadAsync(Memory<byte> buffer)
        => _stream.ReadExactlyAsync(buffer);

    public void Write(ReadOnlySpan<byte> buffer)
        => _stream.Write(buffer);

    public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer)
        => _stream.WriteAsync(buffer);

    public void Dispose()
        => _stream.Dispose();
}
