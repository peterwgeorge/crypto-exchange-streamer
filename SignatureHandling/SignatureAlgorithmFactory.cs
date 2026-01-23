namespace SignatureHandling;

using SignatureHandling.Interfaces;
using AmazonSecretsManagerHandler;

public static class SignatureAlgorithmFactory
{
    public static ISignatureAlgorithm Create(string exchange)
    {
        return SecretsProvider.GetAlgorithmString(exchange) switch
        {
            "ecdsa" => new EcdsaSignatureAlgorithm(SecretsProvider.GetSecretKey(exchange), SecretsProvider.GetApiKeyName(exchange)),
            "ed25519" => new Ed25519SignatureAlgorithm(SecretsProvider.GetSecretKey(exchange), SecretsProvider.GetApiKeyName(exchange)),
            _ => throw new NotSupportedException($"Unsupported algorithm: {SecretsProvider.GetAlgorithmString(exchange)}")
        };
    }
}