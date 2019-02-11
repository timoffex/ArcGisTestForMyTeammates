using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using MapServerTesting.ArcGIS.OriginDestination;

namespace MapServerTesting
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // My clientId and clientSecret are hidden. Get your own, you bum.
            // Follow the instructions on https://developers.arcgis.com/labs/rest/get-an-access-token/
            string clientId;
            string clientSecret;
            throw new Exception("Make an ArcGIS account (it's quick & free).");

            var accessToken = new ArcGIS.AccessToken.ArcGISToken(clientId, clientSecret);

            var ucscLocation = new Geometry { y = 36.992398, x = -122.059935 };
            var soquelLocation = new Geometry { y = 36.974227, x = -122.019266 };
            var laurelAndChestnut = new Geometry { y = 36.968732, x = -122.029347 };
            var felixArea = new Geometry { y = 36.966182, x = -122.032836 };

            // This creates a cost matrix request with origins = {soquel, laurel, felix}
            // and destinations = {laurel, felix, ucsc}. There are more properties
            // that can be specified. This particular snippet of code is meant to
            // mimic the input structure for the service. See the following for reference
            // https://developers.arcgis.com/rest/network/api-reference/origin-destination-cost-matrix-service.htm
            var request = new OriginDestinationRequest
            {
                Origins = new OriginsFeatures
                {
                    Features =
                    {
                        new OriginFeature
                        {
                            Geometry = soquelLocation,
                            Attributes = new OriginAttributes
                            {
                                Name = "Soquel House"
                            }
                        },

                        new OriginFeature
                        {
                            Geometry = laurelAndChestnut,
                            Attributes = new OriginAttributes
                            {
                                Name = "Laurel and Chestnut"
                            }
                        },

                        new OriginFeature
                        {
                            Geometry = felixArea,
                            Attributes = new OriginAttributes
                            {
                                Name = "Felix Area"
                            }
                        }
                    }
                },

                Destinations = new DestinationsFeatures
                {
                    Features =
                    {
                        new DestinationFeature
                        {
                            Geometry = laurelAndChestnut,
                            Attributes = new DestinationAttributes
                            {
                                Name = "Laurel and Chestnut"
                            }
                        },

                        new DestinationFeature
                        {
                            Geometry = felixArea,
                            Attributes = new DestinationAttributes
                            {
                                Name = "Felix Area"
                            }
                        },

                        new DestinationFeature
                        {
                            Geometry = ucscLocation,
                            Attributes = new DestinationAttributes
                            {
                                Name = "UCSC"
                            }
                        }
                    }
                },

                AccessToken = accessToken
            };


            Console.WriteLine(request);
            Console.WriteLine($"Token = {accessToken.Token}");

            var odcApi = new OriginDestinationApi();

            var result = odcApi.PerformQueryAsync(request).Result;

            StringBuilder resultBuilder = new StringBuilder();

            resultBuilder.AppendLine("Got result:");
            foreach (var feature in result.value.features)
            {
                var origin = feature.attributes.OriginName;
                var destination = feature.attributes.DestinationName;
                var time = feature.attributes.Total_Time;
                resultBuilder.AppendLine($"Time from '{origin}' to '{destination}' is {time}");
            }

            // Used by Startup class (this is bad design, of course).
            ComputedStuff = resultBuilder.ToString();
            Console.WriteLine(ComputedStuff);


            // For some reason I accidentally created a web project.
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        public static string ComputedStuff;
    }
}
