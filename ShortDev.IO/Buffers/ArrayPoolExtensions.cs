using System.Buffers;

namespace ShortDev.IO.Buffers;

public static class ArrayPoolExtensions
{
    public static PooledMemory<T> RentMemory<T>(this ArrayPool<T> pool, int length)
        => PooledMemory<T>.Rent(pool, length);
}
