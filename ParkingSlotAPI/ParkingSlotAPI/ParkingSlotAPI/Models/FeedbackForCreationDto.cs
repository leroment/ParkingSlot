using ParkingSlotAPI.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Models
{
    public class FeedbackForCreationDto
    {
        public string Name { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public Guid UserId { get; set; }
    }
}
