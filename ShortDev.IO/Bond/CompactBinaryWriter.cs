using ShortDev.IO.Output;
using ShortDev.IO.ValueStream;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace ShortDev.IO.Bond;

/// <summary>
/// Writer for the Compact Binary tagged protocol
/// </summary>
/// <typeparam name="O">Implementation of IOutputStream interface</typeparam>
/// <param name="buffer">Serialized payload output</param>
/// <param name="version">Protocol version</param>
public ref struct CompactBinaryWriter<TWriter>(ref TWriter writer, ushort version = 1) : IProtocolWriter
    where TWriter : struct, IEndianWriter, allows ref struct
{
    const ushort Magic = (ushort)ProtocolType.COMPACT_PROTOCOL;
    EndianWriter<DelegatingOutputStream<TWriter>> output = new(Endianness.LittleEndian)
    {
        Stream = new(ref writer)
    };
    readonly LinkedList<uint>? lengths = version == 2 ? new() : null;
    readonly Stack<long>? lengthCheck = version == 2 ? new() : null;

    /// <summary>
    /// Write protocol magic number and version
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteVersion()
    {
        output.Write(Magic);
        output.Write(version);
    }

    #region Complex types
    /// <summary>
    /// Start writing a struct
    /// </summary>
    /// <param name="metadata">Schema metadata</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteStructBegin(Metadata? metadata = null)
    {
        if (lengths is null)
            return;

        Debug.Assert(version == 2);
        Debug.Assert(lengths.First is not null);

        uint length = lengths.First.Value;
        lengths.RemoveFirst();

        output.WriteVarUInt32(length);
        // ToDo: PushLengthCheck(checked(output.Position + length));
    }

    /// <summary>
    /// Start writing a base struct
    /// </summary>
    /// <param name="metadata">Base schema metadata</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBaseBegin(Metadata? metadata = null)
    { }

    /// <summary>
    /// End writing a struct
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteStructEnd()
    {
        output.Write((byte)BondDataType.BT_STOP);
        // ToDo: PopLengthCheck(output.Position);
    }

    /// <summary>
    /// End writing a base struct
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBaseEnd()
    {
        output.Write((byte)BondDataType.BT_STOP_BASE);
    }

    /// <summary>
    /// Start writing a field
    /// </summary>
    /// <param name="type">Type of the field</param>
    /// <param name="id">Identifier of the field</param>
    /// <param name="metadata">Metadata of the field</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFieldBegin(BondDataType type, ushort id, Metadata? metadata = null)
    {
        var fieldType = (uint)type;
        if (id <= 5)
        {
            output.Write((byte)(fieldType | ((uint)id << 5)));
        }
        else if (id <= 0xFF)
        {
            output.Write((ushort)(fieldType | (uint)id << 8 | (0x06 << 5)));
        }
        else
        {
            output.Write((byte)(fieldType | (0x07 << 5)));
            output.Write(id);
        }
    }


    /// <summary>
    /// Indicate that field was omitted because it was set to its default value
    /// </summary>
    /// <param name="dataType">Type of the field</param>
    /// <param name="id">Identifier of the field</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFieldOmitted(BondDataType dataType, ushort id, Metadata? metadata = null)
    { }


    /// <summary>
    /// End writing a field
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFieldEnd()
    { }

    /// <summary>
    /// Start writing a list or set container
    /// </summary>
    /// <param name="count">Number of elements in the container</param>
    /// <param name="elementType">Type of the elements</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteContainerBegin(int count, BondDataType elementType)
    {
        if (2 == version && count < 7)
        {
            output.Write((byte)((uint)elementType | (((uint)count + 1) << 5)));
        }
        else
        {
            output.Write((byte)elementType);
            output.WriteVarUInt32((uint)count);
        }
    }

    /// <summary>
    /// Start writing a map container
    /// </summary>
    /// <param name="count">Number of elements in the container</param>
    /// <param name="keyType">Type of the keys</param>
    /// /// <param name="valueType">Type of the values</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteContainerBegin(int count, BondDataType keyType, BondDataType valueType)
    {
        output.Write((byte)keyType);
        output.Write((byte)valueType);
        output.WriteVarUInt32((uint)count);
    }

    /// <summary>
    /// End writing a container
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteContainerEnd()
    { }

    /// <summary>
    /// Write array of bytes verbatim
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBytes(ArraySegment<byte> data)
    {
        output.Write(data);
    }

    /// <summary>
    /// Write array of bytes verbatim
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBytes(scoped ReadOnlySpan<byte> data)
    {
        output.Write(data);
    }

    #endregion

    #region Primitive types
    /// <summary>
    /// Write an UInt8
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt8(byte value)
    {
        output.Write(value);
    }

    /// <summary>
    /// Write an UInt16
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt16(ushort value)
    {
        output.WriteVarUInt16(value);
    }

    /// <summary>
    /// Write an UInt16
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt32(uint value)
    {
        output.WriteVarUInt32(value);
    }

    /// <summary>
    /// Write an UInt64
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUInt64(ulong value)
    {
        output.WriteVarUInt64(value);
    }

    /// <summary>
    /// Write an Int8
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt8(sbyte value)
    {
        output.Write((byte)value);
    }

    /// <summary>
    /// Write an Int16
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt16(short value)
    {
        output.WriteVarUInt16(IntegerHelper.EncodeZigzag16(value));
    }

    /// <summary>
    /// Write an Int32
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt32(int value)
    {
        output.WriteVarUInt32(IntegerHelper.EncodeZigzag32(value));
    }

    /// <summary>
    /// Write an Int64
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteInt64(long value)
    {
        output.WriteVarUInt64(IntegerHelper.EncodeZigzag64(value));
    }

    /// <summary>
    /// Write a float
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteFloat(float value)
    {
        output.Write(value);
    }

    /// <summary>
    /// Write a double
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteDouble(double value)
    {
        output.Write(value);
    }

    /// <summary>
    /// Write a bool
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBool(bool value)
    {
        output.Write((byte)(value ? 1 : 0));
    }

    /// <summary>
    /// Write a UTF-8 string
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteString(string value)
    {
        if (value.Length == 0)
        {
            WriteUInt32(0);
            return;
        }

        var byteSize = Encoding.UTF8.GetByteCount(value);
        WriteUInt32((uint)byteSize);
        WriteString(Encoding.UTF8, value, byteSize);
    }

    /// <summary>
    /// Write a UTF-16 string
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteWString(string value)
    {
        if (value.Length == 0)
        {
            WriteUInt32(0);
            return;
        }

        int byteSize = checked(value.Length * 2);
        WriteUInt32((uint)value.Length);
        WriteString(Encoding.Unicode, value, byteSize);
    }

    private void WriteString(Encoding encoding, string value, int byteSize)
    {
        encoding.GetBytes(value, output.GetSpan(byteSize));
        output.Advance(byteSize);
    }
    #endregion

    #region Length check
    [Conditional("DEBUG")]
    private void PushLengthCheck(long position)
    {
        lengthCheck?.Push(position);
    }

    [Conditional("DEBUG")]
    private void PopLengthCheck(long position)
    {
        if (lengthCheck is null)
            return;

        if (position != lengthCheck.Pop())
        {
            throw new EndOfStreamException("Unexpected end of stream reached.");
        }
    }
    #endregion
}
