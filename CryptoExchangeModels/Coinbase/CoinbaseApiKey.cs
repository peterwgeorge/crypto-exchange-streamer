using Newtonsoft.Json;

namespace CryptoExchangeModels.Coinbase;

public class CoinbaseApiKey
{
    [JsonProperty(PropertyName = "name")]
    public required string Name { get; set; }

    [JsonProperty(PropertyName = "privateKey")]
    public required string PrivateKey { get; set; }
}