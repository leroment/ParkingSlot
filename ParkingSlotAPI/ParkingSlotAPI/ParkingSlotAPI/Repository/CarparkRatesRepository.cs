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
		IEnumerable<CarparkRate> GetCarparkRateById(Guid carparkId, string vehicletype);

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
			return _context.CarparkRates.OrderBy(a => a.CarparkId);
			//	return _context.CarparkRates1.Where(o => o.CarparkId == "TE27");
		}

		public IEnumerable<CarparkRate> GetCarparkRateById(Guid carparkId, string vehicletype)
		{
			return _context.CarparkRates.Where(o => o.CarparkId == carparkId&& o.VehicleType == vehicletype);
		}

	}
}
