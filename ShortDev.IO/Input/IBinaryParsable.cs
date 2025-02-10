namespace ShortDev.IO.Input;

public interface IBinaryParsable<TSelf> where TSelf : IBinaryParsable<TSelf>
{
    static abstract TSelf Parse<TReader>(ref TReader reader) where TReader : struct, IEndianReader, allows ref struct;
}
