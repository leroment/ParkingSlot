using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Models
{
    public class FeedbackForUpdateDto
    {
        public string Name { get; set; }
        public string Topic { get; set; }
        public string Description { get; set; }
        public bool IsResolved { get; set; }
        public string Comments { get; set; }
    }
}
