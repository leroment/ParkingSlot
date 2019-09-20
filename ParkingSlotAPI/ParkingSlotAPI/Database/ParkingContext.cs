using Microsoft.EntityFrameworkCore;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Database
{
    public class ParkingContext : DbContext
    {
        private IParkingInfoServices _parkingInfoServices;

        public ParkingContext(DbContextOptions<ParkingContext> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Carpark> Carparks { get; set; }
        // public DbSet<CarparkRate> CarparkRates { get; set; }
    }
}
