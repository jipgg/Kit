namespace Precursor.Unmanaged.Internal;

using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static System.Runtime.CompilerServices.MethodImplOptions;
using System.Diagnostics;

public unsafe struct BasicVector<T, TAllocator> : IDisposable
where T : unmanaged where TAllocator : IAllocator<T> {
   static readonly nuint TypeWidth = (nuint)Marshal.SizeOf<T>();
   T* _data = null;
   nuint _size;
   nuint _capacity;
   float _growthFactor;
   nuint ByteCount(nuint count) => TypeWidth * count;
   public ref T this[nuint idx] {
      [MethodImpl(AggressiveInlining)]
      get {
         Debug.Assert(idx < _size);
         return ref _data[idx];
      }
   }
   public nuint Length => _size;
   public nuint Capacity => _capacity;
   public Span<T> AsSpan() => new(_data, (int)_size);
   public PointerEnumerator<T> GetEnumerator() => new(_data, _data + _size);
   public void Add(in T v) {
      Reserve(1);
      _data[_size] = v;
      ++_size;
   }
   public void PopLast() {
      Debug.Assert(_size != 0);
      --_size;
   }
   public void AddRange(ReadOnlySpan<T> span) {
      Reserve((nuint)span.Length);
      var here = new Span<T>(_data + _size, span.Length);
      span.CopyTo(here);
      _size += (nuint)span.Length;
   }
   public void AddRange(params T[] args) => AddRange(args.AsSpan());
   public void AddRange(IEnumerable<T> enumerable, int? count) {
      var size = count ?? enumerable.Count();
      Debug.Assert(size is < 512 and >= 0);
      Span<T> span = stackalloc T[size];
      int idx = 0;
      foreach (var v in enumerable) span[idx++] = v;
      AddRange(span);
   }
   TAllocator _allocator;
   void Allocate(nuint capacity) {
      _data = _allocator.Allocate(capacity);
      Debug.Assert(_data != null);
   }
   void Reallocate(nuint capacity) {
      _data = _allocator.Reallocate(_data, capacity);
      Debug.Assert(_data != null);
   }
   public void Free() {
      if (_data is null) return;
      _allocator.Free(_data);
      _data = null;
      _capacity = 0;
      _size = 0;
   }
   void GrowCapacity(nuint capacity) {
      var newCapacity = nuint.Max(capacity, (nuint)(capacity + capacity * _growthFactor));
      Reallocate(newCapacity);
      _capacity = newCapacity;
   }
   public void Reserve(nuint amount) {
      var capacity = _size + amount;
      if (capacity > _capacity) GrowCapacity(capacity);
   }
   public void ResizeZeroed(nuint newSize) {
      if (newSize > _capacity) GrowCapacity(newSize);
      if (newSize > _size) {
         var span = new Span<T>(_data + _size, (int)(newSize - _size));
         span.Fill(default(T));
      }
      _size = newSize;
   }
   [MethodImpl(AggressiveInlining)]
   public void Resize(nuint newSize) {
      if (newSize > _capacity) GrowCapacity(newSize);
      _size = newSize;
   }

   public void CopyTo(Span<T> span) {
      checked {
         new Span<T>(_data, (int)_size).CopyTo(span);
      }
   }
   public BasicVector<T, TAllocator> Clone() {
      Debug.Assert(_size > 0);
      Debug.Assert(_capacity > 0);
      Debug.Assert(_allocator != null);
      Debug.Assert(_data != null);
      var list = new BasicVector<T, TAllocator>(_capacity, _allocator);
      NativeMemory.Copy(_data, list._data, ByteCount(_size));
      return list;
   }
   public Span<T> Slice(nuint start) => AsSpan().Slice((int)start);
   public Span<T> Slice(nuint start, int length) => AsSpan().Slice((int)start, length);

   public void ShrinkToFit() {
      if (_capacity == _size) return;
      Reallocate(_size);
      _capacity = _size;
   }
   [MethodImpl(AggressiveInlining)]
   public void Clear() {
      NativeMemory.Fill(_data, ByteCount(_size), 0);
      _size = 0;
   }
   public BasicVector(nuint capacity, TAllocator allocator, float growthFactor = 0) {
      Debug.Assert(allocator != null);
      _growthFactor = growthFactor;
      _allocator = allocator;
      Allocate(capacity);
      _capacity = capacity;
   }
   public bool Freed => _data is null;
   public void Dispose() => Free();
}
