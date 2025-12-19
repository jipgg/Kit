using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Order;
using Precursor;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Bench_ints_without_spill {
   readonly static int N = DefaultSmallBuffer<int>.Length;

   [Benchmark]
   public int List_Add() {
      var list = new List<int>(DefaultSmallBuffer<int>.Length);
      for (int i = 0; i < N; i++)
         list.Add(i);
      return list.Count;
   }

   [Benchmark]
   public int ValueList_Add() {
      var list = new ValueList<int, DefaultSmallBuffer<int>>();
      for (int i = 0; i < N; i++)
         list.Add(i);
      return list.Count;
   }
}
[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Bench_ints_with_spill {
   readonly static int N = DefaultSmallBuffer<int>.Length * 50;

   [Benchmark]
   public int List_Add() {
      var list = new List<int>(DefaultSmallBuffer<int>.Length);
      for (int i = 0; i < N; i++)
         list.Add(i);
      return list.Count;
   }

   [Benchmark]
   public int ValueList_Add() {
      var list = new ValueList<int, DefaultSmallBuffer<int>>();
      for (int i = 0; i < N; i++)
         list.Add(i);
      return list.Count;
   }
}

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Bench_big_struct_without_spill {
   readonly static int N = DefaultSmallBuffer<BigStruct>.Length;

   [InlineArray(100)]
   struct BigStruct : IEquatable<BigStruct> {
      byte _data;
      public bool Equals(BigStruct other) { throw new(); }
   }
   [Benchmark]
   public int List_Add() {
      var list = new List<BigStruct>(DefaultSmallBuffer<BigStruct>.Length);
      for (int i = 0; i < N; i++)
         list.Add(default);
      return list.Count;
   }

   [Benchmark]
   public int ValueList_Add() {
      var list = new ValueList<BigStruct, DefaultSmallBuffer<BigStruct>>();
      for (int i = 0; i < N; i++)
         list.Add(default);
      return list.Count;
   }
}

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class Bench_big_struct_with_spill {
   readonly static int N = DefaultSmallBuffer<BigStruct>.Length * 50;

   [InlineArray(64)]
   struct BigStruct : IEquatable<BigStruct> {
      byte _data;
      public bool Equals(BigStruct other) { throw new(); }
   }
   [Benchmark]
   public int List_Add() {
      var list = new List<BigStruct>(DefaultSmallBuffer<BigStruct>.Length);
      for (int i = 0; i < N; i++)
         list.Add(default);
      return list.Count;
   }

   [Benchmark]
   public int ValueList_Add() {
      var list = new ValueList<BigStruct, DefaultSmallBuffer<BigStruct>>();
      for (int i = 0; i < N; i++)
         list.Add(default);
      return list.Count;
   }
}
