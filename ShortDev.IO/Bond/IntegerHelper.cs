// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// https://github.com/microsoft/bond/blob/e98cfa5024de7aa15244659ee7e816f8f7fb0b60/cs/src/core/io/IntegerHelper.cs

using System;
using System.Runtime.CompilerServices;

namespace ShortDev.IO.Bond;

/// <summary>
/// Helper methods for encoding and decoding integer values.
/// </summary>
internal static class IntegerHelper
{
    public const int MaxBytesVarInt16 = 3;
    public const int MaxBytesVarInt32 = 5;
    public const int MaxBytesVarInt64 = 10;

    public static int GetVarUInt16Length(ushort value)
    {
        if (value < 1 << 7)
        {
            return 1;
        }
        else if (value < 1 << 14)
        {
            return 2;
        }
        else
        {
            return 3;
        }
    }

    public static int EncodeVarUInt16(Span<byte> data, ushort value, int index)
    {
        // byte 0
        if (value >= 0x80)
        {
            data[index++] = (byte)(value | 0x80);
            value >>= 7;
            // byte 1
            if (value >= 0x80)
            {
                data[index++] = (byte)(value | 0x80);
                value >>= 7;
            }
        }
        // byte 2
        data[index++] = (byte)value;
        return index;
    }

    public static int GetVarUInt32Length(uint value)
    {
        if (value < 1 << 7)
        {
            return 1;
        }
        else if (value < 1 << 14)
        {
            return 2;
        }
        else if (value < 1 << 21)
        {
            return 3;
        }
        else if (value < 1 << 28)
        {
            return 4;
        }
        else
        {
            return 5;
        }
    }

    public static int EncodeVarUInt32(Span<byte> data, uint value, int index)
    {
        // byte 0
        if (value >= 0x80)
        {
            data[index++] = (byte)(value | 0x80);
            value >>= 7;
            // byte 1
            if (value >= 0x80)
            {
                data[index++] = (byte)(value | 0x80);
                value >>= 7;
                // byte 2
                if (value >= 0x80)
                {
                    data[index++] = (byte)(value | 0x80);
                    value >>= 7;
                    // byte 3
                    if (value >= 0x80)
                    {
                        data[index++] = (byte)(value | 0x80);
                        value >>= 7;
                    }
                }
            }
        }
        // last byte
        data[index++] = (byte)value;
        return index;
    }

    public static int GetVarUInt64Length(ulong value)
    {
        if (value < 1UL << 7)
        {
            return 1;
        }
        else if (value < 1UL << 14)
        {
            return 2;
        }
        else if (value < 1UL << 21)
        {
            return 3;
        }
        else if (value < 1UL << 28)
        {
            return 4;
        }
        else if (value < 1UL << 35)
        {
            return 5;
        }
        else if (value < 1UL << 42)
        {
            return 6;
        }
        else if (value < 1UL << 49)
        {
            return 7;
        }
        else if (value < 1UL << 56)
        {
            return 8;
        }
        else if (value < 1UL << 63)
        {
            return 9;
        }
        else
        {
            return 10;
        }
    }

    public static int EncodeVarUInt64(Span<byte> data, ulong value, int index)
    {
        // byte 0
        if (value >= 0x80)
        {
            data[index++] = (byte)(value | 0x80);
            value >>= 7;
            // byte 1
            if (value >= 0x80)
            {
                data[index++] = (byte)(value | 0x80);
                value >>= 7;
                // byte 2
                if (value >= 0x80)
                {
                    data[index++] = (byte)(value | 0x80);
                    value >>= 7;
                    // byte 3
                    if (value >= 0x80)
                    {
                        data[index++] = (byte)(value | 0x80);
                        value >>= 7;
                        // byte 4
                        if (value >= 0x80)
                        {
                            data[index++] = (byte)(value | 0x80);
                            value >>= 7;
                            // byte 5
                            if (value >= 0x80)
                            {
                                data[index++] = (byte)(value | 0x80);
                                value >>= 7;
                                // byte 6
                                if (value >= 0x80)
                                {
                                    data[index++] = (byte)(value | 0x80);
                                    value >>= 7;
                                    // byte 7
                                    if (value >= 0x80)
                                    {
                                        data[index++] = (byte)(value | 0x80);
                                        value >>= 7;
                                        // byte 8
                                        if (value >= 0x80)
                                        {
                                            data[index++] = (byte)(value | 0x80);
                                            value >>= 7;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        // last byte
        data[index++] = (byte)value;
        return index;
    }

    public static ushort DecodeVarUInt16(Span<byte> data, ref int index)
    {
        var i = index;
        // byte 0
        uint result = data[i++];
        if (0x80u <= result)
        {
            // byte 1
            uint raw = data[i++];
            result = result & 0x7Fu | (raw & 0x7Fu) << 7;
            if (0x80u <= raw)
            {
                // byte 2
                raw = data[i++];
                result |= raw << 14;
            }
        }
        index = i;
        return (ushort)result;
    }

    public static uint DecodeVarUInt32(Span<byte> data, ref int index)
    {
        var i = index;
        // byte 0
        uint result = data[i++];
        if (0x80u <= result)
        {
            // byte 1
            uint raw = data[i++];
            result = result & 0x7Fu | (raw & 0x7Fu) << 7;
            if (0x80u <= raw)
            {
                // byte 2
                raw = data[i++];
                result |= (raw & 0x7Fu) << 14;
                if (0x80u <= raw)
                {
                    // byte 3
                    raw = data[i++];
                    result |= (raw & 0x7Fu) << 21;
                    if (0x80u <= raw)
                    {
                        // byte 4
                        raw = data[i++];
                        result |= raw << 28;
                    }
                }
            }
        }
        index = i;
        return result;
    }

    public static ulong DecodeVarUInt64(Span<byte> data, ref int index)
    {
        var i = index;
        // byte 0
        ulong result = data[i++];
        if (0x80u <= result)
        {
            // byte 1
            ulong raw = data[i++];
            result = result & 0x7Fu | (raw & 0x7Fu) << 7;
            if (0x80u <= raw)
            {
                // byte 2
                raw = data[i++];
                result |= (raw & 0x7Fu) << 14;
                if (0x80u <= raw)
                {
                    // byte 3
                    raw = data[i++];
                    result |= (raw & 0x7Fu) << 21;
                    if (0x80u <= raw)
                    {
                        // byte 4
                        raw = data[i++];
                        result |= (raw & 0x7Fu) << 28;
                        if (0x80u <= raw)
                        {
                            // byte 5
                            raw = data[i++];
                            result |= (raw & 0x7Fu) << 35;
                            if (0x80u <= raw)
                            {
                                // byte 6
                                raw = data[i++];
                                result |= (raw & 0x7Fu) << 42;
                                if (0x80u <= raw)
                                {
                                    // byte 7
                                    raw = data[i++];
                                    result |= (raw & 0x7Fu) << 49;
                                    if (0x80u <= raw)
                                    {
                                        // byte 8
                                        raw = data[i++];
                                        result |= raw << 56;
                                        if (0x80u <= raw)
                                        {
                                            // byte 9
                                            i++;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        index = i;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort EncodeZigzag16(short value)
    {
        return (ushort)(value << 1 ^ value >> sizeof(short) * 8 - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint EncodeZigzag32(int value)
    {
        return (uint)(value << 1 ^ value >> sizeof(int) * 8 - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong EncodeZigzag64(long value)
    {
        return (ulong)(value << 1 ^ value >> sizeof(long) * 8 - 1);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short DecodeZigzag16(ushort value)
    {
        return (short)(value >> 1 ^ -(value & 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int DecodeZigzag32(uint value)
    {
        return (int)(value >> 1 ^ -(value & 1));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long DecodeZigzag64(ulong value)
    {
        return (long)(value >> 1 ^ (ulong)-(long)(value & 1));
    }
}
