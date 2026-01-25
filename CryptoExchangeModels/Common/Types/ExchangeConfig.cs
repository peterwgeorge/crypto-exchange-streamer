namespace CryptoExchangeModels.Common.Types;

using Credentials.Types;

public class ExchangeConfig
{
    public required string Name { get; set; }
    public required string Channel { get; set; }
    public required List<string> Symbols { get; set; }
    public AuthenticationConfig? Authentication { get; set; }

    public ExchangeConfig()
    {

    }

    public ExchangeConfig(string name, string channel, List<string> symbols)
    {
        Name = name;
        Channel = channel;
        Symbols = symbols;
    }
}
