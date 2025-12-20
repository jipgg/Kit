using System.Diagnostics;
namespace Precursor.Unmanaged;

public unsafe ref struct PointerEnumerator<T> where T : unmanaged {
   internal readonly T* _begin;
   internal readonly T* _end;
   internal T* _current;
   public PointerEnumerator(T* begin, T* end) {
      Debug.Assert(begin is not null && end is not null && begin < end);
      _begin = begin;
      _end = end;
      _current = StartPosition;
   }
   T* StartPosition => _begin - 1;
   public ref T Current => ref *_current;
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public bool MoveNext() => ++_current != _end;
   public void Reset() => _current = StartPosition;
}
