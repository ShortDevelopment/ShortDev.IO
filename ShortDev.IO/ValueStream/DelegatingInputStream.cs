using ShortDev.IO.Input;
using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ShortDev.IO.ValueStream;

internal unsafe readonly ref struct DelegatingInputStream<TReader>(ref TReader reader) : IValueInputStream
    where TReader : struct, IEndianReader, allows ref struct
{
    readonly TReader* input = (TReader*)Unsafe.AsPointer(ref reader);

    public void Skip(int count)
        => input->SkipBytes(count);

    public void Read(scoped Span<byte> buffer)
        => input->ReadBytes(buffer);

    ValueTask IValueInputStream.ReadAsync(Memory<byte> buffer)
    {
        input->ReadBytes(buffer.Span);
        return ValueTask.CompletedTask;
    }

    public bool TryReadSlice(int length, out ReadOnlySpan<byte> slice)
        => input->TryReadSlice(length, out slice);

    public void Dispose()
        => input->Dispose();
}
