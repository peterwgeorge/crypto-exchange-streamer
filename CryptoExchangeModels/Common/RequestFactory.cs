namespace CryptoExchangeModels.Common;

using CryptoExchangeModels.Binance;
using CryptoExchangeModels.Btcc;
using CryptoExchangeModels.Coinbase;
using CryptoExchangeModels.Kraken;

public static class RequestFactory
{
    public static IExchangeRequest CreateSubscribeRequest(string exchange, string channel, string[] symbols)
    {
        switch(exchange.ToLower()){
            case "coinbase":
                return new CoinbaseRequest(MethodTypes.Subscribe, channel, symbols);
            case "kraken":
                return new KrakenRequest(MethodTypes.Subscribe, channel,  symbols);
            case "binance":
                return new BinanceRequest(MethodTypes.Subscribe.ToUpper(), symbols);
            case "btcc":
                return new BtccRequest("ReqSubcriV2", symbols);
            default:
                throw new ArgumentException($"{exchange} not supported.");
        }
    }
}