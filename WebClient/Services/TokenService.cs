using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System.Net.Http;

namespace WebClient.Services
{

    public class TokenService : ITokenService
    {
        private readonly IOptions<IdentityServerSettings> _identityServerSettings;
        private readonly DiscoveryDocumentResponse _discoveryDocument;
        public TokenService(IOptions<IdentityServerSettings> identityServerSettings)
        {
            _identityServerSettings = identityServerSettings;
            using var httpClient = new HttpClient();
            _discoveryDocument = httpClient.GetDiscoveryDocumentAsync(identityServerSettings.Value.DiscoveryUrl).Result;
        }
        public async Task<TokenResponse> GetToken(string scope)
        {
            using (var client = new HttpClient())
            {
                var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
                {
                    Address = _discoveryDocument.TokenEndpoint,
                    ClientId = _identityServerSettings.Value.ClientName,
                    ClientSecret = _identityServerSettings.Value.ClientPassword,
                    Scope = scope
                });
                if (tokenResponse.IsError)
                {
                    throw new Exception("Token Error");
                }
                return tokenResponse;
            }
        }
    }
}
