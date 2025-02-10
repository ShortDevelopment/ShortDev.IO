namespace ShortDev.IO.ValueStream;

public interface IValueStreamPosition
{
    long Length { get; }
    long Position { get; set; }
}
