using Kit.Unmanaged;
using Kit;
using static Kit.Result;

Span<int> a = stackalloc int[] { 1, 2, 3, 4, 5 };

var rr = Wrap<int, InvalidOperationException>(() => {
    if (true) return 1;
    else throw new InvalidOperationException();
});
var rrr = Wrap(() => 1);
Result<int, string> r = Ok(1);
var x = r.AndThen(o => Err(o.ToString()))
    .Peek(Console.WriteLine)
    .PeekError(Console.WriteLine)
    .Unwrap();

var myvec = new VectorList<double>(10);


myvec.AddRange(stackalloc double[] { 1, 2, 3 });
myvec.Reserve(10000000);
Console.WriteLine($"CAPACITY IS {myvec.Capacity} SIZE IS {myvec.Count}");
myvec.ShrinkToFit();
Console.WriteLine($"CAPACITY IS {myvec.Capacity}");
foreach (ref var val in myvec) {
    Console.WriteLine($"val is {val}");
}
