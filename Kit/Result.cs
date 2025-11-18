namespace Kit;
using System.Diagnostics.CodeAnalysis;
using static Result;

public readonly record struct ResultErr<E>(in E Err);
public readonly record struct ResultOk<T>(in T Ok);
public sealed class ResultWasErr : Exception;
public sealed class ResultWasOk : Exception;

public static class Result {
    public static ResultOk<T> Ok<T>(in T r) => new(r);
    public static ResultErr<E> Err<E>(in E r) => new(r);
    public static Result<T, E> Wrap<T, E>(Func<T> f) where E: Exception {
        try {
            return Ok(f());
        } catch (E e) {
            return Err(e);
        }
    }
    public static Result<T, Exception> Wrap<T>(Func<T> f) => Wrap<T, Exception>(f);
}

public readonly struct Result<T, E> {
    internal readonly T _value;
    internal readonly E _error;
    internal readonly bool _hasValue;

    Result(bool hasValue, in T value, in E error) {
        _hasValue = hasValue;
        _value = value;
        _error = error;
    }
    public Result(in ResultOk<T> r) {
        _value = r.Ok;
        _error = default!;
        _hasValue = true;
    }
    public Result(in ResultErr<E> r) {
        _value = default!;
        _error = r.Err;
        _hasValue = false;
    }
    public bool HasValue => _hasValue;
    public bool HasError => !_hasValue;
    public T Value => HasValue ? _value : throw new ResultWasOk();
    public E Error => HasError ? _error : throw new ResultWasErr();
    public static implicit operator Result<T, E>(in ResultErr<E> err) => new(err);
    public static implicit operator Result<T, E>(in ResultOk<T> ok) => new(ok);
    public static bool operator true(in Result<T, E> r) => r._hasValue;
    public static bool operator false(in Result<T, E> r) => !r._hasValue;
    public static bool operator !(in Result<T, E> r) => !r._hasValue;

    public Result<T, E> AndThen(Func<T, Result<T, E>> fn) => _hasValue ? fn(_value) : this;
    public Result<T, E> Peek(Action<T> fn) {
        if (_hasValue) fn(_value);
        return this;
    }
    public Result<T, E> OrElse(Func<E, Result<T, E>> errFn) => _hasValue ? this : errFn(_error);
    public Result<T, E> PeekError(Action<E> errFn) {
        if (HasError) errFn(_error);
        return this;
    }

    public Result<R, E> Map<R>(Func<T, R> fok) => _hasValue ? Ok(fok(_value)) : Err(_error);
    public Result<T, R> MapError<R>(Func<E, R> ferr) => _hasValue ? Ok(_value) : Err(ferr(_error));
    public R Match<R>(Func<T, R> fok, Func<E, R> ferr) => _hasValue ? fok(_value) : ferr(_error);
    public void Switch(Action<T> valueFn, Action<E> errorFn) {
        if (_hasValue) valueFn(_value);
        else errorFn(_error);
    }
    public bool Try([NotNullWhen(true)] out T? val) {
        if (_hasValue) {
            val = _value!;
            return true;
        }
        val = default!;
        return false;
    }
    public bool TryError([NotNullWhen(true)] out E? err) {
        if (HasError) {
            err = _error!;
            return true;
        }
        err = default!;
        return false;
    }
}

public static class Result_WhereStruct {
    public static T? Unwrap<T, E>(this in Result<T, E> result) where T : struct
        => result.HasValue ? result._value : null;
    public static E? UnwrapError<T, E>(this in Result<T, E> result) where E : struct
        => result.HasValue ? null : result._error;
}
public static class Result_WhereClass {
    public static T? Unwrap<T, E>(this in Result<T, E> result) where T : class
        => result.HasValue ? result._value : null;
    public static E? UnwrapError<T, E>(this in Result<T, E> result) where E : class
        => result.HasValue ? null : result._error;
}

public static partial class Result_Deconstruct {
    public static void Deconstruct<T, E>(this in Result<T, E> r, out T? ok, out E? err) where T : struct where E : struct {
        if (r.HasValue) {
            ok = r._value;
            err = null;
        } else {
            ok = null;
            err = r._error;
        }
    }
    public static void Deconstruct<T, E>(this in Result<T, E> r, out T? ok, out E? err) where T : class where E : struct {
        if (r.HasValue) {
            ok = r._value;
            err = null;
        } else {
            ok = null;
            err = r._error;
        }
    }
    public static void Deconstruct<T, E>(this in Result<T, E> r, out T? ok, out E? err) where T : struct where E : class {
        if (r.HasValue) {
            ok = r._value;
            err = null;
        } else {
            ok = null;
            err = r._error;
        }
    }
    public static void Deconstruct<T, E>(this in Result<T, E> r, out T? ok, out E? err) where T : class where E : class {
        if (r.HasValue) {
            ok = r._value;
            err = null;
        } else {
            ok = null;
            err = r._error;
        }
    }
}
