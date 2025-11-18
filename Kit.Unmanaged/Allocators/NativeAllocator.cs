namespace Kit.Unmanaged.Allocators;

public unsafe readonly struct NativeAllocator<T>: IAllocator<T> where T: unmanaged {
    public T* Allocate(nuint count) => (T*)NativeMemory.Alloc(Common.ByteCount<T>(count));
    public T* Reallocate(T* oldPtr, nuint count) => (T*)NativeMemory.Realloc(oldPtr, Common.ByteCount<T>(count));
    public void Free(T* ptr) => NativeMemory.Free(ptr);
}
