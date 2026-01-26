namespace CryptoExchangeModels.Btcc;

using CryptoExchangeModels.Common.Types;
using Newtonsoft.Json;

public class BtccHeartbeat : IExchangeRequest
{
    [JsonProperty(PropertyName = "action")]
    public string Action;

    public BtccHeartbeat()
    {
        Action = "KeepLive";
    }

}