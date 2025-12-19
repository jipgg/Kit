namespace Precursor.Unmanaged;

public sealed class Managed<T> : IDisposable, IAsyncDisposable where T : IDisposable {
   public T Resource { get; }

   public Managed(in T resource) {
      Resource = resource;
   }

   public bool IsDisposed { get; private set; }
   public void Dispose() {
      if (!IsDisposed) {
         IsDisposed = true;
         Resource.Dispose();
      }
   }
   public ValueTask DisposeAsync() {
      Dispose();
      return new();
   }
   ~Managed() => Dispose();
}
