using System;
using System.Threading.Tasks;

namespace MapServerTesting.ArcGIS.AccessToken
{
    public class ArcGISToken
    {
        public ArcGISToken(string clientId, string clientSecret)
        {
            this.clientId = clientId;
            this.clientSecret = clientSecret;

            retrievedTokenExpiration = DateTime.MinValue;

            api = new AccessTokenApi();
        }

        /// <summary>
        /// Gets the token asynchronously. This will usually return immediately
        /// except when the previously retrieved token has expired.
        /// </summary>
        /// <returns>The token.</returns>
        public async Task<string> GetTokenAsync()
        {
            // Number of seconds to subtract from token expiration date to
            // ensure we don't return a token that is about to expire.
            int tolerance = 15;

            if (DateTime.Now < retrievedTokenExpiration)
                return retrievedToken;

            var response = await api.GetAccessTokenAsync(clientId, clientSecret);

            retrievedToken = response.access_token;
            retrievedTokenExpiration = DateTime.Now.AddSeconds(response.expires_in - tolerance);

            return retrievedToken;
        }

        /// <summary>
        /// Synchronously gets token. This calls GetTokenAsync() and may block.
        /// </summary>
        /// <value>The token.</value>
        public string Token
        {
            get => GetTokenAsync().Result;
        }

        private string retrievedToken;
        private DateTime retrievedTokenExpiration;

        private readonly string clientId;
        private readonly string clientSecret;
        private AccessTokenApi api;
    }
}
