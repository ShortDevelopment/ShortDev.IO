using DotNext.Buffers;
using ShortDev.IO.Bond;
using ShortDev.IO.Output;
using ShortDev.IO.ValueStream;
using System.Buffers;
using System.Runtime.CompilerServices;

namespace ShortDev.IO.Tests.Output;

public class EndianWriterTest
{
    [Theory]
    [InlineData(Endianness.LittleEndian)]
    [InlineData(Endianness.BigEndian)]
    public void FixedStackStream(Endianness endianness)
    {
        byte[] data = new byte[5];
        SpanStream stream = new(data);
        Assert.Equal(0, stream.Position);

        TestWriter(endianness, ref stream);

        Assert.Equal(5, stream.Position);
        Assert.Equal([1, 2, 3, 4, 5], data);
    }

    [Theory]
    [InlineData(Endianness.LittleEndian)]
    [InlineData(Endianness.BigEndian)]
    public void HeapOutputStream(Endianness endianness)
    {
        PoolingArrayBufferWriter<byte> writer = [];
        HeapOutputStream stream = new() { Writer = writer };
        Assert.Equal(0, stream.Position);

        TestWriter(endianness, ref stream);

        Assert.Equal(5, stream.Position);
        Assert.Equal([1, 2, 3, 4, 5], stream.WrittenSpan.ToArray());
        Assert.Equal([1, 2, 3, 4, 5], stream.WrittenMemory.ToArray());
        Assert.Equal([1, 2, 3, 4, 5], writer.WrittenMemory.ToArray());
    }

    static void TestWriter<TStream>(Endianness endianness, ref TStream stream) where TStream : struct, IValueOutputStream, IBufferWriter<byte>, IValueStreamPosition, allows ref struct
    {
        EndianWriter<TStream> writer = new(endianness) { Stream = stream };

        if (endianness == Endianness.BigEndian)
            Assert.False(writer.UseLittleEndian);
        else
            Assert.True(writer.UseLittleEndian);

        writer.Write((byte)1);
        writer.Write((byte)2);
        writer.Write((byte)3);
        writer.Write((byte)4);
        writer.Write((byte)5);

        stream = writer.Stream;
    }

    [Fact]
    public unsafe void PointerException()
    {
        var writer = EndianWriter.Create(Endianness.BigEndian, ArrayPool<byte>.Shared);
        try
        {
            var pWriter = (EndianWriter<HeapOutputStream>*)Unsafe.AsPointer(ref writer);
            pWriter->WriteVarUInt32(1234);
        }
        finally
        {
            writer.Dispose();
        }
    }
}
