namespace CryptoExchangeModels.Btcc;

using Newtonsoft.Json;
using System.Collections.Generic;

public class BtccResponse<T>
{
    [JsonProperty("action")]
    public string Action { get; set; }

    [JsonProperty("data")]
    public List<T> Data { get; set; }
}
