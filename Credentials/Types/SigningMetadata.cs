namespace Credentials.Types;

using Newtonsoft.Json;

public class SigningMetadata
{
    [JsonProperty(PropertyName = "algorithm")]
    public required string Algorithm { get; set; }

    [JsonProperty(PropertyName = "secret")]
    public required string Secret { get; set; }

    [JsonProperty(PropertyName = "keyId")]
    public required string KeyId { get; set; }
}