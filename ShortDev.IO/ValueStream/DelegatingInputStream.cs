using ShortDev.IO.Input;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ShortDev.IO.ValueStream;

internal unsafe readonly ref struct DelegatingInputStream<TReader>(ref TReader reader) : IValueInputStream where TReader : struct, IEndianReader, allows ref struct
{
    readonly TReader* input = (TReader*)Unsafe.AsPointer(ref reader);

    public readonly void Read(scoped Span<byte> buffer)
        => input->ReadBytes(buffer);

    public readonly ValueTask ReadAsync(Memory<byte> buffer)
        => throw new NotImplementedException();

    public readonly bool TryReadSlice(int length, out ReadOnlySpan<byte> slice)
        => throw new NotImplementedException();

    public void Dispose()
        => input->Dispose();
}
