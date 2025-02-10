namespace ShortDev.IO.Input;

public interface IEndianReader : IReader
{
    bool UseLittleEndian { get; }
}
