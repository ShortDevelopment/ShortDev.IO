using System;
using System.Runtime.InteropServices;

namespace ShortDev.IO;

public static class Extensions
{
    public static Span<T> AsSpanUnsafe<T>(this ReadOnlySpan<T> buffer)
        => MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(buffer), buffer.Length);
}
