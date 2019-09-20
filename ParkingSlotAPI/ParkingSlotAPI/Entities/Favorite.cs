using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ParkingSlotAPI.Entities
{
    public class Favorite
    {
        [Key]
        public Guid Id { get; set; }
        [ForeignKey("CaparkId")]
        public User User { get; set; }

        public Guid UserId { get; set; }

    }
}