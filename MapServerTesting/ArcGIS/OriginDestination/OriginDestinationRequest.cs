using System;
using System.Text;
using System.Collections.Generic;
using RestSharp;
using MapServerTesting.ArcGIS.AccessToken;
using Newtonsoft.Json;

namespace MapServerTesting.ArcGIS.OriginDestination
{
    public class OriginDestinationRequest
    {
        public void AddToRequest(IRestRequest request)
        {
            // Ignore null values during serialization.
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            request.AddParameter("origins", JsonConvert.SerializeObject(Origins, settings));
            request.AddParameter("destinations", JsonConvert.SerializeObject(Destinations, settings));
            request.AddParameter("token", AccessToken.Token);
            request.AddParameter("f", "json");
        }

        public override string ToString()
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };

            return
                  $"Origins = {JsonConvert.SerializeObject(Origins, settings)}\n"
                + $"Destinations = {JsonConvert.SerializeObject(Destinations, settings)}\n"
                + $"Token = {AccessToken.Token}";
        }

        public Origins Origins { get; set; }
        public Destinations Destinations { get; set; }
        public ArcGISToken AccessToken { get; set; }
    }

    public abstract class Origins
    {
    }

    public abstract class Destinations
    {
    }

    public class OriginsFeatures : Origins
    {
        [JsonProperty(PropertyName = "features")]
        public IList<OriginFeature> Features { get; } = new List<OriginFeature>();
    }

    public class DestinationsFeatures : Destinations
    {
        [JsonProperty(PropertyName = "features")]
        public IList<DestinationFeature> Features { get; } = new List<DestinationFeature>();
    }

    public abstract class Feature
    {
        [JsonProperty(PropertyName = "geometry")]
        public Geometry Geometry { get; set; }
    }

    public class Geometry
    {
        public double x;
        public double y;
    }

    public class OriginFeature : Feature
    {
        [JsonProperty(PropertyName = "attributes")]
        public OriginAttributes Attributes { get; set; }
    }

    public class DestinationFeature : Feature
    {
        [JsonProperty(PropertyName = "attributes")]
        public DestinationAttributes Attributes { get; set; }
    }

    public abstract class FeatureAttribute
    {
        public string Name { get; set; } = null;
        public CurbApproachType CurbApproach { get; set; } = CurbApproachType.RightSide;

        public enum CurbApproachType
        {
            EitherSide, RightSide, LeftSide
        }
    }

    public class OriginAttributes : FeatureAttribute
    {
        public int? TargetDestinationCount { get; set; } = null;
        public double? Cutoff { get; set; } = null;
    }

    public class DestinationAttributes : FeatureAttribute
    {
        // Doesn't add anything.
    }
}
