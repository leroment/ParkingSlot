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

        //[Required]
        //public string LotType { get; set; }

        public string Area { get; set; }

        [Required]
        public string AgencyType { get; set; }

        public string Address { get; set; }

        public string XCoord { get; set; }
        public string YCoord { get; set; }
        public int Available { get; set; }




    }
}
