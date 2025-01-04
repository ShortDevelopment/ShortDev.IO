using System;
using System.Buffers;
using System.Diagnostics;

namespace ShortDev.IO.Buffers;

public readonly struct PooledMemory<T> : IDisposable
{
    private readonly ArrayPool<T>? _pool;
    private readonly Range _range;
    private readonly T[]? _array;

    private PooledMemory(ArrayPool<T>? pool, T[]? array, Range range)
    {
        _pool = pool;
        _array = array;
        _range = range;
    }

    public readonly Span<T> Span => _array.AsSpan()[_range];

    public readonly int Length
    {
        get
        {
            if (_array is null)
                return 0;

            var (_, length) = _range.GetOffsetAndLength(_array.Length);
            return length;
        }
    }

    public readonly PooledMemory<T> this[Range range]
    {
        get
        {
            if (_array is null)
                return default;

            var (oldStart, oldLength) = _range.GetOffsetAndLength(_array.Length);
            var (start, length) = range.GetOffsetAndLength(oldLength);

            var newStart = oldStart + start;
            return new(_pool, _array, new Range(newStart, newStart + length));
        }
    }

    public void Dispose() => _pool?.Return(_array ?? throw new UnreachableException("_array was null"));

    public static PooledMemory<T> Rent(ArrayPool<T> pool, int length)
    {
        var array = pool.Rent(length);
        return new(pool, array, 0..length);
    }

    public static implicit operator DisposeToken<T>(PooledMemory<T> memory) => new(memory._pool, memory._array);
}
