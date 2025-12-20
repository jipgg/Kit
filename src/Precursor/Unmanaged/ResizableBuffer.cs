using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Precursor.Unmanaged;

using static MethodImplOptions;

public unsafe struct DynamicBuffer<T, Allocator> : IDisposable
where T : unmanaged
where Allocator : notnull, IAllocator<T> {
   public static readonly nuint SizeOfT = (nuint)Unsafe.SizeOf<T>();
   T* _data = null;
   readonly float _growthFactor;
   readonly Allocator _allocator;
   public nuint Size { get; private set; }
   public nuint Capacity { get; private set; }
   public readonly nuint ByteCount {
      [MethodImpl(AggressiveInlining)]
      get => GetByteCount(Size);
   }
   public readonly T* Data {
      [MethodImpl(AggressiveInlining)]
      get => _data;
   }

   [MethodImpl(AggressiveInlining)]
   readonly nuint GetByteCount(nuint count) => SizeOfT * count;

   public readonly ref T this[nuint idx] {
      [MethodImpl(AggressiveInlining)]
      get {
         Debug.Assert(idx < Size);
         return ref _data[idx];
      }
   }
   public readonly Span<T> AsSpan() => new(_data, (int)Size);
   public readonly PointerEnumerator<T> GetEnumerator() => new(_data, _data + Size);
   public void Add(in T v) {
      Reserve(1);
      _data[Size] = v;
      ++Size;
   }
   public void AddRange(params ReadOnlySpan<T> span) {
      Reserve((nuint)span.Length);
      var here = new Span<T>(_data + Size, span.Length);
      span.CopyTo(here);
      Size += (nuint)span.Length;
   }
   public void AddRange(IEnumerable<T> enumerable, int? count = null) {
      var size = count ?? enumerable.Count();
      Debug.Assert(size is < 512 and >= 0);
      Span<T> span = stackalloc T[size];
      int idx = 0;
      foreach (var v in enumerable) span[idx++] = v;
      AddRange(span);
   }
   public void RemoveLast() {
      Debug.Assert(Size is not 0);
      --Size;
   }
   void Allocate(nuint capacity) {
      _data = _allocator.Allocate(capacity);
      Debug.Assert(_data is not null);
   }
   void Reallocate(nuint capacity) {
      _data = _allocator.Reallocate(_data, capacity);
      Debug.Assert(_data is not null);
   }
   void Free() {
      if (IsFreed) return;
      _allocator.Free(_data);
      _data = null;
      Capacity = 0;
      Size = 0;
   }
   void GrowCapacity(nuint capacity) {
      var newCapacity = nuint.Max(capacity, (nuint)(capacity + capacity * _growthFactor));
      Reallocate(newCapacity);
      Capacity = newCapacity;
   }
   public void Reserve(nuint amount) {
      var capacity = Size + amount;
      if (capacity > Capacity) GrowCapacity(capacity);
   }
   public void ResizeZeroed(nuint newSize) {
      if (newSize > Capacity) GrowCapacity(newSize);
      if (newSize > Size) {
         var span = new Span<T>(_data + Size, (int)(newSize - Size));
         span.Fill(default(T));
      }
      Size = newSize;
   }
   public void Resize(nuint newSize) {
      if (newSize > Capacity) GrowCapacity(newSize);
      Size = newSize;
   }

   [MethodImpl(AggressiveInlining)]
   public readonly void CopyTo(Span<T> span) {
      checked {
         new Span<T>(_data, (int)Size).CopyTo(span);
      }
   }
   public DynamicBuffer<T, Allocator> Clone() {
      Debug.Assert(Size > 0 && Capacity > 0 && _allocator is not null && _data is not null);
      var list = new DynamicBuffer<T, Allocator>(Capacity, _allocator);
      NativeMemory.Copy(_data, list._data, GetByteCount(Size));
      return list;
   }
   [MethodImpl(AggressiveInlining)]
   public Span<T> Slice(int start) => AsSpan().Slice(start);
   [MethodImpl(AggressiveInlining)]
   public Span<T> Slice(nuint start) => AsSpan().Slice((int)start);
   [MethodImpl(AggressiveInlining)]
   public Span<T> Slice(nuint start, int length) => AsSpan().Slice((int)start, length);
   [MethodImpl(AggressiveInlining)]
   public Span<T> Slice(int start, int length) => AsSpan().Slice(start, length);

   public void ShrinkToFit() {
      if (Capacity == Size) return;
      Reallocate(Size);
      Capacity = Size;
   }
   [MethodImpl(AggressiveInlining)]
   public void Clear() {
      if (_data is null) return;
      NativeMemory.Fill(_data, GetByteCount(Size), 0);
      Size = 0;
   }
   public DynamicBuffer(nuint capacity, Allocator allocator, float growthFactor = 2) {
      Debug.Assert(allocator is not null);
      _growthFactor = growthFactor;
      _allocator = allocator;
      Allocate(capacity);
      Capacity = capacity;
   }
   public readonly bool IsFreed => _data is null;
   public void Dispose() => Free();
}
