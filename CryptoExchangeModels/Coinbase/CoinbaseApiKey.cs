using Newtonsoft.Json;
using Credentials.Types;

namespace CryptoExchangeModels.Coinbase;

public class CoinbaseApiKey : IKey
{
    [JsonProperty(PropertyName = "name")]
    public string Name { get; set; }

    [JsonProperty(PropertyName = "privateKey")]
    public string PrivateKey { get; set; }

    public string GetName()
    {
        return Name;
    }

    public string GetKey()
    {
        return PrivateKey;
    }
}