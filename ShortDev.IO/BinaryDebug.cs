using ShortDev.IO.Input;
using System;
using System.Diagnostics;

namespace ShortDev.IO;

public static class BinaryDebug
{
    [Conditional("DEBUG")]
    public static void PrintContent(this ref EndianReader reader)
        => reader.ReadToEnd().PrintContent();

    [Conditional("DEBUG")]
    public static void PrintContent(this ReadOnlySpan<byte> content)
        => Debug.Print(Convert.ToHexString(content));
}
