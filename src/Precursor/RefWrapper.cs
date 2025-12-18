namespace Precursor;

using static MethodImplOptions;

public readonly ref struct RefWrapper<T> {
   internal readonly ref T _ref;

   [MethodImpl(AggressiveInlining)]
   public RefWrapper(ref T r) => _ref = ref r;

   [MethodImpl(AggressiveInlining)]
   public readonly ReadOnlyRefWrapper<T> AsReadOnly() => new(ref _ref);

   [MethodImpl(AggressiveInlining)]
   public ref T Unwrap() => ref _ref;

   [MethodImpl(AggressiveInlining)]
   public static implicit operator ReadOnlyRefWrapper<T>(RefWrapper<T> r) => new(ref r._ref);

}

public readonly ref struct ReadOnlyRefWrapper<T> {
   internal readonly ref readonly T _ref;

   [MethodImpl(AggressiveInlining)]
   public ReadOnlyRefWrapper(ref readonly T r) => _ref = ref r;

   [MethodImpl(AggressiveInlining)]
   public ref readonly T Unwrap() => ref _ref;

}
