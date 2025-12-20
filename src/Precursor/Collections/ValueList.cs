using System.Diagnostics;
using Precursor.Storage;
namespace Precursor.Collections;

using static MethodImplOptions;

public struct ValueList<T, SmallBuffer>
where SmallBuffer : struct, ISmallBuffer<SmallBuffer, T> where T : IEquatable<T> {
   internal SmallBuffer _buffer;

   internal List<T>? _list;
   internal int _count;

   public readonly int Count => IsBufferStored ? _count : _list.Count;

   [Conditional("DEBUG")]
   internal readonly void AssertInvariant() {
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

   [MethodImpl(AggressiveInlining)]
   public ValueList() {
      _count = 0;
      _buffer = default;
      _list = null;
   }
   public void AddRange(params ReadOnlySpan<T> source) {
      AssertInvariant();
      if (!IsBufferStored) {
         _list.AddRange(source);
         return;
      }
      if (source.Length + _count <= SmallBuffer.Length) {
         var destination = SmallBuffer.AsSpan(ref _buffer).Slice(_count, source.Length);
         Debug.Assert(destination.Length == source.Length);
         source.CopyTo(destination);
         _count += source.Length;
         return;
      }
      if (_list is null) MigrateToList();
      _list.AddRange(source);
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
         _list.Insert(index, value);
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

   public bool Contains(in T item) => IndexOf(item) is not -1;

   public bool Remove(in T item) {
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

public struct ValueList<T> where T : IEquatable<T> {
   ValueList<T, SmallBuffer8<T>> _impl;
   public readonly int Count => _impl.Count;
   public readonly bool IsBufferStored => _impl.IsBufferStored;
   public ValueList() => _impl = new();
   public void Add(in T item) => _impl.Add(in item);
   public void AddRange(params ReadOnlySpan<T> source) => _impl.AddRange(source);

   public T this[int i] {
      get => _impl[i];
      set => _impl[i] = value;
   }
   public readonly int IndexOf(in T item) => _impl.IndexOf(in item);
   public void Insert(int index, in T item) => _impl.Insert(index, item);
   public void RemoveAt(int index) => _impl.RemoveAt(index);
   public void Clear() => _impl.Clear();
   public List<T> AsList() => _impl.AsList();
   public bool Contains(in T item) => _impl.Contains(item);
   public bool Remove(in T item) => _impl.Remove(item);
   [UnscopedRef]
   public ValueList<T, SmallBuffer8<T>>.Enumerator GetEnumerator() => _impl.GetEnumerator();
}
