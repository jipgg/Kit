namespace Kit.Unmanaged.Allocators;

public unsafe struct MarshalAllocator<T>(): IAllocator<T> where T: unmanaged {
    public T* Allocate(nuint count) => (T*)Marshal.AllocHGlobal((int)Common.ByteCount<T>(count));
    public T* Reallocate(T* oldPtr, nuint count) => (T*)Marshal.ReAllocHGlobal((nint)oldPtr, (nint)Common.ByteCount<T>(count));
    public void Free(T* ptr) => Marshal.FreeHGlobal((nint)ptr);
}
