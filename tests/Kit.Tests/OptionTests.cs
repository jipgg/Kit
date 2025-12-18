using FluentAssertions;
namespace Kit.Tests;

using Option = Option<Foo>;
using RefOption = RefOption<RefFoo>;

public class OptionTests {
   [Fact]
   public void HasValue_when_constructed_with_value() {
      var o = new Option(new Foo(42));
      o.HasValue.Should().BeTrue();
      o.Value!.X.Should().Be(42);
   }

   [Fact]
   public void Default_has_no_value() {
      var o = default(Option);
      o.HasValue.Should().BeFalse();
   }

   [Fact]
   public void Explicit_cast_throws_when_empty() {
      var o = default(Option);
      Action act = () => { var _ = (Foo)o; };
      act.Should().Throw<InvalidOptionAccessException>();
   }
}

public class OptionMapTests {
   [Fact]
   public void Map_applies_only_when_has_value() {
      var o = new Option(new Foo(10))
          .Map(f => new Foo(f.X * 2));

      o.HasValue.Should().BeTrue();
      o.Value!.X.Should().Be(20);
   }

   [Fact]
   public void Map_skips_when_empty() {
      var o = default(Option)
          .Map(f => new Foo(f.X * 2));

      o.HasValue.Should().BeFalse();
   }
}

public class OptionBindTests {
   static Option Inc(Foo f)
       => new Option(new Foo(f.X + 1));

   [Fact]
   public void AndThen_left_identity() {
      var o = new Option(new Foo(5))
          .AndThen(Inc);

      o.HasValue.Should().BeTrue();
      o.Value!.X.Should().Be(6);
   }

   [Fact]
   public void AndThen_right_identity() {
      var o = new Option(new Foo(7));
      var bound = o.AndThen(x => new Option(x));

      bound.HasValue.Should().BeTrue();
      bound.Value!.X.Should().Be(7);
   }

   [Fact]
   public void AndThen_skips_when_empty() {
      var o = default(Option)
          .AndThen(Inc);

      o.HasValue.Should().BeFalse();
   }
}

public unsafe class OptionFunctionPointerTests {
   static Foo Double(Foo f) => new Foo(f.X * 2);
   static Option DoubleOpt(Foo f) => new Option(new Foo(f.X * 2));

   [Fact]
   public void Map_with_delegate_pointer_works() {
      var o = new Option(new Foo(3))
          .Map(&Double);

      o.Value!.X.Should().Be(6);
   }

   [Fact]
   public void AndThen_with_delegate_pointer_works() {
      var o = new Option(new Foo(3))
          .AndThen(&DoubleOpt);

      o.Value!.X.Should().Be(6);
   }
}

public class OptionInvocableTests {
   struct Invoker : IInvocable<Foo, Foo> {
      public Foo Invoke(Foo f) => new Foo(f.X * 2);
   }

   struct InvokerOpt : IInvocable<Foo, Option> {
      public Option Invoke(Foo f) => new(new Foo(f.X * 2));
   }

   [Fact]
   public void Map_with_IInvocable_works() {
      var o = new Option(new Foo(4))
          .Map<Invoker, Foo>(default);

      o.Value!.X.Should().Be(8);
   }

   [Fact]
   public void AndThen_with_IInvocable_works() {
      var o = new Option(new Foo(4))
          .AndThen<InvokerOpt, Foo>(default);

      o.Value!.X.Should().Be(8);
   }

   [Fact]
   public void Long_chain_option_does_not_throw() {
      var o = new Option(new Foo(0));

      for (int i = 0; i < 5000; i++)
         o = o.Map(f => new Foo(f.X + 1));

      o.HasValue.Should().BeTrue();
      o.Value!.X.Should().Be(5000);
   }
}

// @RefOption
public class RefOptionTests {
   [Fact]
   public void HasValue_when_constructed_with_value() {
      var o = new RefOption(new RefFoo(42));
      o.HasValue.Should().BeTrue();
      o.Value.X.Should().Be(42);
   }

   [Fact]
   public void Default_has_no_value() {
      var o = default(RefOption);
      o.HasValue.Should().BeFalse();
   }

   [Fact]
   public void Explicit_cast_throws_when_empty() {
      Action act = () => {
         var o = default(RefOption);
         var _ = (RefFoo)o;
      };
      act.Should().Throw<InvalidOptionAccessException>();
   }
}

public class RefOptionMapTests {
   [Fact]
   public void Map_applies_only_when_has_value() {
      var o = new RefOption(new RefFoo(10))
          .Map(f => new RefFoo(f.X * 2));

      o.HasValue.Should().BeTrue();
      o.Value.X.Should().Be(20);
   }

   [Fact]
   public void Map_skips_when_empty() {
      var o = default(RefOption)
          .Map(f => new RefFoo(f.X * 2));

      o.HasValue.Should().BeFalse();
   }
}

public class RefOptionBindTests {
   static RefOption Inc(RefFoo f)
       => new(new RefFoo(f.X + 1));

   [Fact]
   public void AndThen_left_identity() {
      var o = new RefOption(new RefFoo(5))
          .AndThen(Inc);

      o.Value.X.Should().Be(6);
   }

   [Fact]
   public void AndThen_right_identity() {
      var o = new RefOption(new RefFoo(7));
      var bound = o.AndThen(x => new RefOption(x));

      bound.HasValue.Should().BeTrue();
      bound.Value.X.Should().Be(7);
   }

   [Fact]
   public void AndThen_skips_when_empty() {
      var o = default(RefOption)
          .AndThen(Inc);

      o.HasValue.Should().BeFalse();
   }
}

public unsafe class RefOptionFunctionPointerTests {
   static RefFoo Double(RefFoo f) => new RefFoo(f.X * 2);
   static RefOption DoubleOpt(RefFoo f) => new(new RefFoo(f.X * 2));

   [Fact]
   public void Map_with_delegate_pointer_works() {
      var o = new RefOption(new RefFoo(3))
          .Map(&Double);

      o.Value.X.Should().Be(6);
   }

   [Fact]
   public void AndThen_with_delegate_pointer_works() {
      var o = new RefOption(new RefFoo(3))
          .AndThen(&DoubleOpt);

      o.Value.X.Should().Be(6);
   }
}

public class RefOptionInvocableTests {
   struct Invoker : IInvocable<RefFoo, RefFoo> {
      public RefFoo Invoke(RefFoo f) => new RefFoo(f.X * 2);
   }

   struct InvokerOpt : IInvocable<RefFoo, RefOption> {
      public RefOption Invoke(RefFoo f) => new(new RefFoo(f.X * 2));
   }

   [Fact]
   public void Map_with_IInvocable_works() {
      var o = new RefOption(new RefFoo(4))
          .Map<Invoker, RefFoo>(default);

      o.Value.X.Should().Be(8);
   }

   [Fact]
   public void AndThen_with_IInvocable_works() {
      var o = new RefOption(new RefFoo(4))
          .AndThen<InvokerOpt, RefFoo>(default);

      o.Value.X.Should().Be(8);
   }

   [Fact]
   public void Long_chain_refoption_does_not_throw() {
      var o = new RefOption(new RefFoo(0));

      for (int i = 0; i < 5000; i++)
         o = o.Map(f => new RefFoo(f.X + 1));

      o.HasValue.Should().BeTrue();
      o.Value.X.Should().Be(5000);
   }
}
