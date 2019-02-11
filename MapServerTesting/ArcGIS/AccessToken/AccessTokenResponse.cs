using System;
namespace MapServerTesting.ArcGIS.AccessToken
{
    public struct AccessTokenResponse
    {
        public string access_token { get; set; }
        public int expires_in { get; set; }
    }
}
