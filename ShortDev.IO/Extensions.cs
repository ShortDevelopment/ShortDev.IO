using System;
using System.Runtime.InteropServices;

namespace ShortDev.IO;

public static class Extensions
{
    public static Span<T> AsSpanUnsafe<T>(this ReadOnlySpan<T> buffer)
        => MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(buffer), buffer.Length);

    public static Span<T> ToReversed<T>(this ReadOnlySpan<T> span)
    {
        Span<T> result = span.ToArray();
        result.Reverse();
        return result;
    }

    public static Span<T> ReverseInPlace<T>(this Span<T> span)
    {
        span.Reverse();
        return span;
    }
}
