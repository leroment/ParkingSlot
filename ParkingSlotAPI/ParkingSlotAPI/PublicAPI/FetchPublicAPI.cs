using Newtonsoft.Json;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Helpers;
using ParkingSlotAPI.PublicAPIEntities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParkingSlotAPI.PublicAPI
{
    public interface IFetchPublicAPI
    {
        Task<List<Carpark>> GetParkingInfoAsync();
        Task<List<CarparkRate>> GetHDBParkingInfoAsync();
        Task<List<CarparkRate>> GetURAParkingRateAsync();
    }


    public class FetchPublicAPI : IFetchPublicAPI
    {

        public FetchPublicAPI()
        {
        }

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
                            Area = value.Area,
                            AgencyType = value.Agency,
                            Address = value.Agency != "LTA" ? value.Development : "",
                            XCoord = coordinates[0],
                            YCoord = coordinates[1],
                            Available = value.AvailableLots
                        };

                    if (carParks.Count == 0)
                    {
                        carParks.Add(carpark);
                    }

                    if (carParks.Count > 0)
                    {
                        if (carpark.CarparkId == carParks[carParks.Count - 1].CarparkId)
                        {

                        }
                        else
                        {
                            carParks.Add(carpark);
                        }
                    }

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
        
        public async Task<List<CarparkRate>> GetURAParkingRateAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://www.ura.gov.sg/uraDataService/invokeUraDS?service=Car_Park_Details");
                    client.DefaultRequestHeaders.Add("AccessKey", "942ba181-3f0a-4e37-bb3e-6f66093945d3");
                    client.DefaultRequestHeaders.Add("Token", "b44N3a4aG3j4a3K7rEEUWK4Atb-9TF--f4Ha3664kwe9H3cbkfej41w3Aa7e5dG+hEFatv00P5qD@QRexa4nbHT34-evenzm6465");

                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                    response.EnsureSuccessStatusCode();

                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    URACarparkRatesEntity _URACarparkRatesEntity = JsonConvert.DeserializeObject<URACarparkRatesEntity>(responseBody);

                    // _mapper.Map<IEnumerable<CarparkRate>>(_URACarparkRatesEntity);

                    List<CarparkRate> carparkRates = new List<CarparkRate>();

                    foreach (var value in _URACarparkRatesEntity.Result)
                    {
                        CarparkRate carparkRate = new CarparkRate()
                        {
                            Id = Guid.NewGuid(),
                            CarparkId = value.ppCode,
                            CarparkName = value.ppName,
                            EndTime = value.endTime,
                            SatdayMin = value.satdayMin,
                            SatdayRate = value.satdayRate,
                            Remarks = value.remarks,
                            SunPHMin = value.sunPHMin,
                            SunPHRate = value.sunPHRate,
                            StartTime = value.startTime,
                            VehicleType = value.vehCat,
                            WeekdayMin = value.weekdayMin,
                            WeekdayRate = value.weekdayRate
                        };

                        carparkRates.Add(carparkRate);
                    }

                    return carparkRates;
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
