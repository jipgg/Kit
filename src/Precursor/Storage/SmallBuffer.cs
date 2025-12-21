namespace Precursor.Storage;

using static MethodImplOptions;

public interface ISmallBuffer<Self, T> where Self : ISmallBuffer<Self, T> {
   static abstract int Length { get; }
   static abstract Span<T> AsSpan(ref Self self);
   static abstract ReadOnlySpan<T> AsReadOnlySpan(ref readonly Self self);
}

[InlineArray(LengthAsConst)]
public struct DefaultSmallBuffer<T> : ISmallBuffer<DefaultSmallBuffer<T>, T> {
   internal T _data;
   internal const int LengthAsConst = 16;
   public static int Length => LengthAsConst;

   [MethodImpl(AggressiveInlining)]
   public static Span<T> AsSpan(ref DefaultSmallBuffer<T> b) => b;

   [MethodImpl(AggressiveInlining)]
   public static ReadOnlySpan<T> AsReadOnlySpan(ref readonly DefaultSmallBuffer<T> b) => b;
}

[InlineArray(LengthAsConst)]
public struct SmallBuffer8<T> : ISmallBuffer<SmallBuffer8<T>, T> {
   internal T _data;
   internal const int LengthAsConst = 8;
   public static int Length => LengthAsConst;

   [MethodImpl(AggressiveInlining)]
   public static Span<T> AsSpan(ref SmallBuffer8<T> b) => b;

   [MethodImpl(AggressiveInlining)]
   public static ReadOnlySpan<T> AsReadOnlySpan(ref readonly SmallBuffer8<T> b) => b;
}
[InlineArray(LengthAsConst)]
public struct SmallBuffer16<T> : ISmallBuffer<SmallBuffer16<T>, T> {
   internal T _data;
   internal const int LengthAsConst = 16;
   public static int Length => LengthAsConst;

   [MethodImpl(AggressiveInlining)]
   public static Span<T> AsSpan(ref SmallBuffer16<T> b) => b;

   [MethodImpl(AggressiveInlining)]
   public static ReadOnlySpan<T> AsReadOnlySpan(ref readonly SmallBuffer16<T> b) => b;
}
[InlineArray(LengthAsConst)]
public struct SmallBuffer4<T> : ISmallBuffer<SmallBuffer4<T>, T> {
   internal T _data;
   internal const int LengthAsConst = 4;
   public static int Length => LengthAsConst;

   [MethodImpl(AggressiveInlining)]
   public static Span<T> AsSpan(ref SmallBuffer4<T> b) => b;

   [MethodImpl(AggressiveInlining)]
   public static ReadOnlySpan<T> AsReadOnlySpan(ref readonly SmallBuffer4<T> b) => b;
}
[InlineArray(LengthAsConst)]
public struct SmallBuffer32<T> : ISmallBuffer<SmallBuffer32<T>, T> {
   internal T _data;
   internal const int LengthAsConst = 32;
   public static int Length => LengthAsConst;

   [MethodImpl(AggressiveInlining)]
   public static Span<T> AsSpan(ref SmallBuffer32<T> b) => b;

   [MethodImpl(AggressiveInlining)]
   public static ReadOnlySpan<T> AsReadOnlySpan(ref readonly SmallBuffer32<T> b) => b;
}
[InlineArray(LengthAsConst)]
public struct SmallBuffer64<T> : ISmallBuffer<SmallBuffer64<T>, T> {
   internal T _data;
   internal const int LengthAsConst = 64;
   public static int Length => LengthAsConst;

   [MethodImpl(AggressiveInlining)]
   public static Span<T> AsSpan(ref SmallBuffer64<T> b) => b;

   [MethodImpl(AggressiveInlining)]
   public static ReadOnlySpan<T> AsReadOnlySpan(ref readonly SmallBuffer64<T> b) => b;
}
