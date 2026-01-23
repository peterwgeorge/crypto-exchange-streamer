using AmazonSecretsManagerHandler;

public class ExchangeConfig
{
    public required string Name { get; set; }
    public required string Channel { get; set; }
    public required List<string> Symbols { get; set; }
    public AuthenticationConfig? Authentication { get; set; }
}