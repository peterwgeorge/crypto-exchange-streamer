namespace Credentials.Types;

public interface ICredentialProvider
{
    void Configure(AuthenticationConfig configuration, Type type);

    Task Initialize();

    Task<IKey> FetchCredentials();

    IKey GetCredentials();

}