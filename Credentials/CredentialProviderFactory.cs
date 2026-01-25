namespace Credentials;

using Credentials.Types;
using Credentials.Enums;
using Credentials.Implementations;

public static class CredentialProviderFactory
{
    public static ICredentialProvider GetProvider(CredentialProviders? provider){
        switch (provider){
            case CredentialProviders.AwsSecretsManager:
                return new AwsSecretsManagerProvider();
            case CredentialProviders.Local:
                throw new NotImplementedException();
            default:
                throw new NotImplementedException();
        }
    }
}