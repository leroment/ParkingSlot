using Newtonsoft.Json;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Helpers;
using ParkingSlotAPI.PublicAPIEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParkingSlotAPI.PublicAPI
{
    public class FetchPublicAPI
    {
        public async Task<List<Carpark>> GetParkingInfoAsync()
        {
            List<Carpark> carParks = new List<Carpark>();

            for (int i = 0; i <= 2000; i += 500)
            {
                var responseBody = await HttpHelpers.GetResource($"http://datamall2.mytransport.sg/ltaodataservice/CarParkAvailabilityv2?$skip={i}", "AccountKey", "pZIQovINS3+Z2fs9oLqWkg==");

                DataMallEntity dataMallEntity = JsonConvert.DeserializeObject<DataMallEntity>(responseBody);

                foreach (var value in dataMallEntity.value)
                {
                    // Split location coordinates
                    string[] coordinates = { "", "" };

                    // If there is no location, XCoord and YCoord should be blank
                    if (value.Location == "")
                    {
                        coordinates[0] = "";
                        coordinates[1] = "";
                    }
                    // Otherwise, split location into XCoord and YCoord
                    else
                    {
                        coordinates = value.Location.Split(" ");
                    }

                    Carpark carpark =
                        new Carpark
                        {
                            Id = Guid.NewGuid(),
                            CarparkId = value.CarParkID,
                            CarparkName = value.Development,
                            LotType = value.LotType,
                            Area = value.Area,
                            AgencyType = value.Agency,
                            Address = value.Agency != "LTA" ? value.Development : "",
                            XCoord = coordinates[0],
                            YCoord = coordinates[1]
                        };

                    carParks.Add(carpark);
                }
            }

            return carParks;
        }

        public async Task<List<CarparkRate>> GetHDBParkingInfoAsync()
        {

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://data.gov.sg/api/action/datastore_search?resource_id=139a3035-e624-4f56-b63f-89ae28d4ae4c&limit=1000000");

                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                    response.EnsureSuccessStatusCode();

                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    HDBEntity hdbEntity = JsonConvert.DeserializeObject<HDBEntity>(responseBody);


                    return null;
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
