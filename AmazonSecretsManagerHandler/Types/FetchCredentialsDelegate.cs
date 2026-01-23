using AmazonSecretsManagerHandler.Models;

namespace AmazonSecretsManagerHandler;

public delegate Task<SigningMetadata> FetchCredentialsMethod(string exchange);