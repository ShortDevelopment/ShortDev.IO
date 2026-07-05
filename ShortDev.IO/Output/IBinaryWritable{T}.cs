namespace ShortDev.IO.Output;

public interface IBinaryWritable<TSelf> : IBinaryWritable
    where TSelf : IBinaryWritable<TSelf>
{
    /// <summary>
    /// Gets the minimum required size in bytes to write this message to a buffer
    /// </summary>
    ulong MinimumSize
    {
        get
        {
            CalcSizeWriter writer = new();
            Write(ref writer);
            return writer.WrittenBinarySize;
        }
    }
}
