namespace Kit.Hashing;
using System.Security.Cryptography;

public class PasswordHasher<Traits> where Traits : IHashingTraits {
    public static readonly int Length = Traits.SaltLength + Traits.PassHashLength;
    public static readonly Range Salt = ..Traits.SaltLength;
    public static readonly Range Pass = Traits.SaltLength..;

    public static byte[] Hash(string password) {
        Span<byte> salt = stackalloc byte[Traits.SaltLength];
        RandomNumberGenerator.Fill(salt);

        return Hash(password, salt);
    }
    public static byte[] Hash(string password, ReadOnlySpan<byte> salt) {
        var hashArr = new byte[Length];
        var hash = hashArr.AsSpan();

        var saltPart = hash[Salt];
        salt.CopyTo(saltPart);

        var passPart = hash[Pass];
        EmplacePassHash(passPart, password, salt);

        return hashArr;
    }
    public static bool Verify(ReadOnlySpan<byte> hash, string password) {
        Span<byte> pass = stackalloc byte[Traits.PassHashLength];
        EmplacePassHash(pass, password, hash[Salt]);
        return hash[Pass].SequenceEqual(pass);
    }
    static void EmplacePassHash(Span<byte> passPart, string password, ReadOnlySpan<byte> saltPart) {
        Rfc2898DeriveBytes.Pbkdf2(password, saltPart, passPart, Traits.IterationCount, Traits.AlgorithmName);
    }
}

public class Hasher : PasswordHasher<DefaultHashingTraits>;
