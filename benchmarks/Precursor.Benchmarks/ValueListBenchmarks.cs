using BenchmarkDotNet.Attributes;
using System.Runtime.CompilerServices;
using BenchmarkDotNet.Order;
using System.Collections.Immutable;
using Precursor.Collections;
namespace Precursor.Benchmarks.ValueListBenchmarks;

[InlineArray(64)]
public struct Byte64 : IEquatable<Byte64> {
   byte _data;
   public bool Equals(Byte64 other) => ((Span<byte>)this).SequenceEqual(other);
}

public sealed class Class8 : IEquatable<Class8> {
   readonly byte _data = default;
   public bool Equals(Class8? other)
      => other is null ? false : _data == other._data;
}

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.Default)]
[GenericTypeArguments(typeof(int))]
[GenericTypeArguments(typeof(double))]
[GenericTypeArguments(typeof(Class8))]
public class ValueListAdd<T> where T : IEquatable<T>, new() {

   [Params(8, 16, 64)]
   public int N;

   ImmutableArray<T> _range;
   ReadOnlySpan<T> Span => _range.AsSpan();
   [GlobalSetup]
   public void Setup() {
      var builder = ImmutableArray.CreateBuilder<T>(N);
      for (int i = 0; i < N; i++)
         builder.Add(new());

      _range = builder.MoveToImmutable();
   }

   [Benchmark]
   public int List_Add() {
      var list = new List<T>();
      for (int i = 0; i < N; ++i) list.Add(new());
      return list.Count;
   }
   [Benchmark]
   public int ValueList_Add() {
      var list = new ValueList<T>();
      for (int i = 0; i < N; ++i) list.Add(new());
      return list.Count;
   }
   [Benchmark]
   public int List_CapacityN_Add() {
      var list = new List<T>(N);
      for (int i = 0; i < N; ++i) list.Add(new());
      return list.Count;
   }

   [Benchmark]
   public int List_AddRange_Span() {
      var list = new List<T>();
      list.AddRange(Span);
      return list.Count;
   }
   [Benchmark]
   public int ValueList_AddRange_Span() {
      var list = new ValueList<T>();
      list.AddRange(Span);
      return list.Count;
   }
}
