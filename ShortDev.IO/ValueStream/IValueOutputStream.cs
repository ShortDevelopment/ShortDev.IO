using System;
using System.Buffers;
using System.Threading.Tasks;

namespace ShortDev.IO.ValueStream;

public interface IValueOutputStream : IDisposable
{
    void Write(scoped ReadOnlySpan<byte> buffer);
    ValueTask WriteAsync(ReadOnlyMemory<byte> buffer);
}
