using System;
using System.Buffers;
using System.Text;

namespace ShortDev.IO.Output;

public interface IWriter : IBufferWriter<byte>, IDisposable
{
    void Write<T>(T value) where T : IBinaryWritable;

    void Write(scoped ReadOnlySpan<byte> buffer);

    void Write(sbyte value);
    void Write(byte value);

    void Write(short value);
    void Write(ushort value);

    void Write(int value);
    void Write(uint value);

    void Write(long value);
    void Write(ulong value);

    void Write(Half value);
    void Write(float value);
    void Write(double value);

    void Write(Guid value);

    void WriteWithLength(string value, Encoding? encoding = null);
    void WriteWithLength(scoped ReadOnlySpan<byte> value);
}
