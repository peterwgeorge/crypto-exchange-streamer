namespace CryptoExchangeModels.Btcc;

using Newtonsoft.Json;
public class DealTickItem
{
    [JsonProperty("P")]
    public double P { get; set; }

    [JsonProperty("V")]
    public double V { get; set; }

    [JsonProperty("T")]
    public long T { get; set; }
}