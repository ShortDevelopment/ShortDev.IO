using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace ShortDev.IO;

public static class Extensions
{
    public static Span<T> AsSpan<T>(this ReadOnlySpan<T> buffer)
        => MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(buffer), buffer.Length);

#if NETSTANDARD

    // https://source.dot.net/#System.Private.CoreLib/src/libraries/System.Private.CoreLib/src/System/IO/Stream.cs,41b23b4f3a16f72a,references
    public static void ReadExactly(this Stream stream, Span<byte> buffer)
    {
        int totalRead = 0;
        while (totalRead < buffer.Length)
        {
            int read = stream.Read(buffer[totalRead..]);
            if (read == 0)
                throw new EndOfStreamException();

            totalRead += read;
        }

        Debug.Assert(totalRead == buffer.Length);
    }
#endif
}
