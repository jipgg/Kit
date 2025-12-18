namespace Kit;

public sealed class InvalidOptionAccessException: InvalidOperationException;

public readonly struct Option<T> {
   public readonly T? Value;
   [MemberNotNullWhen(true, nameof(Value))]
   public bool HasValue { get; }
   public Option() {
      Value = default!;
      HasValue = false;
   }
   public Option(in T value) {
      Value = value;
      HasValue = true;
   }
   public static implicit operator Option<T>(in T v) => new(v);
   public bool TryValue([NotNullWhen(true)] out T? value) {
      if (HasValue) {
         value = Value!;
         return true;
      }
      value = default;
      return false;
   }
   public unsafe Option<X> Map<X>(delegate*<T, X> f)
       => HasValue ? new(f(Value)) : default;
   public Option<X> Map<X>(Func<T, X> f)
       => HasValue ? new(f(Value)) : default;
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Option<X> Map<F, X>(F f) where F : IInvocable<T, X>, allows ref struct
       => HasValue ? new(f.Invoke(Value)) : default;
   public unsafe Option<T> AndThen(delegate*<T, Option<T>> f)
       => HasValue ? f(Value) : this;
   public Option<T> AndThen(Func<T, Option<T>> f)
       => HasValue ? f(Value) : this;
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Option<T> AndThen<F>(F f) where F : IInvocable<T, Option<T>>, allows ref struct
       => HasValue ? f.Invoke(Value) : this;

   public unsafe Option<X> AndThen<X>(delegate*<T, Option<X>> f)
       => HasValue ? f(Value) : default;
   public Option<X> AndThen<X>(Func<T, Option<X>> f)
       => HasValue ? f(Value) : default;
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public Option<X> AndThen<F, X>(F f) where F : IInvocable<T, Option<X>>, allows ref struct
       => HasValue ? f.Invoke(Value) : default;

   public static explicit operator T(in Option<T> n) => n.HasValue ? n.Value! : throw new InvalidOptionAccessException();
}

public readonly ref struct RefOption<T>
where T : allows ref struct {
   public readonly T? Value { get; }
   [MemberNotNullWhen(true, nameof(Value))]
   public bool HasValue { get; }
   public RefOption() {
      Value = default!;
      HasValue = false;
   }
   public RefOption(scoped in T value) {
      Value = value;
      HasValue = true;
   }
   public static implicit operator RefOption<T>(scoped in T v) => new(v);
   public bool TryValue([NotNullWhen(true)] out T? value) {
      if (HasValue) {
         value = Value!;
         return true;
      }
      value = default;
      return false;
   }
   public unsafe RefOption<X> Map<X>(delegate*<T, X> f) where X : allows ref struct
       => HasValue ? new(f(Value)) : default;
   public RefOption<X> Map<X>(Func<T, X> f) where X : allows ref struct
       => HasValue ? new(f(Value)) : default;
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public RefOption<X> Map<F, X>(F f) where F : IInvocable<T, X>, allows ref struct where X : allows ref struct
       => HasValue ? new(f.Invoke(Value)) : default;
   public unsafe RefOption<T> AndThen(delegate*<T, RefOption<T>> f)
       => HasValue ? f(Value) : this;
   public RefOption<T> AndThen(Func<T, RefOption<T>> f)
       => HasValue ? f(Value) : this;

   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public RefOption<T> AndThen<F>(F f) where F : IInvocable<T, RefOption<T>>, allows ref struct
       => HasValue ? f.Invoke(Value) : this;

   public unsafe RefOption<X> AndThen<X>(delegate*<T, RefOption<X>> f) where X : allows ref struct
       => HasValue ? f(Value) : default;
   public RefOption<X> AndThen<X>(Func<T, RefOption<X>> f) where X : allows ref struct
       => HasValue ? f(Value) : default;
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public RefOption<X> AndThen<F, X>(F f) where F : IInvocable<T, RefOption<X>>, allows ref struct where X : allows ref struct
       => HasValue ? f.Invoke(Value) : default;
   public static explicit operator T(in RefOption<T> n) => n.HasValue ? n.Value! : throw new InvalidOptionAccessException();
}

public static class OptionExtensions {
   extension<T>(in RefOption<T> o) {
      public Option<T> AsOption() => o.HasValue ? new(o.Value) : default;
   }
   extension<T>(in Option<T> o) {
      public RefOption<T> AsRefOption() => o.HasValue ? new(o.Value) : default;
   }
}

public static class OptionStructExtensions {
   extension<T>(in Option<T> n) where T : struct {
      public T? AsNullable() => n.HasValue ? n.Value : null;
   }
   extension<T>(in RefOption<T> o) where T : struct {
      public T? AsNullable() => o.HasValue ? o.Value : null;
   }
}
public static class OptionClassExtensions {
   extension<T>(in Option<T> n) where T : class {
      public T? AsNullable() => n.HasValue ? n.Value : null;
   }
   extension<T>(in RefOption<T> o) where T : class {
      public T? AsNullable() => o.HasValue ? o.Value : null;
   }
}
