namespace CryptoExchangeModels.Coinbase;

using System.Security.Cryptography;
using Newtonsoft.Json;
using SignatureHandling;
using SignatureHandling.Interfaces;
using CryptoExchangeModels.Common.Types;
using Credentials.Types;

public class CoinbaseRequest : IExchangeRequest
{
    [JsonProperty(PropertyName = "type")]
    public string Type { get; set; }

    [JsonProperty(PropertyName = "channel")]
    public string Channel { get; set; }

    [JsonProperty(PropertyName = "product_ids")]
    public string[] ProductIds { get; set; }
    
    [JsonProperty(PropertyName = "jwt")]
    public string Jwt { get; private set; }

    public CoinbaseRequest(string type, ExchangeConfig c, ICredentialProvider p)
    {
        Type = type;
        Channel = c.Channel;
        ProductIds = c.Symbols.ToArray();
        Jwt = GenerateToken(p);
    }

    static string GenerateToken(ICredentialProvider p)
    {
        ISignatureAlgorithm algo = SignatureAlgorithmFactory.Create(p);
        var payload = new Dictionary<string, object>
        {
            { "sub", p.GetApiKeyName()},
            { "iss", "coinbase-cloud" },
            { "nbf", Convert.ToInt64((DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds) },
            { "exp", Convert.ToInt64((DateTime.UtcNow.AddMinutes(1) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds) },
        };

        var extraHeaders = new Dictionary<string, object>
        {
            // add nonce to prevent replay attacks with a random 10 digit number
            { "nonce", RandomHex(10) },
        };

        return algo.SignJwt(payload, extraHeaders);
    }

    static string RandomHex(int digits) {
        using(var random = RandomNumberGenerator.Create()){
            byte[] buffer = new byte[digits / 2];
            random.GetBytes(buffer);
            string result = String.Concat(buffer.Select(x => x.ToString("X2")).ToArray());
            if (digits % 2 == 0)
                return result;
            return result + RandomNumberGenerator.GetInt32(16).ToString("X");
        }
    }
}