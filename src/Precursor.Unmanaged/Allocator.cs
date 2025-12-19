namespace Precursor.Unmanaged;

public unsafe interface IAllocator<T> where T : unmanaged {
   T* Allocate(nuint count);
   T* Reallocate(T* oldPtr, nuint count);
   void Free(T* ptr);
}

public readonly unsafe struct NativeAllocator<T> : IAllocator<T> where T : unmanaged {
   public T* Allocate(nuint count) => (T*)NativeMemory.Alloc(Detail.ByteCount<T>(count));
   public T* Reallocate(T* oldPtr, nuint count) => (T*)NativeMemory.Realloc(oldPtr, Detail.ByteCount<T>(count));
   public void Free(T* ptr) => NativeMemory.Free(ptr);
}
public readonly unsafe struct AlignedNativeAllocator<T>(nuint alignment = 32) : IAllocator<T> where T : unmanaged {
   public T* Allocate(nuint count) => (T*)NativeMemory.AlignedAlloc(Detail.ByteCount<T>(count), alignment);
   public T* Reallocate(T* oldPtr, nuint count) => (T*)NativeMemory.AlignedRealloc(oldPtr, Detail.ByteCount<T>(count), alignment);
   public void Free(T* ptr) => NativeMemory.AlignedFree(ptr);
}
public readonly unsafe struct MarshalAllocator<T>() : IAllocator<T> where T : unmanaged {
   public T* Allocate(nuint count) => (T*)Marshal.AllocHGlobal((int)Detail.ByteCount<T>(count));
   public T* Reallocate(T* oldPtr, nuint count) => (T*)Marshal.ReAllocHGlobal((nint)oldPtr, (nint)Detail.ByteCount<T>(count));
   public void Free(T* ptr) => Marshal.FreeHGlobal((nint)ptr);
}
