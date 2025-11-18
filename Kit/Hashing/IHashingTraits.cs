namespace Kit.Hashing;
using System.Security.Cryptography;

public interface IHashingTraits {
    abstract static int SaltLength { get; }
    abstract static int PassHashLength { get; }
    abstract static int IterationCount { get; }
    abstract static HashAlgorithmName AlgorithmName { get; }
}

public struct DefaultHashingTraits : IHashingTraits {
    public static int SaltLength => 16;
    public static int PassHashLength => 32;
    public static int IterationCount => 100_000;
    public static HashAlgorithmName AlgorithmName => HashAlgorithmName.SHA256;
}
