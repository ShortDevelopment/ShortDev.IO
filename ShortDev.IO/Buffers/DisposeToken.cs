using System;
using System.Buffers;
using System.Diagnostics;

namespace ShortDev.IO.Buffers;

public readonly struct DisposeToken<T>(ArrayPool<T>? pool, T[]? array) : IDisposable
{
    private readonly ArrayPool<T>? _pool = pool;
    private readonly T[]? _array = array;

    public void Dispose() => _pool?.Return(_array ?? throw new UnreachableException("_array was null"));
}
