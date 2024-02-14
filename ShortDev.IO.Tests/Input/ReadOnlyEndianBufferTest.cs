using ShortDev.IO.Input;

namespace ShortDev.IO.Tests.Input;

public sealed class ReadOnlyEndianBufferTest
{
    [Fact]
    public void IsAtEnd_ShouldReturnTrue_WhenAtEnd()
    {
        {
            ReadOnlyEndianBuffer buffer = new([]);
            Assert.True(buffer.IsAtEnd);
        }

        {
            ReadOnlyEndianBuffer buffer = new([1]);
            Assert.False(buffer.IsAtEnd);
            buffer.ReadByte();
            Assert.True(buffer.IsAtEnd);
        }
    }

    [Fact]
    public void IsAtEnd_ShouldReturnFalse_WhenNotAtEnd()
    {
        {
            ReadOnlyEndianBuffer buffer = new([1]);
            Assert.False(buffer.IsAtEnd);
        }

        {
            ReadOnlyEndianBuffer buffer = new([1, 2]);
            Assert.False(buffer.IsAtEnd);
        }
    }
}
