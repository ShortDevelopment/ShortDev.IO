using ShortDev.IO.Buffers;
using ShortDev.IO.Input;
using ShortDev.IO.ValueStream;
using System.Buffers;

namespace ShortDev.IO.Tests.Input;

public class EndianReaderTest
{
    [Theory]
    [InlineData(Endianness.LittleEndian)]
    [InlineData(Endianness.BigEndian)]
    public void FixedStackStream(Endianness endianness)
    {
        byte[] data = [1, 2, 3, 4, 5];

        TestRead<SpanStream>(endianness, new(data));
    }

    [Theory]
    [InlineData(Endianness.LittleEndian)]
    [InlineData(Endianness.BigEndian)]
    public void FixedReadOnlyStackStream(Endianness endianness)
    {
        byte[] data = [1, 2, 3, 4, 5];

        TestRead<ReadOnlySpanStream>(endianness, new(data));
    }

    [Theory]
    [InlineData(Endianness.LittleEndian)]
    [InlineData(Endianness.BigEndian)]
    public void FixedReadOnlyHeapStream(Endianness endianness)
    {
        byte[] data = [1, 2, 3, 4, 5];

        TestRead<ReadOnlyMemoryStream>(endianness, new(data));
    }

    [Theory]
    [InlineData(Endianness.LittleEndian)]
    [InlineData(Endianness.BigEndian)]
    public void DelegatingInputStream(Endianness endianness)
    {
        byte[] data = [1, 2, 3, 4, 5];

        var innerReader = EndianReader.FromSpan(endianness, data);

        DelegatingInputStream<EndianReader<ReadOnlySpanStream>> stream = new(ref innerReader);

        // Test keep state
        Assert.Equal(1, stream.ReadByte());
        Assert.Equal(2, stream.ReadByte());
        Assert.Equal(3, stream.ReadByte());
        Assert.Equal(4, stream.ReadByte());
        Assert.Equal(5, stream.ReadByte());

        try
        {
            stream.ReadByte();
            Assert.Fail("Expected ReadByte to throw");
        }
        catch (Exception ex)
        {
            Assert.NotNull(ex);
        }

        // Reset
        innerReader.Stream.Position = 0;

        // Test skip
        stream.Skip(4);
        Assert.Equal(5, stream.ReadByte());
    }

    static void TestRead<TStream>(Endianness endianness, TStream stream) where TStream : struct, IValueInputStream, IValueStreamPosition, allows ref struct
    {
        EndianReader<TStream> reader = new(endianness) { Stream = stream };

        if (endianness == Endianness.BigEndian)
            Assert.False(reader.UseLittleEndian);
        else
            Assert.True(reader.UseLittleEndian);

        Assert.Equal(5, reader.Stream.Length);

        Assert.False(reader.Stream.IsAtEnd());
        Assert.Equal(1, reader.ReadUInt8());
        Assert.False(reader.Stream.IsAtEnd());
        Assert.Equal(2, reader.ReadUInt8());
        Assert.False(reader.Stream.IsAtEnd());
        Assert.Equal(3, reader.ReadUInt8());
        Assert.False(reader.Stream.IsAtEnd());
        Assert.Equal(4, reader.ReadUInt8());
        Assert.False(reader.Stream.IsAtEnd());
        Assert.Equal(5, reader.ReadUInt8());
        Assert.True(reader.Stream.IsAtEnd());

        try
        {
            reader.ReadUInt8();
            Assert.Fail("Expected ReadUInt8 to throw");
        }
        catch (Exception ex)
        {
            Assert.NotNull(ex);
        }
    }
}
