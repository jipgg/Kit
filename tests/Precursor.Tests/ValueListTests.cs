using FluentAssertions;

namespace Precursor.Tests;

using IntList = ValueList<int, SmallBuffer8<int>>;

public class ValueListLaws {
   private static IntList From(params int[] xs) {
      var v = new IntList();
      foreach (var x in xs) v.Add(x);
      return v;
   }

   private static List<int> ModelFrom(params int[] xs) => xs.ToList();

   private static void AssertSequenceEqual(ref IntList sut, List<int> model) {
      sut.Count.Should().Be(model.Count);

      for (int i = 0; i < model.Count; i++) {
         sut[i].Should().Be(model[i], $"element at index {i} should match the model");
      }

      for (int i = 0; i < model.Count; i++) {
         var val = model[i];
         sut.Contains(val).Should().BeTrue();
         sut.IndexOf(val).Should().Be(model.IndexOf(val));
      }

      // non-membership checks (avoid accidental collisions)
      var missing = 0;
      while (model.Contains(missing)) missing++;
      sut.Contains(missing).Should().BeFalse();
      sut.IndexOf(missing).Should().Be(-1);
   }

   [Fact]
   public void Default_is_empty() {
      var v = new IntList();
      Assert.True(v.Count is 0);
      Assert.Empty(v.AsList());
   }

   [Theory]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(5)]
   [InlineData(10)]
   [InlineData(11)]
   [InlineData(25)]
   public void Add_preserves_order_and_count(int n) {
      var sut = new IntList();
      var model = new List<int>();

      for (int i = 0; i < n; i++) {
         sut.Add(i);
         model.Add(i);
      }

      AssertSequenceEqual(ref sut, model);
   }
   [Theory]
   [InlineData(5)]
   [InlineData(10)]
   [InlineData(11)]
   [InlineData(25)]
   public void AddRange_preserves_order_and_count(int n) {
      var sut = new IntList();
      var model = new List<int>();

      int[] arr = [.. Enumerable.Range(0, n)];

      sut.AddRange(arr.AsSpan());
      model.AddRange(arr.AsSpan());

      AssertSequenceEqual(ref sut, model);
   }

   [Theory]
   [InlineData(1)]
   [InlineData(10)]
   [InlineData(11)]
   [InlineData(32)]
   public void Indexer_set_updates_value_at_index(int n) {
      var sut = new IntList();
      var model = new List<int>();

      for (int i = 0; i < n; i++) { sut.Add(i); model.Add(i); }

      var mid = n / 2;
      sut[mid] = 12345;
      model[mid] = 12345;

      AssertSequenceEqual(ref sut, model);
   }

   [Theory]
   [InlineData(0, 0)]
   [InlineData(1, 0)]
   [InlineData(1, 1)]
   [InlineData(10, 0)]
   [InlineData(10, 5)]
   [InlineData(10, 10)]
   [InlineData(11, 0)]
   [InlineData(11, 5)]
   [InlineData(11, 11)]
   [InlineData(25, 12)]
   public void Insert_matches_List_semantics(int initialCount, int index) {
      var sut = new IntList();
      var model = new List<int>();
      for (int i = 0; i < initialCount; i++) { sut.Add(i); model.Add(i); }

      sut.Insert(index, 777);
      model.Insert(index, 777);

      AssertSequenceEqual(ref sut, model);
   }

   [Theory]
   [InlineData(1, 0)]
   [InlineData(2, 0)]
   [InlineData(2, 1)]
   [InlineData(10, 0)]
   [InlineData(10, 5)]
   [InlineData(10, 9)]
   [InlineData(11, 0)]
   [InlineData(11, 5)]
   [InlineData(11, 10)]
   [InlineData(25, 12)]
   public void RemoveAt_matches_List_semantics(int initialCount, int index) {
      var sut = new IntList();
      var model = new List<int>();

      for (int i = 0; i < initialCount; i++) { sut.Add(i); model.Add(i); }

      sut.RemoveAt(index);
      model.RemoveAt(index);

      AssertSequenceEqual(ref sut, model);
   }

   [Theory]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(10)]
   [InlineData(11)]
   [InlineData(25)]
   public void Remove_returns_false_when_item_not_present(int n) {
      var sut = new IntList();
      for (int i = 0; i < n; i++) sut.Add(i);

      Assert.False(sut.Remove(999_999_999));
   }

   [Theory]
   [InlineData(1)]
   [InlineData(10)]
   [InlineData(11)]
   [InlineData(25)]
   public void Remove_first_occurrence_matches_List_semantics_with_duplicates(int n) {
      var values = Enumerable.Range(0, n).SelectMany(x => new[] { x, x }).ToArray();

      var sut = new IntList();
      var model = new List<int>();
      foreach (var x in values) { sut.Add(x); model.Add(x); }

      var target = n / 2;

      var r1 = sut.Remove(target);
      var r2 = model.Remove(target);

      r1.Should().Be(r2);
      AssertSequenceEqual(ref sut, model);
   }

   [Theory]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(10)]
   [InlineData(11)]
   [InlineData(25)]
   public void Clear_results_in_empty_list(int n) {
      var sut = new IntList();
      for (int i = 0; i < n; i++) sut.Add(i);

      sut.Clear();

      Assert.True(sut.Count is 0, $"{sut.Count}");
      Assert.Empty(sut.AsList());

      // should still work after clear
      sut.Add(42);
      Assert.True(sut.Count is 1, $"{sut.Count}");
      Assert.True(sut[0] is 42, $"{sut[0]}");
   }

   [Theory]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(10)]
   [InlineData(11)]
   [InlineData(25)]
   public void AsList_is_a_snapshot_equal_to_enumerating_indices(int n) {
      var sut = new IntList();
      for (int i = 0; i < n; i++) sut.Add(i);

      var list = sut.AsList();

      list.Count.Should().Be(sut.Count);
      for (int i = 0; i < list.Count; i++)
         list[i].Should().Be(sut[i]);
   }

   [Theory]
   [InlineData(0)]
   [InlineData(1)]
   [InlineData(10)]
   [InlineData(11)]
   [InlineData(25)]
   public void Foreach_enumeration_matches_index_order(int n) {
      var sut = new IntList();
      for (int i = 0; i < n; i++) sut.Add(i);

      var seen = new List<int>();
      foreach (var x in sut)
         seen.Add(x);

      seen.Should().Equal(Enumerable.Range(0, n));
   }

   [Fact]
   public void Randomized_operations_match_List_reference_model() {
      var rng = new Random(123456);
      var sut = new IntList();
      var model = new List<int>();

      for (int step = 0; step < 5_000; step++) {
         var op = rng.Next(0, 6);

         switch (op) {
            case 0: {
                  var x = rng.Next(0, 100);
                  sut.Add(x);
                  model.Add(x);
                  break;
               }
            case 1: {
                  var x = rng.Next(0, 100);
                  var idx = model.Count == 0 ? 0 : rng.Next(0, model.Count + 1);
                  sut.Insert(idx, x);
                  model.Insert(idx, x);
                  break;
               }
            case 2: {
                  if (model.Count == 0) break;
                  var idx = rng.Next(0, model.Count);
                  sut.RemoveAt(idx);
                  model.RemoveAt(idx);
                  break;
               }
            case 3: {
                  var x = rng.Next(0, 100);
                  var r1 = sut.Remove(x);
                  var r2 = model.Remove(x);
                  r1.Should().Be(r2);
                  break;
               }
            case 4: {
                  if (model.Count == 0) break;
                  var idx = rng.Next(0, model.Count);
                  var x = rng.Next(0, 100);
                  sut[idx] = x;
                  model[idx] = x;
                  break;
               }
            case 5: {
                  if (rng.NextDouble() < 0.02) {
                     sut.Clear();
                     model.Clear();
                  }
                  break;
               }
         }

         AssertSequenceEqual(ref sut, model);
      }
   }
}
