using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Entities
{
    public class Carpark
    {
        public Guid Id { get; set; }

        [Required]
        [Key]
        public string CarparkId { get; set; }

        [Required]
        public string CarparkName { get; set; }
        public string ParkingSystem { get; set; }
        public int TotalAvailableLots { get; set; }
        public int TotalLots { get; set; }
        [Required]
        public string AgencyType { get; set; }
        public string Address { get; set; }
        public string XCoord { get; set; }
        public string YCoord { get; set; }
        public bool IsCentral { get; set; }
        public string LotType { get; set; }
    }
}
