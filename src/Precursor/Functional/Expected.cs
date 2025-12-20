namespace Precursor.Functional;

using static MethodImplOptions;

public sealed class InvalidExpectedAccessException : InvalidOperationException;

public static class UnexpectedFunction {
   [MethodImpl(AggressiveInlining)]
   public static Unexpected<E> Unexpected<E>(scoped in E error) => new(error);
}

public readonly ref struct Unexpected<E>
where E : allows ref struct {
   public E Error { get; }

   [MethodImpl(AggressiveInlining)]
   public Unexpected(scoped in E error) => Error = error;
}

public readonly struct Expected<T, E> {
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

   public T? Value { get; }
   public E? Error { get; }

   [MethodImpl(AggressiveInlining)]
   [OverloadResolutionPriority(1)]
   public Expected(in T value) {
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

   public Expected<X, E> Map<X>(Func<T, X> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);

   [MethodImpl(AggressiveInlining)]
   public Expected<X, E> Map<Invoker, X>(in Invoker f)
      where Invoker : IInvoker<T, X>, allows ref struct
       => HasValue ? f.Invoke(Value) : new Unexpected<E>(Error);

   [MethodImpl(AggressiveInlining)]
   public Expected<X, E> Map<StaticInvoker, X>()
      where StaticInvoker : IStaticInvoker<T, X>, allows ref struct
       => HasValue ? StaticInvoker.Invoke(Value) : new Unexpected<E>(Error);

   public unsafe Expected<X, E> Map<X>(delegate* managed<in T, X> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);

   public unsafe Expected<X, E> Map<X>(delegate* managed<T, X> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);

   public unsafe Expected<T, X> MapError<X>(delegate* managed<in E, X> f)
       => HasError ? new Unexpected<X>(f(Error)) : Value;

   public unsafe Expected<T, X> MapError<X>(delegate* managed<E, X> f)
       => HasError ? new Unexpected<X>(f(Error)) : Value;

   public Expected<T, X> MapError<X>(Func<E, X> f)
       => HasError ? new Unexpected<X>(f(Error)) : Value;

   [MethodImpl(AggressiveInlining)]
   public Expected<T, X> MapError<Invoker, X>(in Invoker f)
      where Invoker : IInvoker<E, X>, allows ref struct
       => HasValue ? Value : new Unexpected<X>(f.Invoke(Error));

   [MethodImpl(AggressiveInlining)]
   public Expected<T, X> MapError<StaticInvoker, X>()
      where StaticInvoker : IStaticInvoker<E, X>, allows ref struct
       => HasValue ? Value : new Unexpected<X>(StaticInvoker.Invoke(Error));

   public unsafe Expected<T, E> AndThen(delegate* managed<T, Expected<T, E>> f)
       => HasValue ? f(Value) : this;

   public unsafe Expected<T, E> AndThen(delegate* managed<in T, Expected<T, E>> f)
       => HasValue ? f(Value) : this;

   public Expected<T, E> AndThen(Func<T, Expected<T, E>> f)
       => HasValue ? f(Value) : this;

   [MethodImpl(AggressiveInlining)]
   public Expected<T, E> AndThen<Invoker>(in Invoker f)
      where Invoker : IInvoker<T, Expected<T, E>>, allows ref struct
       => HasValue ? f.Invoke(Value) : this;

   [MethodImpl(AggressiveInlining)]
   public Expected<T, E> AndThen<StaticInvoker>()
      where StaticInvoker : IStaticInvoker<T, Expected<T, E>>, allows ref struct
       => HasValue ? StaticInvoker.Invoke(Value) : this;

   public unsafe Expected<X, E> AndThen<X>(delegate* managed<T, Expected<X, E>> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);

   public unsafe Expected<X, E> AndThen<X>(delegate* managed<in T, Expected<X, E>> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);

   public Expected<X, E> AndThen<X>(Func<T, Expected<X, E>> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);

   [MethodImpl(AggressiveInlining)]
   public Expected<X, E> AndThen<Invoker, X>(in Invoker f)
      where Invoker : IInvoker<T, Expected<X, E>>, allows ref struct
       => HasValue ? f.Invoke(Value) : new Unexpected<E>(Error);

   [MethodImpl(AggressiveInlining)]
   public Expected<X, E> AndThen<StaticInvoker, X>()
      where StaticInvoker : IStaticInvoker<T, Expected<X, E>>, allows ref struct
       => HasValue ? StaticInvoker.Invoke(Value) : new Unexpected<E>(Error);

   public unsafe Expected<T, E> OrElse(delegate* managed<E, Expected<T, E>> f)
       => HasError ? f(Error) : this;

   public unsafe Expected<T, E> OrElse(delegate* managed<in E, Expected<T, E>> f)
       => HasError ? f(Error) : this;

   public Expected<T, E> OrElse(Func<E, Expected<T, E>> f)
       => HasError ? f(Error) : this;

   [MethodImpl(AggressiveInlining)]
   public Expected<T, E> OrElse<Invoker>(in Invoker f)
      where Invoker : IInvoker<E, Expected<T, E>>, allows ref struct
       => HasError ? f.Invoke(Error) : this;

   [MethodImpl(AggressiveInlining)]
   public Expected<T, E> OrElse<StaticInvoker>()
      where StaticInvoker : IStaticInvoker<E, Expected<T, E>>, allows ref struct
       => HasError ? StaticInvoker.Invoke(Error) : this;

   public unsafe Expected<T, X> OrElse<X>(delegate* managed<E, Expected<T, X>> f)
       => HasError ? f(Error) : Value;

   public unsafe Expected<T, X> OrElse<X>(delegate* managed<in E, Expected<T, X>> f)
       => HasError ? f(Error) : Value;

   public Expected<T, X> OrElse<X>(Func<E, Expected<T, X>> f)
       => HasError ? f(Error) : Value;

   [MethodImpl(AggressiveInlining)]
   public Expected<T, X> OrElse<Invoker, X>(in Invoker f)
      where Invoker : IInvoker<E, Expected<T, X>>, allows ref struct
       => HasError ? f.Invoke(Error) : Value;

   [MethodImpl(AggressiveInlining)]
   public Expected<T, X> OrElse<StaticInvoker, X>()
      where StaticInvoker : IStaticInvoker<E, Expected<T, X>>, allows ref struct
       => HasError ? StaticInvoker.Invoke(Error) : Value;

   [MethodImpl(AggressiveInlining)]
   public static implicit operator Expected<T, E>(in T v) => new(v);

   [MethodImpl(AggressiveInlining)]
   public static implicit operator Expected<T, E>(in Unexpected<E> u) => new(u);

   [MethodImpl(AggressiveInlining)]
   public static bool operator true(in Expected<T, E> r) => r.HasValue;

   [MethodImpl(AggressiveInlining)]
   public static bool operator false(in Expected<T, E> r) => r.HasError;

   [MethodImpl(AggressiveInlining)]
   public static bool operator !(in Expected<T, E> r) => r.HasError;

   public static explicit operator T(in Expected<T, E> e) => e.HasValue ? e.Value : throw new InvalidExpectedAccessException();
}
public readonly ref struct RefExpected<T, E>
where T : allows ref struct
where E : allows ref struct {
   public readonly T? Value { get; }
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
   public RefExpected(scoped in T value) {
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
   public unsafe RefExpected<X, E> Map<X>(delegate*<T, X> f)
      where X : allows ref struct
       => HasValue ? f(Value) : new Unexpected<E>(Error);

   public RefExpected<X, E> Map<X>(Func<T, X> f)
      where X : allows ref struct
       => HasValue ? f(Value) : new Unexpected<E>(Error);

   [MethodImpl(AggressiveInlining)]
   public RefExpected<X, E> Map<Invoker, X>(in Invoker f)
      where Invoker : IInvoker<T, X>, allows ref struct
      where X : allows ref struct
       => HasValue ? f.Invoke(Value) : new Unexpected<E>(Error);

   [MethodImpl(AggressiveInlining)]
   public RefExpected<X, E> Map<StaticInvoker, X>()
      where StaticInvoker : IStaticInvoker<T, X>, allows ref struct
      where X : allows ref struct
       => HasValue ? StaticInvoker.Invoke(Value) : new Unexpected<E>(Error);

   public unsafe RefExpected<T, X> MapError<X>(delegate*<E, X> f)
      where X : allows ref struct
       => HasError ? new Unexpected<X>(f(Error)) : Value;

   public unsafe RefExpected<T, X> MapError<X>(Func<E, X> f)
      where X : allows ref struct
       => HasError ? new Unexpected<X>(f(Error)) : Value;

   [MethodImpl(AggressiveInlining)]
   public RefExpected<T, X> MapError<Invoker, X>(in Invoker f)
      where Invoker : IInvoker<E, X>, allows ref struct
      where X : allows ref struct
       => HasValue ? Value : new Unexpected<X>(f.Invoke(Error));

   [MethodImpl(AggressiveInlining)]
   public RefExpected<T, X> MapError<StaticInvoker, X>()
      where StaticInvoker : IStaticInvoker<E, X>, allows ref struct
      where X : allows ref struct
       => HasValue ? Value : new Unexpected<X>(StaticInvoker.Invoke(Error));

   public unsafe RefExpected<T, E> AndThen(delegate*<T, RefExpected<T, E>> f)
       => HasValue ? f(Value) : this;
   public RefExpected<T, E> AndThen(Func<T, RefExpected<T, E>> f)
       => HasValue ? f(Value) : this;
   [MethodImpl(AggressiveInlining)]
   public RefExpected<T, E> AndThen<Invoker>(Invoker f)
      where Invoker : IInvoker<T, RefExpected<T, E>>
       => HasValue ? f.Invoke(Value) : this;
   [MethodImpl(AggressiveInlining)]
   public RefExpected<T, E> AndThen<StaticInvoker>()
      where StaticInvoker : IStaticInvoker<T, RefExpected<T, E>>
       => HasValue ? StaticInvoker.Invoke(Value) : this;

   public unsafe RefExpected<X, E> AndThen<X>(delegate*<T, RefExpected<X, E>> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);
   public RefExpected<X, E> AndThen<X>(Func<T, RefExpected<X, E>> f)
       => HasValue ? f(Value) : new Unexpected<E>(Error);

   [MethodImpl(AggressiveInlining)]
   public RefExpected<X, E> AndThen<Invoker, X>(in Invoker f)
      where Invoker : IInvoker<T, RefExpected<X, E>>, allows ref struct
      where X : allows ref struct
       => HasValue ? f.Invoke(Value) : new Unexpected<E>(Error);

   [MethodImpl(AggressiveInlining)]
   public RefExpected<X, E> AndThen<StaticInvoker, X>()
      where StaticInvoker : IStaticInvoker<T, RefExpected<X, E>>, allows ref struct
      where X : allows ref struct
       => HasValue ? StaticInvoker.Invoke(Value) : new Unexpected<E>(Error);

   public unsafe RefExpected<T, E> OrElse(delegate*<E, RefExpected<T, E>> f)
       => HasError ? f(Error) : this;

   public RefExpected<T, E> OrElse(Func<E, RefExpected<T, E>> f)
       => HasError ? f(Error) : this;

   [MethodImpl(AggressiveInlining)]
   public RefExpected<T, E> OrElse<Invoker>(in Invoker f)
      where Invoker : IInvoker<E, RefExpected<T, E>>, allows ref struct
       => HasError ? f.Invoke(Error) : this;

   [MethodImpl(AggressiveInlining)]
   public RefExpected<T, E> OrElse<StaticInvoker>()
      where StaticInvoker : IStaticInvoker<E, RefExpected<T, E>>, allows ref struct
       => HasError ? StaticInvoker.Invoke(Error) : this;

   public unsafe RefExpected<T, X> OrElse<X>(delegate*<E, RefExpected<T, X>> f)
      where X : allows ref struct
       => HasError ? f(Error) : Value;

   public RefExpected<T, X> OrElse<X>(Func<E, RefExpected<T, X>> f)
      where X : allows ref struct
       => HasError ? f(Error) : Value;

   [MethodImpl(AggressiveInlining)]
   public RefExpected<T, X> OrElse<Invoker, X>(in Invoker f)
      where Invoker : IInvoker<E, RefExpected<T, X>>, allows ref struct
      where X : allows ref struct
       => HasError ? f.Invoke(Error) : Value;

   [MethodImpl(AggressiveInlining)]
   public RefExpected<T, X> OrElse<StaticInvoker, X>()
      where StaticInvoker : IStaticInvoker<E, RefExpected<T, X>>, allows ref struct
      where X : allows ref struct
       => HasError ? StaticInvoker.Invoke(Error) : Value;

   [MethodImpl(AggressiveInlining)]
   public static implicit operator RefExpected<T, E>(scoped in T v) => new(v);

   [MethodImpl(AggressiveInlining)]
   public static implicit operator RefExpected<T, E>(scoped in Unexpected<E> u) => new(u);

   [MethodImpl(AggressiveInlining)]
   public static bool operator true(in RefExpected<T, E> r) => r.HasValue;
   [MethodImpl(AggressiveInlining)]
   public static bool operator false(in RefExpected<T, E> r) => r.HasError;
   [MethodImpl(AggressiveInlining)]
   public static bool operator !(in RefExpected<T, E> r) => r.HasError;

   public static explicit operator T(in RefExpected<T, E> e) => e.HasValue ? e.Value : throw new InvalidExpectedAccessException();
}

public static class ExpectedExtensions {
   extension<V, E>(RefExpected<V, E> e) {
      public Expected<V, E> AsExpected() => e.HasValue ? e.Value : new Unexpected<E>(e.Error);
   }
   extension<V, E>(Expected<V, E> e) {
      public RefExpected<V, E> AsRefExpected() => e.HasValue ? e.Value : new Unexpected<E>(e.Error);
   }
}
