using ParkingSlotAPI.Entities;
using ParkingSlotAPI.PublicAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Database
{
    public static class ParkingContextExtensions
    {

        public static void EnsureSeedDataForContext(this ParkingContext context )
        {

            if (!context.Carparks.Any())
            {
                FetchPublicAPI publicAPI = new FetchPublicAPI();

                var carparks = new List<Carpark>();

                var task = Task.Run(async () => await publicAPI.GetParkingInfoAsync());

                carparks = task.Result;

                context.Carparks.AddRange(carparks);

                context.SaveChanges();
            }

            if (!context.CarparkRates.Any())
            {
                FetchPublicAPI publicAPI = new FetchPublicAPI();

                var carparkRates = new List<CarparkRate>();

                var task = Task.Run(async () => await publicAPI.GetURAParkingRateAsync());

                carparkRates = task.Result;

                context.CarparkRates.AddRange(carparkRates);

                context.SaveChanges();

            }
        }



    }
}
