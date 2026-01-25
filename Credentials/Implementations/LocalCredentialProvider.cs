namespace Credentials.Implementations;

using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Credentials.Types;

public class LocalCredentialProvider : ICredentialProvider
{
    private IKey _credentials;
    private Type _type;
    private AuthenticationConfig _config;
    private readonly object _lock = new object();
    private bool _initialized = false;
    
    public void Configure(AuthenticationConfig config, Type type)
    {
        _config = config;
        _type = type;
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
        try
        {
            string contents = await File.ReadAllTextAsync(_config.Location);
            var result = JsonConvert.DeserializeObject(contents, _type);

            if (result == null)
            {
                throw new InvalidOperationException($"Failed to deserialize credentials at {_config.Location}");
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