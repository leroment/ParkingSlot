using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Helpers
{
    public class HttpHelpers
    {
        public static async Task<string> GetResource(string uriString, string headerKey = "", string headerValue = "")
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(uriString);
                    client.DefaultRequestHeaders.Add(headerKey, headerValue);

                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                    response.EnsureSuccessStatusCode();

                    string responseBody = response.Content.ReadAsStringAsync().Result;
                    return responseBody;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nHttpRequest couldn't be fulfilled.");
                Console.WriteLine("Message: {0} ", e.Message);
                return null;
            }
        }
    }
}
