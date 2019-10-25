using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkingSlotAPI.Database;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Helpers;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Repository
{
    public interface IUserRepository
    {
        User Authenticate(string username, string password);
        IEnumerable<User> GetUsers();
        User GetUser(Guid userId);
        void AddUser(User user);
        void DeleteUser(User user);
        bool UserExists(Guid userId);
        bool Save();
    }

    public class UserRepository : IUserRepository
    {

        // users hardcoded for simplicity, store in a db with hashed passwords in production applications
        private List<User> _users = new List<User>
        {
            new User { FirstName = "Andrew", LastName = "Cai", Email="andrewcai19972011@gmail.com", Id = Guid.NewGuid(), Role = "Admin" }
        };


        private ParkingContext _context;
        private readonly AppSettings _appSettings;

        public UserRepository(ParkingContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        public User Authenticate(string username, string password)
        {
            var user = _users.SingleOrDefault(x => x.Username == username && x.Password == password);

            if (user == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            user.Token = tokenHandler.WriteToken(token);

            // remove password before returning
            user.Password = null;

            return user;
        }

        public IEnumerable<User> GetUsers()
        {
            return _context.Users.OrderBy(a => a.FirstName);
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

        public bool UserExists(Guid userId)
        {
            return _context.Users.Any(a => a.Id == userId);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
