using System;
using System.Runtime.CompilerServices;

namespace ShortDev.IO.ValueStream;

public static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static void SetPosition(ref long position, long length, long value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, 0);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, length);

        position = value;
    }

    /// <summary>
    /// Wether end of file has been reached.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool IsAtEnd<TStream>(this ref TStream stream) where TStream : struct, IValueStreamPosition, allows ref struct
        => stream.Position >= stream.Length;

    /// <summary>
    /// Reads a single byte from the stream.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ReadByte<TStream>(this ref TStream stream) where TStream : struct, IValueInputStream, allows ref struct
    {
        byte value = 0;
        stream.Read(new Span<byte>(ref value));
        return value;
    }
}
