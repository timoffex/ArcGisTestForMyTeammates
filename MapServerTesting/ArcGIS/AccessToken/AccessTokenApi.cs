using RestSharp;
using System.Threading.Tasks;
using System;

namespace MapServerTesting.ArcGIS.AccessToken
{
    public class AccessTokenApi
    {
        public AccessTokenApi()
        {
            client = new RestClient("https://www.arcgis.com/sharing/rest/oauth2/token");
            client.AddHandler("text/plain", new RestSharp.Serialization.Json.JsonDeserializer());
        }

        public async Task<AccessTokenResponse> GetAccessTokenAsync(string clientId, string clientSecret)
        {
            var request = new RestRequest(Method.POST);
            request.AddObject(new AccessTokenRequest
            {
                client_id = clientId,
                client_secret = clientSecret,
                grant_type = "client_credentials"
            }, "client_id", "client_secret", "grant_type");

            var response = await client.ExecuteTaskAsync<AccessTokenResponse>(request);

            if (response.ErrorException != null)
            {
                throw new ApplicationException("Error getting access token.", response.ErrorException);
            }

            return response.Data;
        }

        private RestClient client;
    }
}

