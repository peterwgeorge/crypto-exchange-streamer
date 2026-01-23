namespace AmazonSecretsManagerHandler;
public class AuthenticationConfig
{
    public required string Provider { get; set; } = default!;
    // "local", "aws-secrets-manager", "env", etc.

    // Cloud
    public string? SecretName { get; set; }
    public string? Region { get; set; }

    // Local
    public string? Location { get; set; }
}