namespace Credentials.Implementations;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Credentials.Types;

public class LocalCredentialProvider : ICredentialProvider
{
    private SigningMetadata? _credentials;
    private AuthenticationConfig _config;
    private readonly object _lock = new object();
    private bool _initialized = false;
    
    public void Configure(AuthenticationConfig config)
    {
        _config = config;
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
        try
        {
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