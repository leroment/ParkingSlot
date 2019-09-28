using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Models
{
    public class FavoriteDto
    {
        public Guid Id { get; set; }

        public String CarparkId { get; set; }

        public String CarparkName { get; set; }
    }
}

