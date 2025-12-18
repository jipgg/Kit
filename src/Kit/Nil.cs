namespace Kit;

public readonly struct Nil {
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool operator true(Nil n) => false;
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool operator false(Nil n) => true;
   [MethodImpl(MethodImplOptions.AggressiveInlining)]
   public static bool operator !(Nil n) => true;

   public readonly static Nil nil = new();
}

public sealed class NilValueException(string? message = null) : NullReferenceException(message);
