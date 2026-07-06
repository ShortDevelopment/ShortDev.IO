using System;
using System.Threading.Tasks;

namespace ShortDev.IO.ValueStream;

public interface IValueInputStream : IDisposable
{
    void Skip(int count);
    void Read(scoped Span<byte> buffer);
    ValueTask ReadAsync(Memory<byte> buffer);

    bool TryReadSlice(int length, out ReadOnlySpan<byte> slice)
    {
        slice = default;
        return false;
    }
}
