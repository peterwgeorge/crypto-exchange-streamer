namespace CryptoExchangeModels.Btcc;

using CryptoExchangeModels.Common.Types;
using Newtonsoft.Json;

public class BtccRequest : IExchangeRequest
{
    [JsonProperty(PropertyName = "action")]
    public string Action;

    [JsonProperty(PropertyName = "symbols")]
    public string[] Symbols;

    [JsonProperty(PropertyName = "deep")]
    public string Deep;

    public BtccRequest(string action, string[] symbols)
    {
        Action = action;
        Symbols = symbols;
        Deep = symbols[0];
    }

}