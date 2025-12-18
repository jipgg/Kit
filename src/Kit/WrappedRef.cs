namespace Kit;

using static MethodImplOptions;

public readonly ref struct Ref<T> {
   internal readonly ref T _value;

   [MethodImpl(AggressiveInlining)]
   public Ref(ref T r) => _value = ref r;

   [MethodImpl(AggressiveInlining)]
   public readonly ReadOnlyRef<T> AsReadOnly() => new(ref _value);

   public ref T Target {
      [MethodImpl(AggressiveInlining)]
      get => ref _value;
   }

   [MethodImpl(AggressiveInlining)]
   public static implicit operator ReadOnlyRef<T>(Ref<T> r) => new(ref r._value);

   [MethodImpl(AggressiveInlining)]
   public static implicit operator T(in Ref<T> r) => r._value;
}

public readonly ref struct ReadOnlyRef<T> {
   internal readonly ref readonly T _value;

   [MethodImpl(AggressiveInlining)]
   public ReadOnlyRef(ref readonly T r) => _value = ref r;

   [MethodImpl(AggressiveInlining)]
   public static implicit operator T(in ReadOnlyRef<T> r) => r._value;

   public ref readonly T Value {
      [MethodImpl(AggressiveInlining)]
      get => ref _value;
   }
}
