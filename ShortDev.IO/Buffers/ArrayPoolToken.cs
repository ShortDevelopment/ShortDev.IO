using System;
using System.Buffers;

namespace ShortDev.IO.Buffers;

public readonly struct ArrayPoolToken<T> : IDisposable
{
    readonly ArrayPool<T> _pool;
    readonly T[] _array;
    readonly int _length;
    private ArrayPoolToken(ArrayPool<T> pool, T[] array, int length)
    {
        _pool = pool;
        _array = array;
        _length = length;
    }

    public Span<T> Span
        => _array.AsSpan()[0.._length];
    public Memory<T> Memory
        => _array.AsMemory()[0.._length];

    public void Dispose()
        => _pool.Return(_array);

    public static ArrayPoolToken<T> Rent(ArrayPool<T> pool, int length)
        => new(pool, pool.Rent(length), length);
}
