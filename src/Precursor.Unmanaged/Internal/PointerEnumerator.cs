namespace Precursor.Unmanaged.Internal;

public unsafe ref struct PointerEnumerator<T> where T : unmanaged {
   readonly T* _begin;
   readonly T* _end;
   T* _current;
   public PointerEnumerator(T* begin, T* end) {
      Debug.Assert(begin != null);
      Debug.Assert(end != null);
      _begin = begin;
      _end = end;
      _current = StartPosition;
   }
   T* StartPosition => _begin - 1;
   public ref T Current => ref *_current;
   [MethodImpl(AggressiveInlining)]
   public bool MoveNext() => ++_current != _end;
   public void Reset() => _current = StartPosition;
}
