namespace Kit;
using System.Diagnostics.CodeAnalysis;
using static Result;

public readonly struct NullWrapper<T> {
    public readonly struct Null;
    internal readonly T _value;
    public bool HasValue {get; init;}
    public bool IsNull => !HasValue;
    public NullWrapper(in T value) {
        _value = value;
        HasValue = true;
    }
    public NullWrapper() {
        _value = default!;
        HasValue = false;
    }
    public static implicit operator NullWrapper<T>(in T v) => new(v);
    public static implicit operator NullWrapper<T>(Null? p) => new();
    public bool TryUnwrap([NotNullWhen(true)] out T? value) {
        if (HasValue) {
            value = _value!;
            return true;
        }
        value = default;
        return false;
    }
    public Result<T, Null> AsResult() => HasValue ? Ok(_value) : Err(new Null());
}

public static class NullWrapper_WhereStruct {
    public static T? Unwrap<T>(this in NullWrapper<T> w) where T: struct => w.HasValue ? w._value : null;
}
public static class NullWrapper_WhereClass {
    public static T? Unwrap<T>(this in NullWrapper<T> w) where T: class => w.HasValue ? w._value : null;
}
