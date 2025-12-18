namespace Precursor.Unmanaged.Internal;

using System.Runtime.InteropServices;

static class Common {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   static public nuint ByteCount<T>(nuint count) where T : unmanaged => (nuint)Marshal.SizeOf<T>() * count;
}

