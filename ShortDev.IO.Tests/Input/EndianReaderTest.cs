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

        TestRead<FixedStackStream>(endianness, new(data));
    }

    [Theory]
    [InlineData(Endianness.LittleEndian)]
    [InlineData(Endianness.BigEndian)]
    public void FixedReadOnlyStackStream(Endianness endianness)
    {
        byte[] data = [1, 2, 3, 4, 5];

        TestRead<FixedReadOnlyStackStream>(endianness, new(data));
    }

    [Theory]
    [InlineData(Endianness.LittleEndian)]
    [InlineData(Endianness.BigEndian)]
    public void FixedReadOnlyHeapStream(Endianness endianness)
    {
        byte[] data = [1, 2, 3, 4, 5];

        TestRead<FixedReadOnlyHeapStream>(endianness, new(data));
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
