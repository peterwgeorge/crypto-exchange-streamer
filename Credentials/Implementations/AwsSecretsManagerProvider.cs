namespace Credentials.Implementations;

using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Credentials.Types;

public class AwsSecretsManagerProvider : ICredentialProvider
{
    private AuthenticationConfig _config;
    private IKey _credentials;
    private Type _type;
    private readonly object _lock = new object();
    private bool _initialized = false;
    
    public void Configure(AuthenticationConfig configuration, Type type)
    {
        _config = configuration;
        _type = type;
        if (string.IsNullOrEmpty(_config.SecretName) || string.IsNullOrEmpty(_config.Region))
        {
            throw new InvalidOperationException("SecretsManager configuration missing. Check appsettings.json.");
        }
    }

    public async Task Initialize()
    {
        if (!_initialized)
        {
            var credentials = await FetchCredentials();
            lock (_lock)
            {
                _credentials = credentials;
                _initialized = true;
            }
        }
    }

    public async Task<IKey> FetchCredentials()
    {
        IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(_config.Region));
        GetSecretValueRequest request = new GetSecretValueRequest
        {
            SecretId = _config.SecretName,
            VersionStage = "AWSCURRENT"
        };
        
        try
        {
            var response = await client.GetSecretValueAsync(request);
            string secretJson = response.SecretString;
            var result = JsonConvert.DeserializeObject(secretJson, _type);
            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize secret");
            }

            return result as IKey;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error retrieving credentials: {e.Message}");
            throw;
        }
    }

    public IKey GetCredentials()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException($"{_config.Provider.ToString()} provider has not been initialized. Call Initialize() first.");
        }

        if(_credentials == null){
            throw new InvalidOperationException($"Failed to deserialize {_config.Provider.ToString()} credentials");
        }

        return _credentials;
    }
}