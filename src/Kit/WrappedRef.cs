namespace Kit;

using static MethodImplOptions;

public readonly ref struct Ref<T> {
   internal readonly ref T _target;

   [MethodImpl(AggressiveInlining)]
   public Ref(ref T r) => _target = ref r;

   [MethodImpl(AggressiveInlining)]
   public readonly ReadOnlyRef<T> AsReadOnly() => new(ref _target);

   [MethodImpl(AggressiveInlining)]
   public ref T Get() => ref _target;

   public ref T Target {
      [MethodImpl(AggressiveInlining)]
      get => ref _target;
   }

   [MethodImpl(AggressiveInlining)]
   public static implicit operator ReadOnlyRef<T>(Ref<T> r) => new(ref r._target);

   [MethodImpl(AggressiveInlining)]
   public static implicit operator T(in Ref<T> r) => r._target;
}

public readonly ref struct ReadOnlyRef<T> {
   internal readonly ref readonly T _target;

   [MethodImpl(AggressiveInlining)]
   public ReadOnlyRef(ref readonly T r) => _target = ref r;

   [MethodImpl(AggressiveInlining)]
   public static implicit operator T(in ReadOnlyRef<T> r) => r._target;

   [MethodImpl(AggressiveInlining)]
   public ref readonly T Get() => ref _target;

   public ref readonly T Target {
      [MethodImpl(AggressiveInlining)]
      get => ref _target;
   }
}
