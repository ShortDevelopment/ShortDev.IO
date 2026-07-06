using ShortDev.IO.Input;
using ShortDev.IO.Output;
using System.Runtime.CompilerServices;

namespace ShortDev.IO.Bond;

public static class Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteVarUInt16<TWriter>(this ref TWriter writer, ushort value)
        where TWriter : struct, IEndianWriter, allows ref struct
    {
        var buffer = writer.GetSpan(3);
        var length = IntegerHelper.EncodeVarUInt16(buffer, value, 0);
        writer.Advance(length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteVarUInt32<TWriter>(this ref TWriter writer, uint value)
        where TWriter : struct, IEndianWriter, allows ref struct
    {
        var buffer = writer.GetSpan(5);
        var length = IntegerHelper.EncodeVarUInt32(buffer, value, 0);
        writer.Advance(length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteVarUInt64<TWriter>(this ref TWriter writer, ulong value)
        where TWriter : struct, IEndianWriter, allows ref struct
    {
        var buffer = writer.GetSpan(10);
        var length = IntegerHelper.EncodeVarUInt64(buffer, value, 0);
        writer.Advance(length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadVarUInt16<TReader>(this ref TReader reader)
        where TReader : struct, IEndianReader, allows ref struct
    {
        // byte 0
        uint result = reader.ReadUInt8();
        if (0x80u <= result)
        {
            // byte 1
            uint raw = reader.ReadUInt8();
            result = result & 0x7Fu | (raw & 0x7Fu) << 7;
            if (0x80u <= raw)
            {
                // byte 2
                raw = reader.ReadUInt8();
                result |= raw << 14;
            }
        }
        return (ushort)result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadVarUInt32<TReader>(this ref TReader reader)
        where TReader : struct, IEndianReader, allows ref struct
    {
        // byte 0
        uint result = reader.ReadUInt8();
        if (0x80u <= result)
        {
            // byte 1
            uint raw = reader.ReadUInt8();
            result = result & 0x7Fu | (raw & 0x7Fu) << 7;
            if (0x80u <= raw)
            {
                // byte 2
                raw = reader.ReadUInt8();
                result |= (raw & 0x7Fu) << 14;
                if (0x80u <= raw)
                {
                    // byte 3
                    raw = reader.ReadUInt8();
                    result |= (raw & 0x7Fu) << 21;
                    if (0x80u <= raw)
                    {
                        // byte 4
                        raw = reader.ReadUInt8();
                        result |= raw << 28;
                    }
                }
            }
        }
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ReadVarUInt64<TReader>(this ref TReader reader)
        where TReader : struct, IEndianReader, allows ref struct
    {
        // byte 0
        ulong result = reader.ReadUInt8();
        if (0x80u <= result)
        {
            // byte 1
            ulong raw = reader.ReadUInt8();
            result = result & 0x7Fu | (raw & 0x7Fu) << 7;
            if (0x80u <= raw)
            {
                // byte 2
                raw = reader.ReadUInt8();
                result |= (raw & 0x7Fu) << 14;
                if (0x80u <= raw)
                {
                    // byte 3
                    raw = reader.ReadUInt8();
                    result |= (raw & 0x7Fu) << 21;
                    if (0x80u <= raw)
                    {
                        // byte 4
                        raw = reader.ReadUInt8();
                        result |= (raw & 0x7Fu) << 28;
                        if (0x80u <= raw)
                        {
                            // byte 5
                            raw = reader.ReadUInt8();
                            result |= (raw & 0x7Fu) << 35;
                            if (0x80u <= raw)
                            {
                                // byte 6
                                raw = reader.ReadUInt8();
                                result |= (raw & 0x7Fu) << 42;
                                if (0x80u <= raw)
                                {
                                    // byte 7
                                    raw = reader.ReadUInt8();
                                    result |= (raw & 0x7Fu) << 49;
                                    if (0x80u <= raw)
                                    {
                                        // byte 8
                                        raw = reader.ReadUInt8();
                                        result |= raw << 56;
                                        if (0x80u <= raw)
                                        {
                                            // byte 9
                                            reader.ReadUInt8();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        return result;
    }
}
