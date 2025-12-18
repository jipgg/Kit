namespace Kit;

using static MethodImplOptions;

public readonly ref struct WrappedRef<T> {
   internal readonly ref T _ref;

   [MethodImpl(AggressiveInlining)]
   public WrappedRef(ref T r) => _ref = ref r;

   [MethodImpl(AggressiveInlining)]
   public readonly ReadOnlyWrappedRef<T> AsReadOnly() => new(ref _ref);

   [MethodImpl(AggressiveInlining)]
   public ref T Unwrap() => ref _ref;

   [MethodImpl(AggressiveInlining)]
   public static implicit operator ReadOnlyWrappedRef<T>(WrappedRef<T> r) => new(ref r._ref);

}

public readonly ref struct ReadOnlyWrappedRef<T> {
   internal readonly ref readonly T _ref;

   [MethodImpl(AggressiveInlining)]
   public ReadOnlyWrappedRef(ref readonly T r) => _ref = ref r;

   [MethodImpl(AggressiveInlining)]
   public ref readonly T Unwrap() => ref _ref;

}
