namespace ShortDev.IO.Output;

public interface IBinaryWritable<TSelf> where TSelf : IBinaryWritable<TSelf>, IBinaryWritable
{
    /// <summary>
    /// Gets the minimum required size in bytes to write this message to a buffer
    /// </summary>
    ulong MinimumSize
    {
        get
        {
            CalcSizeWriter writer = new();
            ((IBinaryWritable)this).Write(ref writer);
            return writer.WrittenBinarySize;
        }
    }
}
