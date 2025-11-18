namespace Kit.Unmanaged;

public unsafe interface IAllocator<T> where T: unmanaged {
    T* Allocate(nuint count);
    T* Reallocate(T* oldPtr, nuint count);
    void Free(T* ptr);
}

