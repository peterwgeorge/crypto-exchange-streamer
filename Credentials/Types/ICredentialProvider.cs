namespace Credentials.Types;

public interface ICredentialProvider
{
    void Configure(AuthenticationConfig configuration);

    Task Initialize();

    string GetAlgorithmString();

    string GetApiKeyName();

    string GetSecretKey();

}