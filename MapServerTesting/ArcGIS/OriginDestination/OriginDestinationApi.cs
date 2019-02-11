using System;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;
namespace MapServerTesting.ArcGIS.OriginDestination
{
    public class OriginDestinationApi
    {
        public OriginDestinationApi()
        {
            // Submit job request /submitJob?parameters
            // Status query /jobs/<yourJobID>/?token=<yourToken>&returnMessages=true&f=json
            client = new RestClient("https://logistics.arcgis.com/arcgis/rest/services/World/OriginDestinationCostMatrix/GPServer/GenerateOriginDestinationCostMatrix");

            // Fetching a result returns a "text/plain" Content-Type that needs to be
            // parsed with JSON.
            client.AddHandler("text/plain", new RestSharp.Serialization.Json.JsonDeserializer());
        }

        /// <summary>
        /// Asynchronously performs an Origin Destination Cost Matrix query, returning
        /// the parsed result. The returned result is in 1-1 correspondence with the
        /// returned JSON, and it is beneficial to parse this into an intermediate
        /// data type that corresponds better to business logic.
        /// </summary>
        /// <returns>The query result.</returns>
        /// <param name="data">The request data.</param>
        /// <param name="pollInterval">
        ///     The wait time in milliseconds between polling for job completion.
        ///     Keep this at a reasonable value (e.g. 1000ms).
        /// </param>
        public async Task<OriginDestinationResult> PerformQueryAsync(OriginDestinationRequest data, int pollInterval = 1000)
        {
            RestRequest request = new RestRequest("/submitJob?parameters", Method.POST);

            data.AddToRequest(request);

            // Submit job.
            var response = await client.ExecuteTaskAsync<OriginDestinationResponse>(request);

            Console.WriteLine($"Returned content: {response.Content}");
            Console.WriteLine($"Job ID: {response.Data.jobId}");
            Console.WriteLine($"Status: {response.Data.jobStatus}");

            string jobId = response.Data.jobId;
            string jobStatus = response.Data.jobStatus;

            // Wait until job is completed.
            while (!IsJobDone(jobStatus))
            {
                RestRequest statusRequest = new RestRequest($"/jobs/{jobId}", Method.GET);
                statusRequest.AddParameter("token", await data.AccessToken.GetTokenAsync());
                statusRequest.AddParameter("returnMessages", "true");
                statusRequest.AddParameter("f", "json");

                response = await client.ExecuteTaskAsync<OriginDestinationResponse>(statusRequest);

                Console.WriteLine($"Status response: {response.Content}");
                Console.WriteLine($"Got status: {response.Data.jobStatus}");

                jobStatus = response.Data.jobStatus;

                // Wait between requests to avoid spamming server.
                await Task.Delay(pollInterval);
            }

            // Get job results. There is more data that can be returned, see
            // https://developers.arcgis.com/rest/network/api-reference/origin-destination-cost-matrix-service.htm
            RestRequest resultRequest = new RestRequest($"/jobs/{jobId}/results/output_origin_destination_lines", Method.GET);
            resultRequest.AddParameter("token", await data.AccessToken.GetTokenAsync());
            resultRequest.AddParameter("f", "pjson"); // prettified json; good for debugging

            var resultResponse = await client.ExecuteTaskAsync<OriginDestinationResult>(resultRequest);

            if (resultResponse.ErrorException != null)
                throw new ApplicationException("Failed to get result.", resultResponse.ErrorException);

            Console.WriteLine($"Final result = {resultResponse.Content}");

            return resultResponse.Data;
        }

        private bool IsJobDone(string jobStatus)
        {
            /* Possible strings:
               (see https://developers.arcgis.com/rest/network/api-reference/origin-destination-cost-matrix-service.htm#ESRI_SECTION1_4C8C90C1429A4724876CA355E3F824B4)           
                esriJobSubmitted
                esriJobWaiting
                esriJobExecuting
                esriJobSucceeded
                esriJobFailed
                esriJobTimedOut
                esriJobCancelling
                esriJobCancelled
            */

            switch (jobStatus)
            {
                case "esriJobSucceeded":
                case "esriJobFailed":
                case "esriJobTimedOut":
                case "esriJobCancelled":
                case "esriJobCancelling":
                    return true;

                default:
                    return false;
            }
        }

        private RestClient client;
    }
}
