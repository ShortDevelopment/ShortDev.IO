using ShortDev.IO.Bond;
using ShortDev.IO.Input;
using ShortDev.IO.Output;
using ShortDev.IO.ValueStream;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace ShortDev.IO.Tests.Bond;

public class CompactBinaryTest
{
    [Theory]
    [InlineData(Endianness.LittleEndian)]
    [InlineData(Endianness.BigEndian)]
    public void Test(Endianness endianness)
    {
        var writer = EndianWriter.Create(endianness, ArrayPool<byte>.Shared);
        var bondWriter = CompactBinaryProtocol.CreateWriter(ref writer);

        bondWriter.WriteStructBegin();
        {
            bondWriter.WriteFieldBegin(BondDataType.BT_UINT64, id: 1);
            bondWriter.WriteUInt64(42);
            bondWriter.WriteFieldEnd();

            bondWriter.WriteFieldBegin(BondDataType.BT_WSTRING, id: 2);
            bondWriter.WriteWString("42");
            bondWriter.WriteFieldEnd();
        }
        bondWriter.WriteStructEnd();

        var reader = EndianReader.FromMemory(endianness, writer.Stream.WrittenMemory);
        var bondReader = CompactBinaryProtocol.CreateReader(ref reader);

        // Test skip
        bondReader.Skip(BondDataType.BT_STRUCT);
        Assert.True(reader.Stream.IsAtEnd());

        // Reset reader
        reader.Stream.Position = 0;

        // Test individual read
        bondReader.ReadStructBegin();
        {
            bondReader.ReadFieldBegin(out var type1, out var id1);
            Assert.Equal(BondDataType.BT_UINT64, type1);
            Assert.Equal(1, id1);
            Assert.Equal(42u, bondReader.ReadUInt64());
            bondReader.ReadFieldEnd();

            bondReader.ReadFieldBegin(out var type2, out var id2);
            Assert.Equal(BondDataType.BT_WSTRING, type2);
            Assert.Equal(2, id2);
            Assert.Equal("42", bondReader.ReadWString());
            bondReader.ReadFieldEnd();

            bondReader.ReadFieldBegin(out var type3, out _);
            Assert.Equal(BondDataType.BT_STOP, type3);
        }
        bondReader.ReadStructEnd();
        Assert.True(reader.Stream.IsAtEnd());
    }
}
