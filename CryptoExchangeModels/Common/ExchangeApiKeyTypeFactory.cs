namespace CryptoExchangeModels.Common;

using CryptoExchangeModels.Binance;
using CryptoExchangeModels.Btcc;
using CryptoExchangeModels.Coinbase;
using CryptoExchangeModels.Kraken;
using CryptoExchangeModels.Common.Types;
using Credentials.Types;

public static class ExchangeApiKeyTypeFactory
{
    public static Type GetExchangeApiKeyType(ExchangeConfig c)
    {
        IExchangeRequest r;
        switch (c.Name.ToLower())
        {
            case "coinbase":
                return new CoinbaseApiKey().GetType();
            default:
                throw new ArgumentException($"{c.Name} not supported.");
        }
    }
}