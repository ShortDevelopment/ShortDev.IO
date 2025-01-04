using System;
using System.IO;
using System.Text;

namespace ShortDev.IO.Output;

public interface IWriter : IDisposable
{
    void Write(ReadOnlySpan<byte> buffer);

    void Write(sbyte value);
    void Write(byte value);

    void Write(short value);
    void Write(ushort value);

    void Write(int value);
    void Write(uint value);

    void Write(long value);
    void Write(ulong value);

    void Write(float value);
    void Write(double value);

    void WriteWithLength(string value, Encoding? encoding = null);
    void WriteWithLength(ReadOnlySpan<byte> value);

    void CopyTo(Stream destination);
    void Clear();
}
