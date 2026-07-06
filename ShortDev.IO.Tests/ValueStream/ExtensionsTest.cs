using ShortDev.IO.ValueStream;

namespace ShortDev.IO.Tests.ValueStream;

public sealed class ExtensionsTest
{
    [Fact]
    public void IsAtEnd_ShouldReturnTrue_WhenAtEnd()
    {
        {
            ReadOnlySpanStream buffer = new([]);
            Assert.True(buffer.IsAtEnd());
        }

        {
            ReadOnlySpanStream buffer = new([1]);
            Assert.False(buffer.IsAtEnd());
            buffer.ReadByte();
            Assert.True(buffer.IsAtEnd());
        }
    }

    [Fact]
    public void IsAtEnd_ShouldReturnFalse_WhenNotAtEnd()
    {
        {
            ReadOnlySpanStream buffer = new([1]);
            Assert.False(buffer.IsAtEnd());
        }

        {
            ReadOnlySpanStream buffer = new([1, 2]);
            Assert.False(buffer.IsAtEnd());
        }
    }

    [Fact]
    public void Position_ShouldNotThrow_IfInBounds()
    {
        ReadOnlySpanStream buffer = new([1, 2, 3, 4]);
        Assert.False(buffer.IsAtEnd());

        buffer.Position = 0;
        buffer.Position = 1;
        buffer.Position = 2;
        buffer.Position = 3;
        Assert.False(buffer.IsAtEnd());

        buffer.Position = 4;
        Assert.True(buffer.IsAtEnd());
    }

    [Fact]
    public void Position_ShouldThrow_IfOutOfBounds()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            ReadOnlySpanStream buffer = new([1, 2, 3, 4]);
            buffer.Position = -1;
        });
        Assert.Throws<ArgumentOutOfRangeException>(() =>
        {
            ReadOnlySpanStream buffer = new([1, 2, 3, 4]);
            buffer.Position = 5;
        });
    }
}
