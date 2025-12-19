using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Order;
using Precursor;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Bench_ints_without_spill {
   readonly static int N = SmallBuffer10<int>.Length;

   [Benchmark]
   public int List_Add() {
      var list = new List<int>(SmallBuffer10<int>.Length);
      for (int i = 0; i < N; ++i)
         list.Add(i);
      return list.Count;
   }

   [Benchmark]
   public int ValueList_Add() {
      var list = new ValueList<int, SmallBuffer10<int>>();
      for (int i = 0; i < N; ++i)
         list.Add(i);
      return list.Count;
   }
}
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Bench_ints_with_spill {
   readonly static int N = SmallBuffer10<int>.Length * 50;

   [Benchmark]
   public int List_Add() {
      var list = new List<int>(SmallBuffer10<int>.Length);
      for (int i = 0; i < N; ++i)
         list.Add(i);
      return list.Count;
   }

   [Benchmark]
   public int ValueList_Add() {
      var list = new ValueList<int, SmallBuffer10<int>>();
      for (int i = 0; i < N; ++i)
         list.Add(i);
      return list.Count;
   }
}

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Bench_big_struct_without_spill {
   readonly static int N = SmallBuffer10<BigStruct>.Length;

   [InlineArray(100)]
   struct BigStruct : IEquatable<BigStruct> {
      byte _data;
      public bool Equals(BigStruct other) { throw new(); }
   }
   [Benchmark]
   public int List_Add() {
      var list = new List<BigStruct>(SmallBuffer10<BigStruct>.Length);
      for (int i = 0; i < N; ++i)
         list.Add(default);
      return list.Count;
   }

   [Benchmark]
   public int ValueList_Add() {
      var list = new ValueList<BigStruct, SmallBuffer10<BigStruct>>();
      for (int i = 0; i < N; ++i)
         list.Add(default);
      return list.Count;
   }
}

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Bench_big_struct_with_spill {
   readonly static int N = SmallBuffer10<BigStruct>.Length * 50;

   [InlineArray(64)]
   struct BigStruct : IEquatable<BigStruct> {
      byte _data;
      public bool Equals(BigStruct other) { throw new(); }
   }
   [Benchmark]
   public int List_Add() {
      var list = new List<BigStruct>(SmallBuffer10<BigStruct>.Length);
      for (int i = 0; i < N; ++i)
         list.Add(default);
      return list.Count;
   }

   [Benchmark]
   public int ValueList_Add() {
      var list = new ValueList<BigStruct, SmallBuffer10<BigStruct>>();
      for (int i = 0; i < N; ++i)
         list.Add(default);
      return list.Count;
   }
}

// weirdly enough the wrapper comes out on top consistently
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class ValueListTS_vs_ValueListT {
   readonly static int N = SmallBuffer10<long>.Length;

   [Benchmark]
   public int ValueListTS_Add() {
      var list = new ValueList<long, SmallBuffer10<long>>();
      for (int i = 0; i < N; ++i) list.Add(default);
      return list.Count;
   }
   [Benchmark]
   public int ValueListT_Add() {
      var list = new ValueList<long>();
      for (int i = 0; i < N; ++i) list.Add(default);
      return list.Count;
   }
}
