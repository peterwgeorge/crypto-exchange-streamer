namespace CryptoExchangeModels.Common;

using CryptoExchangeModels.Binance;
using CryptoExchangeModels.Btcc;
using CryptoExchangeModels.Coinbase;
using CryptoExchangeModels.Kraken;
using CryptoExchangeModels.Common.Types;
using Credentials.Types;

public static class RequestFactory
{
    public static IExchangeRequest CreateSubscribeRequest(ExchangeConfig c, IKey k)
    {
        IExchangeRequest r;
        switch (c.Name.ToLower())
        {
            case "coinbase":
                return new CoinbaseRequest(MethodTypes.Subscribe, c, k);
            case "kraken":
                return new KrakenRequest(MethodTypes.Subscribe, c.Channel, c.Symbols.ToArray());
            case "binance":
                return new BinanceRequest(MethodTypes.Subscribe.ToUpper(), c.Symbols.ToArray());
            case "btcc":
                return new BtccRequest("ReqSubcriV2", c.Symbols.ToArray());
            default:
                throw new ArgumentException($"{c.Name} not supported.");
        }
    }
}