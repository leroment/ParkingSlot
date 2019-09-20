using ParkingSlotAPI.Database;
using ParkingSlotAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Services
{
    public interface IParkingRepository
    {
        IEnumerable<Carpark> GetCarparks();
        User GetUser(Guid userId);
        void AddUser(User user);
        void DeleteUser(User user);
    }

    public class ParkingRepository : IParkingRepository
    {
        private ParkingContext _context;
        public ParkingRepository(ParkingContext context)
        {
            _context = context;
        }

        public IEnumerable<Carpark> GetCarparks()
        {
            return _context.Carparks.OrderBy(a => a.CarparkId);
        }

        public User GetUser(Guid userId)
        {
            return _context.Users.FirstOrDefault(a => a.Id == userId);
        }

        public void AddUser(User user)
        {
            user.Id = Guid.NewGuid();
            _context.Users.Add(user);
        }

        public void DeleteUser(User user)
        {
            _context.Users.Remove(user);
        }
    }
}
