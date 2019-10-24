using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using ParkingSlotAPI.Entities;

namespace ParkingSlotAPI.Models
{
    public class FavoriteDto
    {
        public Guid Id { get; set; }
        public string CarparkId { get; set; }
        public string CarparkName { get; set; }

       [ForeignKey("userId")]
       public User user { get; set; }
       public Guid UserId { get; set; }
    }
}

