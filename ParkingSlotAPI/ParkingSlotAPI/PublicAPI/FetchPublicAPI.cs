using Newtonsoft.Json;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Helpers;
using ParkingSlotAPI.PublicAPIEntities;
using ParkingSlotAPI.Repository;
using SVY21;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ParkingSlotAPI.PublicAPI
{
    public interface IFetchPublicAPI
    {
        Task<List<Carpark>> GetParkingInfoAsync();
        Task<List<Carpark>> GetHDBParkingInfoAsync();
        Task<List<Results>> GetURAParkingInfoAsync();
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

                    if (value.Agency == "LTA")
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
                                AgencyType = value.Agency,
                                Address = value.Area,
                                XCoord = coordinates[0],
                                YCoord = coordinates[1],
                                IsCentral = false,
                                ParkingSystem = "",
                                CarAvailability = 0,
                                CarCapacity = 0,
                                HVCapacity = 0,
                                LotType = value.LotType,
                                HVAvailability = 0,
                                MAvailability = 0,
                                MCapacity = 0,
                                TotalAvailableLots = 0,
                                TotalLots = 0
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
            }

            return carParks;
        }

        public async Task<List<Carpark_Data>> GetHDBAvailabilityAsync()
        {
            var responseBody = await HttpHelpers.GetResourceNoHeader("https://api.data.gov.sg/v1/transport/carpark-availability");

            HDBAvailabilityEntity hDBAvailability = JsonConvert.DeserializeObject<HDBAvailabilityEntity>(responseBody);

            List<Carpark_Data> carparkData = new List<Carpark_Data>();

            foreach (var value in hDBAvailability.items[0].carpark_data)
            {
                carparkData.Add(value);
            }

            return carparkData;
        }


        public async Task<List<Carpark>> GetHDBParkingInfoAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://data.gov.sg/api/action/datastore_search?resource_id=139a3035-e624-4f56-b63f-89ae28d4ae4c&limit=1000000");

                    HttpResponseMessage response = await client.GetAsync(client.BaseAddress);
                    response.EnsureSuccessStatusCode();

                    string responseBody = response.Content.ReadAsStringAsync().Result;

                    HDBEntity HdbEntity = JsonConvert.DeserializeObject<HDBEntity>(responseBody);

                    List<Carpark> carparks = new List<Carpark>();

                    foreach (var c in HdbEntity.result.records)
                    {
                        Svy21Coordinate svy21 = new Svy21Coordinate(double.Parse(c.x_coord), double.Parse(c.y_coord));

                        LatLongCoordinate latLong = svy21.ToLatLongCoordinate();

                        bool isCentral = false;

                        if (c.car_park_no.Contains("HLM")
                            || c.car_park_no.Contains("KAB")
                            || c.car_park_no.Contains("KAM")
                            || c.car_park_no.Contains("KAS")
                            || c.car_park_no.Contains("PRM")
                            || c.car_park_no.Contains("SLS")
                            || c.car_park_no.Contains("SR1")
                            || c.car_park_no.Contains("SR2")
                            || c.car_park_no.Contains("TPM")
                            || c.car_park_no.Contains("UCS")
                            )
                        {
                            isCentral = true;
                        }

                        Carpark carpark = new Carpark()
                        {
                           Id = Guid.NewGuid(),
                           Address = c.address,
                           AgencyType = "HDB",
                           CarparkId = c.car_park_no,
                           XCoord = latLong.Latitude.ToString(),
                           YCoord = latLong.Longitude.ToString(),
                           CarparkName = c.address,
                           IsCentral = isCentral,
                           ParkingSystem = c.type_of_parking_system
                        };

                        carparks.Add(carpark);
                    }
                    return carparks;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nHttpRequest couldn't be fulfilled.");
                Console.WriteLine("Message: {0} ", e.Message);
                return null;
            }
        }

        public async Task<List<Results>> GetURAParkingInfoAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var responseBody = await HttpHelpers.GetResourceTwoHeaders("https://www.ura.gov.sg/uraDataService/invokeUraDS?service=Car_Park_Details", "AccessKey", "942ba181-3f0a-4e37-bb3e-6f66093945d3", "Token", "Sr@MGME4Nk902EdmuM0J0awYrv8adBwXKZ3y83HfTcne379y34+ZDGv6pbm5-y2Ye7sS3JndQ7eQ3f3+b78AE9nJ1AZN3FYB5veG");

                    URACarparkInfo result = JsonConvert.DeserializeObject<URACarparkInfo>(responseBody);

                    List<Results> results = new List<Results>();

                    results.AddRange(result.Result);

                    return results;

                    //List<Carpark> carparks = new List<Carpark>();

                    //var carparkNo = "";
                    //var lotType = "";
                    //foreach (var value in result.Result)
                    //{
                    //    lotType = "";
                    //    if (carparkNo == value.ppCode)
                    //    {
                    //        double xcoord = 0.00, ycoord = 0.00;

                    //        if (value.geometries != null)
                    //        {
                    //            var uncooord = value.geometries[0].coordinates.Split(",");
                    //            var x = uncooord[0];
                    //            var y = uncooord[1];

                    //            Svy21Coordinate svy21 = new Svy21Coordinate(double.Parse(x), double.Parse(y));

                    //            LatLongCoordinate latLong = svy21.ToLatLongCoordinate();

                    //            xcoord = latLong.Latitude;
                    //            ycoord = latLong.Longitude;
                    //        }

                    //        Carpark carpark = new Carpark()
                    //        {
                    //            Id = Guid.NewGuid(),
                    //            CarparkId = value.ppCode,
                    //            CarparkName = value.ppName,
                    //            Address = value.ppName,
                    //            AgencyType = "URA",
                    //            IsCentral = false,
                    //            XCoord = xcoord.ToString(),
                    //            YCoord = ycoord.ToString(),
                    //            ParkingSystem = "",
                    //            LotType = ""
                    //        };

                    //        carparks.Add(carpark);
                    //    }

                    //    carparkNo = value.ppCode;
                    //}

                 
                    
                    

                    //return carparks;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nHttpRequest couldn't be fulfilled.");
                Console.WriteLine("Message: {0} ", e.Message);
                return null;
            }
        }


        public async Task<List<Detail>> GetURAAvailability()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var responseBody = await HttpHelpers.GetResourceTwoHeaders("https://www.ura.gov.sg/uraDataService/invokeUraDS?service=Car_Park_Availability", "AccessKey", "942ba181-3f0a-4e37-bb3e-6f66093945d3", "Token", "9485e67-Va6h4Z6T3TgaqQkefbbC7fgtqeepbMDDg43r6M4D2SyMMz9Mba2EJSQ45AJX63@q-TaeE@BayXH1VxdfTteq@bXTx@9P");

                    URACarparkAvailability result = JsonConvert.DeserializeObject<URACarparkAvailability>(responseBody);

                    List<Detail> results = new List<Detail>();

                    results.AddRange(result.Result);

                    return results;
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nHttpRequest couldn't be fulfilled.");
                Console.WriteLine("Message: {0} ", e.Message);
                return null;
            }
        }

        public async Task<List<Records>> GetShoppingMallAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    var responseBody = await HttpHelpers.GetResourceNoHeader("https://data.gov.sg/api/action/datastore_search?resource_id=85207289-6ae7-4a56-9066-e6090a3684a5&limit=1000000");

                    ShoppingMallEntity result = JsonConvert.DeserializeObject<ShoppingMallEntity>(responseBody);

                    List<Records> records = new List<Records>();

                    records.AddRange(result.result.records);

                    return records;
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
