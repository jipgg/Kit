namespace Precursor.Functional;

public interface IInvoker<in T, out R>
where T : allows ref struct
where R : allows ref struct {
   R Invoke(T v);
}
public interface IInvoker<in T0, in T1, out R>
where T0 : allows ref struct
where T1 : allows ref struct
where R : allows ref struct {
   R Invoke(T0 v0, T1 v1);
}
public interface IInvoker<in T0, in T1, in T2, out R>
where T0 : allows ref struct
where T1 : allows ref struct
where T2 : allows ref struct
where R : allows ref struct {
   R Invoke(T0 v0, T1 v1, T2 v2);
}
public interface IInvoker<in T0, in T1, in T2, in T3, out R>
where T0 : allows ref struct
where T1 : allows ref struct
where T2 : allows ref struct
where T3 : allows ref struct
where R : allows ref struct {
   R Invoke(T0 v0, T1 v1, T2 v2, T3 v3);
}
public interface IInvoker<in T0, in T1, in T2, in T3, in T4, out R>
where T0 : allows ref struct
where T1 : allows ref struct
where T2 : allows ref struct
where T3 : allows ref struct
where T4 : allows ref struct
where R : allows ref struct {
   R Invoke(T0 v0, T1 v1, T2 v2, T3 v3, T4 v4);
}
public interface IInvoker<in T0, in T1, in T2, in T3, in T4, in T5, out R>
where T0 : allows ref struct
where T1 : allows ref struct
where T2 : allows ref struct
where T3 : allows ref struct
where T4 : allows ref struct
where T5 : allows ref struct
where R : allows ref struct {
   R Invoke(T0 v0, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5);
}

public interface IStaticInvoker<in T, out R>
where T : allows ref struct
where R : allows ref struct {
   static abstract R Invoke(T v);
}
public interface IStaticInvoker<in T0, in T1, out R>
where T0 : allows ref struct
where T1 : allows ref struct
where R : allows ref struct {
   static abstract R Invoke(T0 v0, T1 v1);
}
public interface IStaticInvoker<in T0, in T1, in T2, out R>
where T0 : allows ref struct
where T1 : allows ref struct
where T2 : allows ref struct
where R : allows ref struct {
   static abstract R Invoke(T0 v0, T1 v1, T2 v2);
}
public interface IStaticInvoker<in T0, in T1, in T2, in T3, out R>
where T0 : allows ref struct
where T1 : allows ref struct
where T2 : allows ref struct
where T3 : allows ref struct
where R : allows ref struct {
   static abstract R Invoke(T0 v0, T1 v1, T2 v2, T3 v3);
}
public interface IStaticInvoker<in T0, in T1, in T2, in T3, in T4, out R>
where T0 : allows ref struct
where T1 : allows ref struct
where T2 : allows ref struct
where T3 : allows ref struct
where T4 : allows ref struct
where R : allows ref struct {
   static abstract R Invoke(T0 v0, T1 v1, T2 v2, T3 v3, T4 v4);
}
public interface IStaticInvoker<in T0, in T1, in T2, in T3, in T4, in T5, out R>
where T0 : allows ref struct
where T1 : allows ref struct
where T2 : allows ref struct
where T3 : allows ref struct
where T4 : allows ref struct
where T5 : allows ref struct
where R : allows ref struct {
   static abstract R Invoke(T0 v0, T1 v1, T2 v2, T3 v3, T4 v4, T5 v5);
}
