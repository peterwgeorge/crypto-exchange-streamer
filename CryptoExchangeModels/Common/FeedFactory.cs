namespace CryptoExchangeModels.Common;

using CryptoExchangeModels.Binance;
using CryptoExchangeModels.Btcc;
using CryptoExchangeModels.Coinbase;
using CryptoExchangeModels.Kraken;

public static class FeedFactory
{
    public static string GetExchangeFeed(string exchange)
    {
        switch(exchange.ToLower()){
            case "coinbase":
                return CoinbaseMarketDataFeeds.MarketDataEndpoint;
            case "kraken":
                return KrakenMarketDataFeeds.Endpoint;
            case "binance":
                return BinanceMarketDataFeeds.BaseEndpoint;
            case "btcc":
                return BtccMarketDataFeeds.MarketDataEndpoint;
            default:
                throw new ArgumentException($"{exchange} not supported.");
        }
    }
}