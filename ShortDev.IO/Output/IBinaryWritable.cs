namespace ShortDev.IO.Output;

public interface IBinaryWritable
{
    void Write<TWriter>(ref TWriter writer) where TWriter : struct, IEndianWriter, allows ref struct;
}
