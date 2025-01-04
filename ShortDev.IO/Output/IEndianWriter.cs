namespace ShortDev.IO.Output;

public interface IEndianWriter : IWriter
{
    bool UseLittleEndian { get; }
}
