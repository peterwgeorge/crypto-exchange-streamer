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
    private static Dictionary<string,SigningMetadata> _credentials = new();
    private static Dictionary<string, AuthenticationConfig> _config = new();
    private static Dictionary<string, bool> _initialized = new();
    private static Dictionary<string, FetchCredentialsMethod> _fetch = new();
    private static readonly object _lock = new object();
    
    public static void Configure(string exchange, AuthenticationConfig configuration)
    {
        lock (_lock)
        {
            switch (configuration.Provider)
            {
                case "aws-secrets-manager":
                    _fetch[exchange] = FetchFromAwsSecretsManager;
                    break;
                case "local":
                    break;
                default:
                    throw new ArgumentException($"{configuration.Provider} is not currently supported");
            }

            _initialized[exchange] = false;
            _config[exchange] = configuration;
        }

    }

    public static async Task Initialize(string exchange)
    {
        bool needsInit;
        lock (_lock)
        {
            needsInit = !_initialized.TryGetValue(exchange, out var init) || !init;
        }

        if (!needsInit)
            return;

        var credentials = await _fetch[exchange](exchange);

        lock (_lock)
        {
            _credentials[exchange] = credentials;
            _initialized[exchange] = true;
        }
    }


    private static async Task<SigningMetadata> FetchFromAwsSecretsManager(string exchange)
    {
        string? secretName = _config[exchange].SecretName;
        string? region = _config[exchange].Region;

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
        if (!_initialized[exchange])
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if (!_credentials.TryGetValue(exchange, out var creds))
        {
            throw new InvalidOperationException($"No credentials loaded for exchange '{exchange}'");
        }

        return creds.Algorithm;
    }

    public static string GetApiKeyName(string exchange)
    {
        if (!_initialized[exchange])
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if (!_credentials.TryGetValue(exchange, out var creds))
        {
            throw new InvalidOperationException($"No credentials loaded for exchange '{exchange}'");
        }

        return creds.KeyId;
    }

    public static string GetSecretKey(string exchange)
    {
        if (!_initialized[exchange])
        {
            throw new InvalidOperationException("SecretsProvider has not been initialized. Call Initialize() first.");
        }

        if (!_credentials.TryGetValue(exchange, out var creds))
        {
            throw new InvalidOperationException($"No credentials loaded for exchange '{exchange}'");
        }

        return creds.Secret;
    }

}