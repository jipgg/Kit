namespace Kit;

public readonly struct Maybe<T> {
   public readonly T? Value;
   [MemberNotNullWhen(true, nameof(Value))]
   public bool HasValue { get; }
   [MemberNotNullWhen(false, nameof(Value))]
   public bool IsNil {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => !HasValue;
   }
   public Maybe() {
      Value = default!;
      HasValue = false;
   }
   public Maybe(in T value) {
      Value = value;
      HasValue = true;
   }
   public static implicit operator Maybe<T>(in T v) => new(v);
   public bool Try([NotNullWhen(true)] out T? value) {
      if (HasValue) {
         value = Value!;
         return true;
      }
      value = default;
      return false;
   }
   public static explicit operator T(in Maybe<T> n) => n.HasValue ? n.Value! : throw new NilValueException();
}

public readonly ref struct RefOption<T>
where T : allows ref struct {
   public readonly T Value;
   [MemberNotNullWhen(true, nameof(Value))]
   public bool HasValue { get; }
   [MemberNotNullWhen(false, nameof(Value))]
   public bool IsNil {
      [MethodImpl(MethodImplOptions.AggressiveInlining)]
      get => !HasValue;
   }
   public RefOption() {
      Value = default!;
      HasValue = false;
   }
   public RefOption(in T value) {
      Value = value;
      HasValue = true;
   }
   public static implicit operator RefOption<T>(in T v) => new(v);
   public bool Try([NotNullWhen(true)] out T? value) {
      if (HasValue) {
         value = Value!;
         return true;
      }
      value = default;
      return false;
   }
   public static explicit operator T(in RefOption<T> n) => n.HasValue ? n.Value! : throw new NilValueException();
}

public static class OptionStructExtensions {
   extension<T>(in Maybe<T> n) where T : struct {
      public T? AsNullable() => n.HasValue ? n.Value : null;
   }
}
public static class OptionClassExtensions {
   extension<T>(in Maybe<T> n) where T : class {
      public T? AsNullable() => n.HasValue ? n.Value : null;
   }
}
