namespace CryptoExchangeModels.Btcc;

using Newtonsoft.Json;

public class DealTickSnapshot
{
    [JsonProperty("S")]
    public string S { get; set; }

    [JsonProperty("Y")]
    public long Y { get; set; }

    [JsonProperty("ItemCount")]
    public int ItemCount { get; set; }

    [JsonProperty("Items")]
    public List<DealTickItem> Items { get; set; }
}
