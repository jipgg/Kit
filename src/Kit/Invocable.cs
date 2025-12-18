namespace Kit;

public interface IInvocable<in T, out R>
where T : allows ref struct
where R : allows ref struct {
   R Invoke(T v);
}

public readonly struct FuncInvocableWrapper<T, R>(Func<T, R> f) : IInvocable<T, R>
where T : allows ref struct
where R : allows ref struct {
   public R Invoke(T v) => f.Invoke(v);
}

public readonly struct ActionInvocableWrapper<T>(Action<T> f) : IInvocable<T, Nil>
where T : allows ref struct {
   public Nil Invoke(T v) {
      f.Invoke(v);
      return default;
   }
}

public static class InvocableExtensions {
   extension<T, R>(Func<T, R> f)
   where T : allows ref struct
   where R : allows ref struct {
      public FuncInvocableWrapper<T, R> AsInvocable() => new(f);
   }
   extension<T>(Action<T> f)
   where T : allows ref struct {
      public ActionInvocableWrapper<T> AsInvocable() => new(f);
   }
}

public static class Invocable {
   static R Invoke<Invocable, T, R>(scoped in Invocable f, scoped in T v)
   where Invocable : IInvocable<T, R> {
      return f.Invoke(v);
   }
}

