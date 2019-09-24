using Microsoft.EntityFrameworkCore;
using ParkingSlotAPI.Database;
using ParkingSlotAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Repository
{
    public interface ICarparkRatesRepository
    {
        IEnumerable<CarparkRate> GetCarparkRates();
    }

    public class CarparkRatesRepository : ICarparkRatesRepository
    {
        private readonly ParkingContext _context;

        public CarparkRatesRepository(ParkingContext context)
        {
            _context = context;
        }

        public IEnumerable<CarparkRate> GetCarparkRates()
        {
            return _context.CarparkRates;
        }

    }
}
