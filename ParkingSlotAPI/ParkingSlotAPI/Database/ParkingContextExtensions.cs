using ParkingSlotAPI.Entities;
using ParkingSlotAPI.PublicAPI;
using ParkingSlotAPI.PublicAPIEntities;
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

            FetchPublicAPI publicAPI = new FetchPublicAPI();

            if (!context.Carparks.Any())
            {
                var carparks = new List<Carpark>();

                var task = Task.Run(async () => await publicAPI.GetHDBParkingInfoAsync());

                carparks = task.Result;

                context.Carparks.AddRange(carparks);

                context.SaveChanges();
            }
            else
            {

                var carparks = new List<Carpark>();

                var task = Task.Run(async () => await publicAPI.GetURAParkingInfoAsync());

                carparks = task.Result;

                // context.Carparks.AddRange(carparks);

                // context.SaveChanges();

                //var carparkAvailability = new List<Carpark_Data>();

                //var task = Task.Run(async () => await publicAPI.GetHDBAvailabilityAsync());

                //carparkAvailability = task.Result;

                //foreach (var value in carparkAvailability)
                //{
                //    var ttlAvailable = 0;
                //    var ttlLots = 0;

                //    if (value.carpark_info.Length > 0)
                //    {
                //        foreach (var d in value.carpark_info)
                //        {
                //            ttlAvailable += int.Parse(d.lots_available);
                //            ttlLots += int.Parse(d.total_lots);
                //        }
                //    }

                //    var v = context.Carparks.FirstOrDefault(a => a.CarparkId.Equals(value.carpark_number));

                //   if (v != null)
                //   {
                //        v.TotalAvailableLots = ttlAvailable;
                //        v.TotalLots = ttlLots;

                //        context.Carparks.Update(v);
                //   }
                //}

                //context.SaveChanges();

            }
        }



    }
}
