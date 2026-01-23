namespace AmazonSecretsManagerHandler;

using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using AmazonSecretsManagerHandler.Models;

public static class SecretsProvider
{
    private static SigningMetadata? _credentials;
    private static Dictionary<string, AuthenticationConfig> _exchangeConfig;
    private static Dictionary<string, bool> _exchangeInitialized;
    private static Dictionary<string, FetchCredentialsMethod> _exchangeFetch;
    private static readonly object _lock = new object();
    
    public static void Configure(string exchange, AuthenticationConfig configuration)
    {
        switch (configuration.Provider)
        {
            case "aws-secrets-manager":
                _exchangeFetch[exchange] = FetchFromAwsSecretsManager;
                break;
            case "local":
                break;
            default:
                throw new ArgumentException($"{configuration.Provider} is not currently supported");
        }

        _exchangeInitialized[exchange] = false;
        _exchangeConfig[exchange] = configuration;
    }

    public static async Task Initialize(string exchange)
    {
        if (!_exchangeInitialized[exchange])
        {
            var credentials = await _exchangeFetch[exchange](exchange);
            lock (_lock)
            {
                _credentials = credentials;
                _exchangeInitialized[exchange] = true;
            }
        }
    }

    private static async Task<SigningMetadata> FetchFromAwsSecretsManager(string exchange)
    {
        string? secretName = _exchangeConfig[exchange].SecretName;
        string? region = _exchangeConfig[exchange].Region;

        if (string.IsNullOrWhiteSpace(secretName) || string.IsNullOrWhiteSpace(region))
            throw new InvalidOperationException("SecretName or Region was invalid");

        IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));
        GetSecretValueRequest request = new GetSecretValueRequest
        {
            SecretId = secretName,
            VersionStage = "AWSCURRENT"
        };
        
        try
        {
            var response = await client.GetSecretValueAsync(request);
            string secretJson = response.SecretString;
            
            var result = JsonConvert.DeserializeObject<SigningMetadata>(secretJson);
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

    
    public static string GetAlgorithmString(string exchange)
    {
        if (!_exchangeInitialized[exchange])
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if(_credentials == null){
            throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
        }

        return _credentials.Algorithm;
    }

    public static string GetApiKeyName(string exchange)
    {
        if (!_exchangeInitialized[exchange])
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if(_credentials == null){
            throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
        }

        return _credentials.KeyId;
    }

    public static string GetSecretKey(string exchange)
    {
        if (!_exchangeInitialized[exchange])
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if(_credentials == null){
            throw new InvalidOperationException("Failed to deserialize Coinbase credentials");
        }

        return _credentials.Secret;
    }

}