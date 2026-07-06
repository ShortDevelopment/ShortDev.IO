using ShortDev.IO.Input;
using ShortDev.IO.Output;

namespace ShortDev.IO.Bond;

/// <summary>
/// Utils for the Compact Binary tagged protocol
/// </summary>
public static class CompactBinaryProtocol
{
    public static CompactBinaryReader<TReader> CreateReader<TReader>(ref TReader reader, ushort version = 1)
        where TReader : struct, IEndianReader, allows ref struct
    {
        return new(ref reader, version);
    }

    public static CompactBinaryWriter<TWriter> CreateWriter<TWriter>(ref TWriter writer, ushort version = 1)
    where TWriter : struct, IEndianWriter, allows ref struct
    {
        return new(ref writer, version);
    }
}
