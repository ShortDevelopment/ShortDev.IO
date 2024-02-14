using System;
using System.Diagnostics;
using ShortDev.IO.Input;

namespace ShortDev.IO;

public static class BinaryDebug
{
    [Conditional("DEBUG")]
    public static void PrintContent(this ref EndianReader reader)
        => reader.Buffer.AsSpan().PrintContent();

    [Conditional("DEBUG")]
    public static void PrintContent(this ReadOnlySpan<byte> content)
    {
        var hex = BitConverter.ToString(content.ToArray()).Replace("-", null);
        Debug.Print(hex);
    }
}
