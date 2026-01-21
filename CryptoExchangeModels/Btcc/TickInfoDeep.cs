namespace CryptoExchangeModels.Btcc;

using Newtonsoft.Json;

public class TickInfoDeep
{
    [JsonProperty("S")]
    public string S { get; set; }

    [JsonProperty("Y")]
    public string Y { get; set; }

    [JsonProperty("A")]
    public List<string> A { get; set; }

    [JsonProperty("B")]
    public List<string> B { get; set; }

    [JsonProperty("U")]
    public List<string> U { get; set; }

    [JsonProperty("M")]
    public List<string> M { get; set; }

    [JsonProperty("T")]
    public long T { get; set; }

    [JsonProperty("V")]
    public string V { get; set; }

    [JsonProperty("C")]
    public string C { get; set; }

    [JsonProperty("L")]
    public List<string> L { get; set; }

    [JsonProperty("I")]
    public List<string> I { get; set; }

    [JsonProperty("R")]
    public List<string> R { get; set; }

    [JsonProperty("E")]
    public List<string> E { get; set; }
}
