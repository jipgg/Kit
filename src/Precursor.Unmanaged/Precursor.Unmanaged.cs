global using System.Diagnostics;
global using System.Runtime.CompilerServices;
global using static System.Runtime.CompilerServices.MethodImplOptions;
global using System.Runtime.InteropServices;

static class Detail {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static public nuint ByteCount<T>(nuint count) where T : unmanaged => (nuint)Marshal.SizeOf<T>() * count;
}

