// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ShortDev.IO.Input;
using ShortDev.IO.ValueStream;
using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace ShortDev.IO.Bond;

/// <summary>
/// Reader for the Compact Binary tagged protocol
/// </summary>
/// <typeparam name="I">Implementation of IInputStream interface</typeparam>
/// <param name="version2">Protocol version</param>
public unsafe ref struct CompactBinaryReader<TReader>(ref TReader reader, ushort version = 1) : ITaggedProtocolReader
    where TReader : struct, IEndianReader, allows ref struct
{
    EndianReader<DelegatingInputStream<TReader>> input = new(Endianness.LittleEndian)
    {
        Stream = new(ref reader)
    };

    #region Complex types

    /// <summary>
    /// Start reading a struct
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadStructBegin()
    {
        if (2 == version)
        {
            input.ReadVarUInt32();
        }
    }

    /// <summary>
    /// Start reading a base of a struct
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadBaseBegin()
    { }

    /// <summary>
    /// End reading a struct
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadStructEnd()
    { }

    /// <summary>
    /// End reading a base of a struct
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadBaseEnd()
    { }

    /// <summary>
    /// Start reading a field
    /// </summary>
    /// <param name="type">An out parameter set to the field type
    /// or BT_STOP/BT_STOP_BASE if there is no more fields in current struct/base</param>
    /// <param name="id">Out parameter set to the field identifier</param>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadFieldBegin(out BondDataType type, out ushort id)
    {
        uint raw = input.ReadUInt8();

        type = (BondDataType)(raw & 0x1f);
        raw >>= 5;

        if (raw < 6)
        {
            id = (ushort)raw;
        }
        else if (raw == 6)
        {
            id = input.ReadUInt8();
        }
        else
        {
            id = input.ReadUInt16();
        }
    }

    /// <summary>
    /// End reading a field
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadFieldEnd()
    { }

    /// <summary>
    /// Start reading a list or set container
    /// </summary>
    /// <param name="count">An out parameter set to number of items in the container</param>
    /// <param name="elementType">An out parameter set to type of container elements</param>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadContainerBegin(out int count, out BondDataType elementType)
    {
        var raw = input.ReadUInt8();
        elementType = (BondDataType)(raw & 0x1f);

        if (2 == version && (raw & (0x07 << 5)) != 0)
            count = (raw >> 5) - 1;
        else
            count = checked((int)input.ReadVarUInt32());
    }

    /// <summary>
    /// Start reading a map container
    /// </summary>
    /// <param name="count">An out parameter set to number of items in the container</param>
    /// <param name="keyType">An out parameter set to the type of map keys</param>
    /// <param name="valueType">An out parameter set to the type of map values</param>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadContainerBegin(out int count, out BondDataType keyType, out BondDataType valueType)
    {
        keyType = (BondDataType)input.ReadUInt8();
        valueType = (BondDataType)input.ReadUInt8();
        count = checked((int)input.ReadVarUInt32());
    }

    /// <summary>
    /// End reading a container
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadContainerEnd()
    { }

    #endregion

    #region Primitive types

    /// <summary>
    /// Read an UInt8
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte ReadUInt8()
    {
        return input.ReadUInt8();
    }

    /// <summary>
    /// Read an UInt16
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ushort ReadUInt16()
    {
        return input.ReadVarUInt16();
    }

    /// <summary>
    /// Read an UInt32
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public uint ReadUInt32()
    {
        return input.ReadVarUInt32();
    }

    /// <summary>
    /// Read an UInt64
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong ReadUInt64()
    {
        return input.ReadVarUInt64();
    }

    /// <summary>
    /// Read an Int8
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public sbyte ReadInt8()
    {
        return (sbyte)input.ReadUInt8();
    }

    /// <summary>
    /// Read an Int16
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public short ReadInt16()
    {
        return IntegerHelper.DecodeZigzag16(input.ReadVarUInt16());
    }

    /// <summary>
    /// Read an Int32
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int ReadInt32()
    {
        return IntegerHelper.DecodeZigzag32(input.ReadVarUInt32());
    }

    /// <summary>
    /// Read an Int64
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long ReadInt64()
    {
        return IntegerHelper.DecodeZigzag64(input.ReadVarUInt64());
    }

    /// <summary>
    /// Read a bool
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool ReadBool()
    {
        return input.ReadUInt8() != 0;
    }

    /// <summary>
    /// Read a float
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public float ReadFloat()
    {
        return input.ReadSingle();
    }

    /// <summary>
    /// Read a double
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public double ReadDouble()
    {
        return input.ReadDouble();
    }

    /// <summary>
    /// Read a UTF-8 string
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString()
    {
        var length = checked((int)input.ReadVarUInt32());
        return length == 0 ? string.Empty : ReadString(Encoding.UTF8, length);
    }

    /// <summary>
    /// Read a UTF-16 string
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadWString()
    {
        var length = checked((int)(input.ReadVarUInt32() * 2));
        return length == 0 ? string.Empty : ReadString(Encoding.Unicode, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private string ReadString(Encoding encoding, int length)
    {
        var rentedBuffer = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            var buffer = rentedBuffer.AsSpan(start: 0, length);
            input.ReadBytes(buffer);
            return encoding.GetString(buffer);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rentedBuffer);
        }
    }

    /// <summary>
    /// Read an array of bytes verbatim
    /// </summary>
    /// <param name="count">Number of bytes to read</param>
    /// <exception cref="EndOfStreamException"/>
    [Obsolete("Use ReadBytes(scoped Span<byte> buffer) instead")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArraySegment<byte> ReadBytes(int count)
    {
        byte[] buffer = new byte[count];
        input.ReadBytes(buffer);
        return buffer;
    }

    /// <summary>
    /// Read an array of bytes verbatim
    /// </summary>
    /// <param name="count">Number of bytes to read</param>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void ReadBytes(scoped Span<byte> buffer)
    {
        input.ReadBytes(buffer);
    }
    #endregion

    #region Skip
    /// <summary>
    /// Skip a value of specified type
    /// </summary>
    /// <param name="type">Type of the value to skip</param>
    /// <exception cref="EndOfStreamException"/>
    public void Skip(BondDataType type)
    {
        switch (type)
        {
            case (BondDataType.BT_BOOL):
            case (BondDataType.BT_UINT8):
            case (BondDataType.BT_INT8):
                input.SkipBytes(sizeof(byte));
                break;
            case (BondDataType.BT_UINT16):
            case (BondDataType.BT_INT16):
                input.ReadVarUInt16();
                break;
            case (BondDataType.BT_UINT32):
            case (BondDataType.BT_INT32):
                input.ReadVarUInt32();
                break;
            case (BondDataType.BT_FLOAT):
                input.SkipBytes(sizeof(float));
                break;
            case (BondDataType.BT_DOUBLE):
                input.SkipBytes(sizeof(double));
                break;
            case (BondDataType.BT_UINT64):
            case (BondDataType.BT_INT64):
                input.ReadVarUInt64();
                break;
            case (BondDataType.BT_STRING):
                input.SkipBytes(checked((int)input.ReadVarUInt32()));
                break;
            case (BondDataType.BT_WSTRING):
                input.SkipBytes(checked((int)(input.ReadVarUInt32() * 2)));
                break;
            case BondDataType.BT_LIST:
            case BondDataType.BT_SET:
                SkipContainer();
                break;
            case BondDataType.BT_MAP:
                SkipMap();
                break;
            case BondDataType.BT_STRUCT:
                SkipStruct();
                break;
            default:
                throw new InvalidDataException($"Invalid BondDataType {type}");
        }
    }


    void SkipContainer()
    {
        ReadContainerBegin(out int count, out BondDataType elementType);

        if (elementType == BondDataType.BT_UINT8 || elementType == BondDataType.BT_INT8)
        {
            input.SkipBytes(count);
        }
        else if (elementType == BondDataType.BT_FLOAT)
        {
            input.SkipBytes(checked(count * sizeof(float)));
        }
        else if (elementType == BondDataType.BT_DOUBLE)
        {
            input.SkipBytes(checked(count * sizeof(double)));
        }
        else
        {
            int depth = MaxDepthChecker.ValidateDepthForIncrement();
            try
            {
                MaxDepthChecker.SetDepth(depth + 1);

                while (0 <= --count)
                {
                    Skip(elementType);
                }
            }
            finally
            {
                MaxDepthChecker.SetDepth(depth);
            }
        }
    }

    void SkipMap()
    {
        int depth = MaxDepthChecker.ValidateDepthForIncrement();
        try
        {
            MaxDepthChecker.SetDepth(depth + 1);

            ReadContainerBegin(out int count, out BondDataType keyType, out BondDataType valueType);
            while (0 <= --count)
            {
                Skip(keyType);
                Skip(valueType);
            }
        }
        finally
        {
            MaxDepthChecker.SetDepth(depth);
        }
    }

    void SkipStruct()
    {
        if (2 == version)
        {
            input.SkipBytes(checked((int)input.ReadVarUInt32()));
        }
        else
        {
            int depth = MaxDepthChecker.ValidateDepthForIncrement();
            try
            {
                MaxDepthChecker.SetDepth(depth + 1);

                while (true)
                {
                    ReadFieldBegin(out BondDataType type, out _);

                    if (type == BondDataType.BT_STOP_BASE) continue;
                    if (type == BondDataType.BT_STOP) break;

                    Skip(type);
                }
            }
            finally
            {
                MaxDepthChecker.SetDepth(depth);
            }
        }
    }
    #endregion
}

