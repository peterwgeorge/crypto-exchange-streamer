namespace SignatureHandling;

using SignatureHandling.Interfaces;
using Credentials.Types;
public static class SignatureAlgorithmFactory
{
    public static ISignatureAlgorithm Create(IKey key)
    {
        ISignatureAlgorithm algorithm;
        try
        {
            algorithm = new EcdsaSignatureAlgorithm(key.GetKey(), key.GetName());
            return algorithm;
        } 
        catch { }

        try
        {
            algorithm = new Ed25519SignatureAlgorithm(key.GetKey(), key.GetName());
            return algorithm;
        }
        catch { }

        throw new InvalidOperationException("Unknown key algorithm");
    } 
}