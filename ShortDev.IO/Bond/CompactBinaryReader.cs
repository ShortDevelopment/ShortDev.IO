// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using ShortDev.IO.Input;
using ShortDev.IO.ValueStream;
using System.Runtime.CompilerServices;
using System.Text;

namespace ShortDev.IO.Bond;

/// <summary>
/// Reader for the Compact Binary tagged protocol
/// </summary>
/// <typeparam name="I">Implementation of IInputStream interface</typeparam>
/// <param name="version2">Protocol version</param>
public unsafe ref struct CompactBinaryReader<TReader>(ref TReader reader, ushort version = 1) where TReader : struct, IEndianReader, allows ref struct
{
    readonly EndianReader<DelegatingInputStream<TReader>> input = new(Endianness.LittleEndian)
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

        if (2 == version && (raw & 0x07 << 5) != 0)
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
        return length == 0 ? string.Empty : input.ReadString(length, Encoding.UTF8);
    }

    /// <summary>
    /// Read a UTF-16 string
    /// </summary>
    /// <exception cref="EndOfStreamException"/>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadWString()
    {
        var length = checked((int)(input.ReadVarUInt32() * 2));
        return length == 0 ? string.Empty : input.ReadString(length, Encoding.Unicode);
    }
    #endregion
}

