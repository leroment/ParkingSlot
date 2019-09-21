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
                var carparks = new List<Carpark>();

                FetchPublicAPI publicAPI = new FetchPublicAPI();

                var task = Task.Run(async () => await publicAPI.GetParkingInfoAsync());

                carparks = task.Result;

                context.Carparks.AddRange(carparks);

                context.SaveChanges();
            }
        }



    }
}
