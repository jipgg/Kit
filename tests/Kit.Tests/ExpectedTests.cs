using FluentAssertions;
namespace Kit.Tests;

using Expected = Expected<Foo, Bar>;
using RefExpected = RefExpected<RefFoo, RefBar>;

public class ExpectedTests {
   [Fact]
   public void HasValue_when_constructed_with_value() {
      var e = new Expected(new Foo(42));
      e.HasValue.Should().BeTrue();
      e.HasError.Should().BeFalse();
      e.Value.X.Should().Be(42);
   }

   [Fact]
   public void HasError_when_constructed_with_unexpected() {
      var e = new Expected(new Unexpected<Bar>(new("err")));
      e.HasValue.Should().BeFalse();
      e.HasError.Should().BeTrue();
      e.Error.Msg.Should().Be("err");
   }

   [Fact]
   public void Default_is_error_with_default_payload() {
      var e = default(Expected);
      e.HasValue.Should().BeFalse();
      e.HasError.Should().BeTrue();
   }
   [Fact]
   public void Explicit_cast_throws_when_empty() {
      var o = default(Expected);
      Action act = () => { var _ = (Foo)o; };
      act.Should().Throw<InvalidExpectedAccessException>();
   }
}
public class ExpectedMapTests {
   [Fact]
   public void Map_applies_only_on_value() {
      var e = new Expected(new Foo(10))
          .Map(f => new Foo(f.X * 2));

      e.HasValue.Should().BeTrue();
      e.Value.X.Should().Be(20);
   }

   [Fact]
   public void Map_does_not_apply_on_error() {
      var e = new Expected(
          new Unexpected<Bar>(new("err"))
      ).Map(f => new Foo(f.X * 2));

      e.HasError.Should().BeTrue();
      e.Error.Msg.Should().Be("err");
   }

   [Fact]
   public void MapError_applies_only_on_error() {
      var e = new Expected(
          new Unexpected<Bar>(new Bar("abc"))
      ).MapError(err => new Bar("xyz"));

      e.HasError.Should().BeTrue();
      e.Error.Msg.Should().Be("xyz");
   }
}
public class ExpectedBindTests {
   static Expected Inc(Foo f)
       => new Expected(new Foo(f.X + 1));

   [Fact]
   public void AndThen_left_identity() {
      var e = new Expected(new Foo(5))
          .AndThen(Inc);

      e.Value.X.Should().Be(6);
   }

   [Fact]
   public void AndThen_right_identity() {
      var e = new Expected(new Foo(7));
      var bound = e.AndThen(x => new Expected(x));

      bound.HasValue.Should().BeTrue();
      bound.Value.X.Should().Be(7);
   }

   [Fact]
   public void OrElse_applies_on_error() {
      var e = new Expected(
          new Unexpected<Bar>(new("err"))
      ).OrElse(_ => new Expected(new Foo(99)));

      e.HasValue.Should().BeTrue();
      e.Value.X.Should().Be(99);
   }

   [Fact]
   public void OrElse_skips_on_value() {
      var e = new Expected(new Foo(1))
          .OrElse(_ => new Expected(new Foo(2)));

      e.Value.X.Should().Be(1);
   }
}
public unsafe class ExpectedFunctionPointerTests {
   static Foo Double(Foo f) => new Foo(f.X * 2);
   static Expected DoubleExp(Foo f)
       => new Expected(new Foo(f.X * 2));

   [Fact]
   public void Map_with_delegate_pointer_works() {
      var e = new Expected(new Foo(3))
          .Map(&Double);

      e.Value.X.Should().Be(6);
   }

   [Fact]
   public void AndThen_with_delegate_pointer_works() {
      var e = new Expected(new Foo(3))
          .AndThen(&DoubleExp);

      e.Value.X.Should().Be(6);
   }
}
public class ExpectedInvocableTests {
   struct Invoker : IInvocable<Foo, Foo> {
      public Foo Invoke(Foo f) => new Foo(f.X * 2);
   }
   struct InvokerExp : IInvocable<Foo, Expected> {
      public Expected Invoke(Foo f) => new(new Foo(f.X * 2));
   }

   [Fact]
   public void Map_with_IInvocable_works() {
      var e = new Expected(new Foo(4))
          .Map<Invoker, Foo>(default);

      e.Value.X.Should().Be(8);
   }

   [Fact]
   public void AndThen_with_IInvocable_works() {
      var e = new Expected(new Foo(4))
          .AndThen<InvokerExp, Foo>(default);
      e.Value.X.Should().Be(8);
   }

   [Fact]
   public void Long_chain_refexpected_does_not_throw() {
      var e = new Expected(new Foo(0));

      for (int i = 0; i < 5000; i++)
         e = e.Map(f => new Foo(f.X + 1));

      e.HasValue.Should().BeTrue();
      e.Value.X.Should().Be(5000);
   }
}
//@RefExpected
public class RefExpectedTests {
   [Fact]
   public void HasValue_when_constructed_with_value() {
      var e = new RefExpected(new RefFoo(42));
      e.HasValue.Should().BeTrue();
      e.HasError.Should().BeFalse();
      e.Value.X.Should().Be(42);
   }

   [Fact]
   public void HasError_when_constructed_with_unexpected() {
      var e = new RefExpected(
          new Unexpected<RefBar>(new("err"))
      );

      e.HasValue.Should().BeFalse();
      e.HasError.Should().BeTrue();
      e.Error.Msg.Should().Be("err");
   }

   [Fact]
   public void Default_is_error_with_default_payload() {
      var e = default(RefExpected);
      e.HasValue.Should().BeFalse();
      e.HasError.Should().BeTrue();
   }
   [Fact]
   public void Explicit_cast_throws_when_empty() {
      Action act = () => {
         var e = default(RefExpected);
         var _ = (RefFoo)e;
      };
      act.Should().Throw<InvalidExpectedAccessException>();
   }
}
public class RefExpectedMapTests {
   [Fact]
   public void Map_applies_only_on_value() {
      var e = new RefExpected(new RefFoo(10))
          .Map(f => new RefFoo(f.X * 2));

      e.HasValue.Should().BeTrue();
      e.Value.X.Should().Be(20);
   }

   [Fact]
   public void Map_does_not_apply_on_error() {
      var e = new RefExpected(
          new Unexpected<RefBar>(new("err"))
      ).Map(f => new RefFoo(f.X * 2));

      e.HasError.Should().BeTrue();
      e.Error.Msg.Should().Be("err");
   }

   [Fact]
   public void MapError_applies_only_on_error() {
      var e = new RefExpected(
          new Unexpected<RefBar>(new("abc"))
      ).MapError(err => new RefBar("xyz"));

      e.HasError.Should().BeTrue();
      e.Error.Msg.Should().Be("xyz");
   }
}
public class RefExpectedBindTests {
   static RefExpected Inc(RefFoo f) => new(new RefFoo(f.X + 1));

   [Fact]
   public void AndThen_left_identity() {
      var e = new RefExpected(new RefFoo(5))
          .AndThen(Inc);

      e.Value.X.Should().Be(6);
   }

   [Fact]
   public void AndThen_right_identity() {
      var e = new RefExpected(new RefFoo(7));
      var bound = e.AndThen(x => new RefExpected(x));

      bound.HasValue.Should().BeTrue();
      bound.Value.X.Should().Be(7);
   }

   [Fact]
   public void OrElse_applies_on_error() {
      var e = new RefExpected(
          new Unexpected<RefBar>(new("err"))
      ).OrElse(_ => new RefExpected(new RefFoo(99)));

      e.HasValue.Should().BeTrue();
      e.Value.X.Should().Be(99);
   }

   [Fact]
   public void OrElse_skips_on_value() {
      var e = new RefExpected(new RefFoo(1))
          .OrElse(_ => new RefExpected(new RefFoo(2)));

      e.Value.X.Should().Be(1);
   }
}
public unsafe class RefExpectedFunctionPointerTests {
   static RefFoo Double(RefFoo f) => new RefFoo(f.X * 2);
   static RefExpected DoubleExp(RefFoo f) => new(new RefFoo(f.X * 2));

   [Fact]
   public void Map_with_delegate_pointer_works() {
      var e = new RefExpected(new RefFoo(3))
          .Map(&Double);

      e.Value.X.Should().Be(6);
   }

   [Fact]
   public void AndThen_with_delegate_pointer_works() {
      var e = new RefExpected(new RefFoo(3))
          .AndThen(&DoubleExp);

      e.Value.X.Should().Be(6);
   }
}
public class RefExpectedInvocableTests {
   static RefFoo InvokeRef(RefFoo f) => new(f.X * 2);

   struct Invoker : IInvocable<RefFoo, RefFoo> {
      public RefFoo Invoke(RefFoo f) => new RefFoo(f.X * 2);
   }

   struct InvokerExp : IInvocable<RefFoo, RefExpected> {
      public RefExpected Invoke(RefFoo f)
          => new RefExpected(new RefFoo(f.X * 2));
   }

   [Fact]
   public void Map_with_IInvocable_works() {
      var e = new RefExpected(new RefFoo(4))
          .Map<Invoker, RefFoo>(default);

      e.Value.X.Should().Be(8);
   }

   [Fact]
   public void AndThen_with_IInvocable_works() {
      var e = new RefExpected(new RefFoo(4))
          .AndThen<InvokerExp, RefFoo>(default);
      e.Value.X.Should().Be(8);
   }

   [Fact]
   public void Long_chain_refexpected_does_not_throw() {
      var e = new RefExpected(new RefFoo(0));

      for (int i = 0; i < 5000; i++)
         e = e.Map(f => new RefFoo(f.X + 1));

      e.HasValue.Should().BeTrue();
      e.Value.X.Should().Be(5000);
   }
}
