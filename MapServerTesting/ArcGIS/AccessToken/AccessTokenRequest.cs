using System;
namespace MapServerTesting.ArcGIS.AccessToken
{
    public struct AccessTokenRequest
    {
        public string client_id { get; set; }
        public string client_secret { get; set; }
        public string grant_type { get; set; }
    }
}
