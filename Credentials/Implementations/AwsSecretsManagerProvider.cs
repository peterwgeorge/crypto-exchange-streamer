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
    private SigningMetadata? _credentials;
    private readonly object _lock = new object();
    private bool _initialized = false;

    private string? _secretName;
    private string? _region;
    
    public void Configure(AuthenticationConfig configuration)
    {
        _secretName = configuration.SecretName;
        _region = configuration.Region;
        
        if (string.IsNullOrEmpty(_secretName) || string.IsNullOrEmpty(_region))
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

    private async Task<SigningMetadata> FetchCredentials()
    {
        IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(_region));
        GetSecretValueRequest request = new GetSecretValueRequest
        {
            SecretId = _secretName,
            VersionStage = "AWSCURRENT"
        };
        
        try
        {
            var response = await client.GetSecretValueAsync(request);
            string secretJson = response.SecretString;
            
            var result = JsonConvert.DeserializeObject<SigningMetadata>(secretJson);
            result.Secret = result.Secret.Replace("\\n", "\n");
            if (result == null)
            {
                throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
            }

            return result;
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error retrieving Coinbase credentials: {e.Message}");
            throw;
        }
    }

    
    public string GetAlgorithmString()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if(_credentials == null){
            throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
        }

        return _credentials.Algorithm;
    }

    public string GetApiKeyName()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if(_credentials == null){
            throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
        }

        return _credentials.KeyId;
    }

    public string GetSecretKey()
    {
        if (!_initialized)
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if(_credentials == null){
            throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
        }

        return _credentials.Secret;
    }

}