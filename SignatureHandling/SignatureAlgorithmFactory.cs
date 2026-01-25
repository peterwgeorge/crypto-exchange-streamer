namespace SignatureHandling;

using SignatureHandling.Interfaces;
using Credentials.Types;

public static class SignatureAlgorithmFactory
{
    public static ISignatureAlgorithm Create(ICredentialProvider p)
    {
        return p.GetAlgorithmString() switch
        {
            "ecdsa" => new EcdsaSignatureAlgorithm(p.GetSecretKey(), p.GetApiKeyName()),
            "ed25519" => new Ed25519SignatureAlgorithm(p.GetSecretKey(), p.GetApiKeyName()),
            _ => throw new NotSupportedException($"Unsupported algorithm: {p.GetAlgorithmString()}")
        };
    }
}