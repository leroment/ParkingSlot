using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Models
{
    public class UserForUpdatePasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
        public string username { get; set; }
    }
}
