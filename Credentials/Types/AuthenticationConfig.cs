namespace Credentials.Types;

using Credentials.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

public class AuthenticationConfig
{
    [JsonConverter(typeof(StringEnumConverter))]
    public required CredentialProviders Provider { get; set; } = default!;

    // Cloud
    public string? SecretName { get; set; }
    public string? Region { get; set; }

    // Local
    public string? Location { get; set; }
}