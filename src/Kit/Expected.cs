namespace Kit;

using static MethodImplOptions;

public readonly ref struct Unexpected<E>
where E : allows ref struct {
   public E Error { get; }

   [MethodImpl(AggressiveInlining)]
   public Unexpected(scoped in E error) => Error = error;
}
public readonly ref struct RefExpected<V, E>
where V : allows ref struct
where E : allows ref struct {
   public readonly V? Value { get; }
   public readonly E? Error { get; }

   [MemberNotNullWhen(true, nameof(Value))]
   [MemberNotNullWhen(false, nameof(Error))]
   public bool HasValue { get; }

   [MemberNotNullWhen(false, nameof(Value))]
   [MemberNotNullWhen(true, nameof(Error))]
   public bool HasError {
      [MethodImpl(AggressiveInlining)]
      get => !HasValue;
   }

   [MethodImpl(AggressiveInlining)]
   [OverloadResolutionPriority(1)]
   public RefExpected(scoped in V value) {
      HasValue = true;
      Error = default;
      Value = value;
   }
   [MethodImpl(AggressiveInlining)]
   public RefExpected(scoped in Unexpected<E> u) {
      HasValue = false;
      Value = default;
      Error = u.Error;
   }
   public unsafe RefExpected<X, E> Map<X>(delegate*<V, X> f) where X : allows ref struct
       => HasValue ? f(Value) : new Unexpected<E>(Error);
   public RefExpected<X, E> Map<X>(Func<V, X> f) where X : allows ref struct
       => HasValue ? f(Value) : new Unexpected<E>(Error);
   [MethodImpl(AggressiveInlining)]
   public RefExpected<X, E> Map<F, X>(F f) where F : IInvocable<V, X>, allows ref struct where X : allows ref struct
       => HasValue ? f.Invoke(Value) : new Unexpected<E>(Error);

   public unsafe RefExpected<V, X> MapError<X>(delegate*<E, X> f) where X : allows ref struct
       => HasError ? new Unexpected<X>(f(Error)) : Value;
   public unsafe RefExpected<V, X> MapError<X>(Func<E, X> f) where X : allows ref struct
       => HasError ? new Unexpected<X>(f(Error)) : Value;
   [MethodImpl(AggressiveInlining)]
   public RefExpected<V, X> MapError<F, X>(F f) where F : IInvocable<E, X>, allows ref struct where X : allows ref struct
       => HasValue ? Value : new Unexpected<X>(f.Invoke(Error));

   public unsafe RefExpected<V, E> AndThen(delegate*<V, RefExpected<V, E>> f)
       => HasValue ? f(Value) : this;
   public RefExpected<V, E> AndThen(Func<V, RefExpected<V, E>> f)
       => HasValue ? f(Value) : this;
   [MethodImpl(AggressiveInlining)]
   public RefExpected<V, E> AndThen<F>(F f) where F : IInvocable<V, RefExpected<V, E>>
       => HasValue ? f.Invoke(Value) : this;

   public unsafe RefExpected<X, E> AndThen<X>(delegate*<V, RefExpected<X, E>> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);
   public RefExpected<X, E> AndThen<X>(Func<V, RefExpected<X, E>> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);
   [MethodImpl(AggressiveInlining)]
   public RefExpected<X, E> AndThen<F, X>(F f)
      where F : IInvocable<V, RefExpected<X, E>>, allows ref struct
      where X : allows ref struct
       => HasValue ? f.Invoke(Value) : new Unexpected<E>(Error);

   public unsafe RefExpected<V, E> OrElse(delegate*<E, RefExpected<V, E>> f)
       => HasError ? f(Error) : this;
   public RefExpected<V, E> OrElse(Func<E, RefExpected<V, E>> f)
       => HasError ? f(Error) : this;
   [MethodImpl(AggressiveInlining)]
   public RefExpected<V, E> OrElse<F>(F f)
      where F : IInvocable<E, RefExpected<V, E>>, allows ref struct
       => HasError ? f.Invoke(Error) : this;

   public unsafe RefExpected<V, X> OrElse<X>(delegate*<E, RefExpected<V, X>> f) where X : allows ref struct
       => HasError ? f(Error) : Value;
   public RefExpected<V, X> OrElse<X>(Func<E, RefExpected<V, X>> f) where X : allows ref struct
       => HasError ? f(Error) : Value;
   [MethodImpl(AggressiveInlining)]
   public RefExpected<V, X> OrElse<F, X>(F f)
      where F : IInvocable<E, RefExpected<V, X>>, allows ref struct
      where X : allows ref struct
       => HasError ? f.Invoke(Error) : Value;

   [MethodImpl(AggressiveInlining)]
   public static implicit operator RefExpected<V, E>(scoped in V v) => new(v);

   [MethodImpl(AggressiveInlining)]
   public static implicit operator RefExpected<V, E>(scoped in Unexpected<E> u) => new(u);

   [MethodImpl(AggressiveInlining)]
   public static bool operator true(in RefExpected<V, E> r) => r.HasValue;
   [MethodImpl(AggressiveInlining)]
   public static bool operator false(in RefExpected<V, E> r) => r.HasError;
   [MethodImpl(AggressiveInlining)]
   public static bool operator !(in RefExpected<V, E> r) => r.HasError;

   public static explicit operator V(in RefExpected<V, E> e) => e.HasValue ? e.Value : throw new InvalidCastException();
}

public readonly struct Expected<V, E> {
   [MemberNotNullWhen(true, nameof(Value))]
   [MemberNotNullWhen(false, nameof(Error))]
   public bool HasValue {
      [MethodImpl(AggressiveInlining)]
      get => field;
   }
   [MemberNotNullWhen(false, nameof(Value))]
   [MemberNotNullWhen(true, nameof(Error))]
   public bool HasError {
      [MethodImpl(AggressiveInlining)]
      get => !HasValue;
   }

   public V? Value { get; }
   public E? Error { get; }

   [MethodImpl(AggressiveInlining)]
   [OverloadResolutionPriority(1)]
   public Expected(in V value) {
      HasValue = true;
      Error = default;
      Value = value;
   }
   [MethodImpl(AggressiveInlining)]
   public Expected(in Unexpected<E> u) {
      HasValue = false;
      Value = default;
      Error = u.Error;
   }

   public unsafe Expected<X, E> Map<X>(delegate*<V, X> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);
   public Expected<X, E> Map<X>(Func<V, X> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);
   [MethodImpl(AggressiveInlining)]
   public Expected<X, E> Map<F, X>(F f) where F : IInvocable<V, X>, allows ref struct
       => HasValue ? f.Invoke(Value) : new Unexpected<E>(Error);

   public unsafe Expected<V, X> MapError<X>(delegate*<E, X> f)
       => HasError ? new Unexpected<X>(f(Error)) : Value;
   public Expected<V, X> MapError<X>(Func<E, X> f)
       => HasError ? new Unexpected<X>(f(Error)) : Value;
   [MethodImpl(AggressiveInlining)]
   public Expected<V, X> MapError<F, X>(F f) where F : IInvocable<E, X>, allows ref struct
       => HasValue ? Value : new Unexpected<X>(f.Invoke(Error));

   public unsafe Expected<V, E> AndThen(delegate*<V, Expected<V, E>> f)
       => HasValue ? f(Value) : this;
   public Expected<V, E> AndThen(Func<V, Expected<V, E>> f)
       => HasValue ? f(Value) : this;
   [MethodImpl(AggressiveInlining)]
   public Expected<V, E> AndThen<F>(F f) where F : IInvocable<V, Expected<V, E>>, allows ref struct
       => HasValue ? f.Invoke(Value) : this;

   public unsafe Expected<X, E> AndThen<X>(delegate*<V, Expected<X, E>> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);
   public Expected<X, E> AndThen<X>(Func<V, Expected<X, E>> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);
   [MethodImpl(AggressiveInlining)]
   public Expected<X, E> AndThen<F, X>(F f) where F : IInvocable<V, Expected<X, E>>, allows ref struct
       => HasValue ? f.Invoke(Value) : new Unexpected<E>(Error);

   public unsafe Expected<V, E> OrElse(delegate*<E, Expected<V, E>> f)
       => HasError ? f(Error) : this;
   public Expected<V, E> OrElse(Func<E, Expected<V, E>> f)
       => HasError ? f(Error) : this;
   [MethodImpl(AggressiveInlining)]
   public Expected<V, E> OrElse<F>(F f) where F : IInvocable<E, Expected<V, E>>, allows ref struct
       => HasError ? f.Invoke(Error) : this;

   public unsafe Expected<V, X> OrElse<X>(delegate*<E, Expected<V, X>> f)
       => HasError ? f(Error) : Value;
   public Expected<V, X> OrElse<X>(Func<E, Expected<V, X>> f)
       => HasError ? f(Error) : Value;
   [MethodImpl(AggressiveInlining)]
   public Expected<V, X> OrElse<F, X>(F f) where F : IInvocable<E, Expected<V, X>>, allows ref struct
       => HasError ? f.Invoke(Error) : Value;

   [MethodImpl(AggressiveInlining)]
   public static implicit operator Expected<V, E>(in V v) => new(v);

   [MethodImpl(AggressiveInlining)]
   public static implicit operator Expected<V, E>(in Unexpected<E> u) => new(u);

   [MethodImpl(AggressiveInlining)]
   public static bool operator true(in Expected<V, E> r) => r.HasValue;
   [MethodImpl(AggressiveInlining)]
   public static bool operator false(in Expected<V, E> r) => r.HasError;
   [MethodImpl(AggressiveInlining)]
   public static bool operator !(in Expected<V, E> r) => r.HasError;

   public static explicit operator V(in Expected<V, E> e) => e.HasValue ? e.Value : throw new InvalidCastException();
}

public static class ExpectsExtensions {
   extension<V, E>(RefExpected<V, E> e) {
      public bool Ok => e.HasValue;
      public Expected<V, E> AsExpected() => e.HasValue ? e.Value : new Unexpected<E>(e.Error);
   }
   extension<V, E>(Expected<V, E> e) {
      public bool Ok => e.HasValue;
      public RefExpected<V, E> AsRefExpected() => e.HasValue ? e.Value : new Unexpected<E>(e.Error);
   }
}
