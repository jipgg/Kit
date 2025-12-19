using System.Diagnostics;
namespace Precursor;

public interface ISmallBuffer<Self, T> where Self : ISmallBuffer<Self, T> {
   static abstract int Length { get; }
   static abstract Span<T> AsSpan(ref Self self);
   static abstract ReadOnlySpan<T> AsReadOnlySpan(ref readonly Self self);
}


[InlineArray(LengthAsConst)]
public struct DefaultSmallBuffer<T> : ISmallBuffer<DefaultSmallBuffer<T>, T> {
   T _data;
   const int LengthAsConst = 10;
   public static int Length => LengthAsConst;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static Span<T> AsSpan(ref DefaultSmallBuffer<T> b) => b;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static ReadOnlySpan<T> AsReadOnlySpan(ref readonly DefaultSmallBuffer<T> b) => b;
}

public struct ValueList<T, SmallBuffer>
where SmallBuffer : struct, ISmallBuffer<SmallBuffer, T> where T : IEquatable<T> {
   SmallBuffer _buffer;

   List<T>? _list;
   int _count;

   public readonly int Count => IsBufferStored ? _count : _list.Count;

   [Conditional("DEBUG")]
   readonly void AssertInvariant() {
      if (_list is null)
         Debug.Assert(_count >= 0 && _count <= SmallBuffer.Length);
      else
         Debug.Assert(_count <= SmallBuffer.Length);
   }
   [MemberNotNull(nameof(_list))]
   public void MigrateToList() {
      Debug.Assert(IsBufferStored);
      _list = new List<T>(SmallBuffer.Length * SmallBuffer.Length);
      _list.AddRange(SmallBuffer.AsReadOnlySpan(ref _buffer)[.._count]);
   }

   [MemberNotNullWhen(false, nameof(_list))]
   public readonly bool IsBufferStored
      => _list is null;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public ValueList() {
      _count = 0;
      _buffer = default;
      _list = null;
   }
   public void Add(in T item) {
      AssertInvariant();
      if (!IsBufferStored) {
         _list.Add(item);
         return;
      }
      var buf = SmallBuffer.AsSpan(ref _buffer);
      if (_count + 1 <= SmallBuffer.Length) {
         buf[_count++] = item;
         return;
      }
      if (_list is null) MigrateToList();
      _list.Add(item);
   }
   public T this[int i] {
      get {
         if (IsBufferStored) return SmallBuffer.AsReadOnlySpan(ref _buffer)[i];
         else return _list[i];
      }
      set {
         if (IsBufferStored) SmallBuffer.AsSpan(ref _buffer)[i] = value;
         else _list[i] = value;
      }
   }
   public readonly int IndexOf(in T item) {
      AssertInvariant();
      if (!IsBufferStored) return _list.IndexOf(item);

      var buf = SmallBuffer.AsReadOnlySpan(in _buffer);
      return buf[.._count].IndexOf(item);
   }
   public void Insert(int index, in T value) {
      AssertInvariant();
      Debug.Assert(index >= 0);
      if (!IsBufferStored) {
         _list.Insert(index, value);
         return;
      }
      if (_count == SmallBuffer.Length) {
         MigrateToList();
         //_list = [.. SmallBuffer.AsReadOnlySpan(ref _buffer)];
         _list!.Insert(index, value);
         return;
      }
      var span = SmallBuffer.AsSpan(ref _buffer);
      for (int i = _count; i > index; --i) {
         span[i] = span[i - 1];
      }
      span[index] = value;
      ++_count;
   }
   public void RemoveAt(int index) {
      AssertInvariant();
      Debug.Assert(index >= 0 && index < Count);
      if (!IsBufferStored) {
         _list.RemoveAt(index);
         return;
      }
      var span = SmallBuffer.AsSpan(ref _buffer);
      for (int i = index; i < _count - 1; ++i) {
         span[i] = span[i + 1];
      }

      --_count;
   }
   public void Clear() {
      AssertInvariant();
      SmallBuffer.AsSpan(ref _buffer).Clear();
      if (!IsBufferStored) _list.Clear();
      _count = 0;
   }
   public List<T> AsList() {
      if (IsBufferStored) MigrateToList();
      return _list;
   }
   //public List<T> AsList() => IsBufferStored ? [.. SmallBuffer.AsSpan(ref _buffer)[.._count]] : _list;

   public bool Contains(T value) => IndexOf(value) is not -1;

   public bool Remove(T item) {
      AssertInvariant();
      var index = IndexOf(item);
      if (index is -1) return false;
      RemoveAt(index);
      return true;
   }
   public ref struct Enumerator(ref readonly ValueList<T, SmallBuffer> v) {
      readonly ref readonly ValueList<T, SmallBuffer> _valueList = ref v;
      int _index = -1;
      public readonly T Current => _valueList[_index];
      public bool MoveNext() => ++_index < _valueList.Count;
   }
   [UnscopedRef]
   public Enumerator GetEnumerator() => new(ref this);
}
