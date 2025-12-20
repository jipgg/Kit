namespace Precursor.Functional;

using static MethodImplOptions;

public readonly ref struct ValueRef<T> {
   internal readonly ref T _ref;

   [MethodImpl(AggressiveInlining)]
   public ValueRef(ref T r) => _ref = ref r;

   [MethodImpl(AggressiveInlining)]
   public readonly ReadOnlyValueRef<T> AsReadOnly() => new(ref _ref);

   public ref T Ref => ref _ref;

   [MethodImpl(AggressiveInlining)]
   public static implicit operator ReadOnlyValueRef<T>(ValueRef<T> r) => new(ref r._ref);

}

public readonly ref struct ReadOnlyValueRef<T> {
   internal readonly ref readonly T _ref;

   [MethodImpl(AggressiveInlining)]
   public ReadOnlyValueRef(ref readonly T r) => _ref = ref r;

   public ref readonly T Ref => ref _ref;

}
