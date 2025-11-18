namespace Kit.Unmanaged.Allocators;

public unsafe struct AlignedNativeAllocator<T>(nuint Alignment = 32): IAllocator<T> where T: unmanaged {
    public T* Allocate(nuint count) => (T*)NativeMemory.AlignedAlloc(Common.ByteCount<T>(count), Alignment);
    public T* Reallocate(T* oldPtr, nuint count) => (T*)NativeMemory.AlignedRealloc(oldPtr, Common.ByteCount<T>(count), Alignment);
    public void Free(T* ptr) => NativeMemory.AlignedFree(ptr);
}

