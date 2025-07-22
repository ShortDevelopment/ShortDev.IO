using ShortDev.IO.Output;
using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ShortDev.IO.ValueStream;

internal unsafe readonly ref struct DelegatingOutputStream<TWriter>(ref TWriter writer) : IValueOutputStream, IBufferWriter<byte> where TWriter : struct, IEndianWriter, allows ref struct
{
    readonly TWriter* _output = (TWriter*)Unsafe.AsPointer(ref writer);

    public void Write(scoped ReadOnlySpan<byte> buffer)
        => _output->Write(buffer);

    public ValueTask WriteAsync(ReadOnlyMemory<byte> buffer)
        => throw new NotImplementedException();

    public Span<byte> GetSpan(int sizeHint = 0)
        => _output->GetSpan(sizeHint);

    public Memory<byte> GetMemory(int sizeHint = 0)
        => _output->GetMemory(sizeHint);

    public void Advance(int count)
        => _output->Advance(count);

    public void Dispose()
        => _output->Dispose();
}
