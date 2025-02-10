using System;
using System.Text;

namespace ShortDev.IO.Input;

public interface IReader : IDisposable
{
    T Read<T>() where T : IBinaryParsable<T>;

    void ReadBytes(scoped Span<byte> buffer);

    sbyte ReadInt8();
    byte ReadUInt8();

    short ReadInt16();
    ushort ReadUInt16();

    int ReadInt32();
    uint ReadUInt32();

    long ReadInt64();
    ulong ReadUInt64();

    Half ReadHalf();
    float ReadSingle();
    double ReadDouble();

    Guid ReadGuid();
    string ReadString(int byteSize, Encoding? encoding = null);
    string ReadStringWithLength(Encoding? encoding = null);

    bool TryReadSlice(int length, out ReadOnlySpan<byte> slice);
}
