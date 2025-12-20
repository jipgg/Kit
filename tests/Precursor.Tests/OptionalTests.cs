using FluentAssertions;
using Precursor.Functional;

namespace Precursor.Tests;

using Optional = Optional<Foo>;
using RefOptional = RefOptional<RefFoo>;
using InvalidAccess = InvalidOptionalAccessException;

public class Optional_Tests {
   [Fact]
   public void HasValue_when_constructed_with_value() {
      var o = new Optional(new Foo(42));
      o.HasValue.Should().BeTrue();
      o.Value!.X.Should().Be(42);
   }

   [Fact]
   public void Default_has_no_value() {
      var o = default(Optional);
      o.HasValue.Should().BeFalse();
   }

   [Fact]
   public void Explicit_cast_throws_when_empty() {
      var o = default(Optional);
      Action act = () => { var _ = (Foo)o; };
      act.Should().Throw<InvalidAccess>();
   }
}

public class Optional_MapTests {
   [Fact]
   public void Map_applies_only_when_has_value() {
      var o = new Optional(new Foo(10))
          .Map(f => new Foo(f.X * 2));

      o.HasValue.Should().BeTrue();
      o.Value!.X.Should().Be(20);
   }

   [Fact]
   public void Map_skips_when_empty() {
      var o = default(Optional)
          .Map(f => new Foo(f.X * 2));

      o.HasValue.Should().BeFalse();
   }
}

public class Optional_BindTests {
   static Optional Inc(Foo f)
       => new Optional(new Foo(f.X + 1));

   [Fact]
   public void AndThen_left_identity() {
      var o = new Optional(new Foo(5))
          .AndThen(Inc);

      o.HasValue.Should().BeTrue();
      o.Value!.X.Should().Be(6);
   }

   [Fact]
   public void AndThen_right_identity() {
      var o = new Optional(new Foo(7));
      var bound = o.AndThen(x => new Optional(x));

      bound.HasValue.Should().BeTrue();
      bound.Value!.X.Should().Be(7);
   }

   [Fact]
   public void AndThen_skips_when_empty() {
      var o = default(Optional)
          .AndThen(Inc);

      o.HasValue.Should().BeFalse();
   }
}

public unsafe class Optional_DelegatePointerTests {
   static Foo Double(Foo f) => new Foo(f.X * 2);
   static Optional DoubleOpt(Foo f) => new(new Foo(f.X * 2));

   [Fact]
   public void Map_with_delegate_pointer_works() {
      var o = new Optional(new Foo(3))
          .Map(&Double);

      o.Value!.X.Should().Be(6);
   }

   [Fact]
   public void AndThen_with_delegate_pointer_works() {
      var o = new Optional(new Foo(3))
          .AndThen(&DoubleOpt);

      o.Value!.X.Should().Be(6);
   }
}

public class Optional_InvokerTests {
   struct Invoker : IInvoker<Foo, Foo> {
      public Foo Invoke(Foo f) => new Foo(f.X * 2);
   }
   struct StaticInvoker : IStaticInvoker<Foo, Foo> {
      public static Foo Invoke(Foo f) => new Foo(f.X * 2);
   }

   struct InvokerOpt : IInvoker<Foo, Optional> {
      public Optional Invoke(Foo f) => new(new Foo(f.X * 2));
   }
   struct StaticInvokerOpt : IStaticInvoker<Foo, Optional> {
      public static Optional Invoke(Foo f) => new(new Foo(f.X * 2));
   }

   [Fact]
   public void Map_with_IInvoker_works() {
      var o = new Optional(new Foo(4))
          .Map<Invoker, Foo>(default);

      o.Value!.X.Should().Be(8);
   }
   [Fact]
   public void Map_with_IStaticInvoker_works() {
      var o = new Optional(new Foo(4))
          .Map<StaticInvoker, Foo>();

      o.Value!.X.Should().Be(8);
   }

   [Fact]
   public void AndThen_with_IInvoker_works() {
      var o = new Optional(new Foo(4))
          .AndThen<InvokerOpt, Foo>(default);

      o.Value!.X.Should().Be(8);
   }
   [Fact]
   public void AndThen_with_IStaticInvoker_works() {
      var o = new Optional(new Foo(4))
          .AndThen<StaticInvokerOpt, Foo>();

      o.Value!.X.Should().Be(8);
   }

   [Fact]
   public void Long_chain_option_does_not_throw() {
      var o = new Optional(new Foo(0));

      for (int i = 0; i < 5000; i++)
         o = o.Map(f => new Foo(f.X + 1));

      o.HasValue.Should().BeTrue();
      o.Value!.X.Should().Be(5000);
   }
}

// @RefOptional
public class RefOptional_Tests {
   [Fact]
   public void HasValue_when_constructed_with_value() {
      var o = new RefOptional(new RefFoo(42));
      o.HasValue.Should().BeTrue();
      o.Value.X.Should().Be(42);
   }

   [Fact]
   public void Default_has_no_value() {
      var o = default(RefOptional);
      o.HasValue.Should().BeFalse();
   }

   [Fact]
   public void Explicit_cast_throws_when_empty() {
      Action act = () => {
         var o = default(RefOptional);
         var _ = (RefFoo)o;
      };
      act.Should().Throw<InvalidAccess>();
   }
}

public class RefOptional_MapTests {
   [Fact]
   public void Map_applies_only_when_has_value() {
      var o = new RefOptional(new RefFoo(10))
          .Map(f => new RefFoo(f.X * 2));

      o.HasValue.Should().BeTrue();
      o.Value.X.Should().Be(20);
   }

   [Fact]
   public void Map_skips_when_empty() {
      var o = default(RefOptional)
          .Map(f => new RefFoo(f.X * 2));

      o.HasValue.Should().BeFalse();
   }
}

public class RefOptional_BindTests {
   static RefOptional Inc(RefFoo f)
       => new(new RefFoo(f.X + 1));

   [Fact]
   public void AndThen_left_identity() {
      var o = new RefOptional(new RefFoo(5))
          .AndThen(Inc);

      o.Value.X.Should().Be(6);
   }

   [Fact]
   public void AndThen_right_identity() {
      var o = new RefOptional(new RefFoo(7));
      var bound = o.AndThen(x => new RefOptional(x));

      bound.HasValue.Should().BeTrue();
      bound.Value.X.Should().Be(7);
   }

   [Fact]
   public void AndThen_skips_when_empty() {
      var o = default(RefOptional)
          .AndThen(Inc);

      o.HasValue.Should().BeFalse();
   }
}

public unsafe class RefOptional_DelegatePointerTests {
   static RefFoo Double(RefFoo f) => new RefFoo(f.X * 2);
   static RefOptional DoubleOpt(RefFoo f) => new(new RefFoo(f.X * 2));

   [Fact]
   public void Map_with_delegate_pointer_works() {
      var o = new RefOptional(new RefFoo(3))
          .Map(&Double);

      o.Value.X.Should().Be(6);
   }

   [Fact]
   public void AndThen_with_delegate_pointer_works() {
      var o = new RefOptional(new RefFoo(3))
          .AndThen(&DoubleOpt);

      o.Value.X.Should().Be(6);
   }
}

public class RefOptional_InvokerTests {
   struct Invoker : IInvoker<RefFoo, RefFoo> {
      public RefFoo Invoke(RefFoo f) => new RefFoo(f.X * 2);
   }
   struct StaticInvoker : IStaticInvoker<RefFoo, RefFoo> {
      public static RefFoo Invoke(RefFoo f) => new RefFoo(f.X * 2);
   }

   struct InvokerOpt : IInvoker<RefFoo, RefOptional> {
      public RefOptional Invoke(RefFoo f) => new(new RefFoo(f.X * 2));
   }
   struct StaticInvokerOpt : IStaticInvoker<RefFoo, RefOptional> {
      public static RefOptional Invoke(RefFoo f) => new(new RefFoo(f.X * 2));
   }

   [Fact]
   public void Map_with_IInvoker_works() {
      var o = new RefOptional(new RefFoo(4))
          .Map<Invoker, RefFoo>(default);

      o.Value.X.Should().Be(8);
   }
   [Fact]
   public void Map_with_IStaticInvoker_works() {
      var o = new RefOptional(new RefFoo(4))
          .Map<StaticInvoker, RefFoo>();

      o.Value.X.Should().Be(8);
   }

   [Fact]
   public void AndThen_with_IInvoker_works() {
      var o = new RefOptional(new RefFoo(4))
          .AndThen<InvokerOpt, RefFoo>(default);

      o.Value.X.Should().Be(8);
   }
   [Fact]
   public void AndThen_with_IStaticInvoker_works() {
      var o = new RefOptional(new RefFoo(4))
          .AndThen<StaticInvokerOpt, RefFoo>();

      o.Value.X.Should().Be(8);
   }

   [Fact]
   public void Long_chain_refoption_does_not_throw() {
      var o = new RefOptional(new RefFoo(0));

      for (int i = 0; i < 5000; i++)
         o = o.Map(f => new RefFoo(f.X + 1));

      o.HasValue.Should().BeTrue();
      o.Value.X.Should().Be(5000);
   }
}
