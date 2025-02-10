using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace ShortDev.IO;

public static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> AsSpanUnsafe<T>(this ReadOnlySpan<T> buffer)
        => MemoryMarshal.CreateSpan(ref MemoryMarshal.GetReference(buffer), buffer.Length);

    /// <inheritdoc cref="Read(ReadOnlySpan{byte}, Span{byte}, ref long)" />
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Read(this Span<byte> bytes, Span<byte> destination, ref long position)
        => Read((ReadOnlySpan<byte>)bytes, destination, ref position);

    /// <summary>
    /// Reads <paramref name="destination"/> from <paramref name="bytes"/> at <paramref name="position"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Read(this ReadOnlySpan<byte> bytes, Span<byte> destination, ref long position)
    {
        ReadOnlySpan<byte> source = bytes.Slice((int)position, destination.Length);
        source.CopyTo(destination);
        position += destination.Length;
    }

    /// <summary>
    /// Reads slice of length <paramref name="length"/> from <paramref name="bytes"/> at <paramref name="position"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<byte> ReadSlice(this Span<byte> bytes, int length, scoped ref long position)
    {
        var slice = bytes.Slice((int)position, length);
        position += length;
        return slice;
    }

    /// <summary>
    /// Reads slice of length <paramref name="length"/> from <paramref name="bytes"/> at <paramref name="position"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<byte> ReadSlice(this ReadOnlySpan<byte> bytes, int length, scoped ref long position)
    {
        var slice = bytes.Slice((int)position, length);
        position += length;
        return slice;
    }

    /// <summary>
    /// Reads slice of length <paramref name="length"/> from <paramref name="bytes"/> at <paramref name="position"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<byte> ReadSlice(this ReadOnlyMemory<byte> bytes, int length, scoped ref long position)
    {
        var slice = bytes.Slice((int)position, length);
        position += length;
        return slice;
    }

    /// <summary>
    /// Writes <paramref name="source"/> to <paramref name="bytes"/> at <paramref name="position"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Write(this Span<byte> bytes, ReadOnlySpan<byte> source, ref long position)
    {
        Span<byte> destination = bytes.Slice((int)position, source.Length);
        source.CopyTo(destination);
        position += source.Length;
    }
}
