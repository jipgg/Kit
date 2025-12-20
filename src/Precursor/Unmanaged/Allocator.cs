using System.Runtime.InteropServices;
namespace Precursor.Unmanaged;

public unsafe interface IAllocator<T> where T : unmanaged {
   T* Allocate(nuint count);
   T* Reallocate(T* oldPtr, nuint count);
   void Free(T* ptr);
}
public readonly unsafe struct NativeAllocator<T> : IAllocator<T> where T : unmanaged {
   public T* Allocate(nuint count) => (T*)NativeMemory.Alloc(LocalHelpers.ByteCount<T>(count));
   public T* Reallocate(T* oldPtr, nuint count) => (T*)NativeMemory.Realloc(oldPtr, LocalHelpers.ByteCount<T>(count));
   public void Free(T* ptr) => NativeMemory.Free(ptr);
}
public readonly unsafe struct AlignedNativeAllocator<T>(nuint alignment = 32) : IAllocator<T> where T : unmanaged {
   public T* Allocate(nuint count) => (T*)NativeMemory.AlignedAlloc(LocalHelpers.ByteCount<T>(count), alignment);
   public T* Reallocate(T* oldPtr, nuint count) => (T*)NativeMemory.AlignedRealloc(oldPtr, LocalHelpers.ByteCount<T>(count), alignment);
   public void Free(T* ptr) => NativeMemory.AlignedFree(ptr);
}
public readonly unsafe struct MarshalAllocator<T>() : IAllocator<T> where T : unmanaged {
   public T* Allocate(nuint count) => (T*)Marshal.AllocHGlobal((int)LocalHelpers.ByteCount<T>(count));
   public T* Reallocate(T* oldPtr, nuint count) => (T*)Marshal.ReAllocHGlobal((nint)oldPtr, (nint)LocalHelpers.ByteCount<T>(count));
   public void Free(T* ptr) => Marshal.FreeHGlobal((nint)ptr);
}

file static class LocalHelpers {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static public nuint ByteCount<T>(nuint count) where T : unmanaged => (nuint)Unsafe.SizeOf<T>() * count;
}

