using System;
using System.Collections.Generic;
using System.Linq;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Database;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Repository
{
    public interface IFavoriteRepository
    {
        IEnumerable<Favorite> GetFavoritesForUser(Guid userId);
        Favorite GetFavoriteForUser(Guid userId, Guid favoriteId);
        void AddFavoriteForUser(Guid userId, Favorite favorite);
        void SaveFavorite(Favorite favorite);
        void DeleteFavorite(Favorite favorite);
        bool Save();
    }
    public class FavoriteRepository : IFavoriteRepository
    {
        private ParkingContext _context;
        private IUserRepository _userRepository;
        public FavoriteRepository(ParkingContext context, IUserRepository userRepository)
        {
            _context = context;
            _userRepository = userRepository;
        }

        public IEnumerable<Favorite> GetFavoritesForUser(Guid userId)
        {
            return _context.Favorites.Where(f => f.UserId == userId).OrderBy(f => f.CarparkId).ToList();
        }

        public Favorite GetFavoriteForUser(Guid userId, Guid favoriteId)
        {
            return _context.Favorites.FirstOrDefault(f => f.UserId == userId && f.Id == favoriteId);
        }

        public void AddFavoriteForUser(Guid userId, Favorite favorite)
        {
            var user = _userRepository.GetUser(userId);

            if (user != null)
            {
                if (favorite.Id == Guid.Empty)
                {
                    favorite.Id = Guid.NewGuid();
                }

                user.Favorites.Add(favorite);
            }
        }

        public void SaveFavorite(Favorite favorite)
        {
            favorite.Id = Guid.NewGuid();
            _context.Favorites.Add(favorite);
        }

        public void DeleteFavorite(Favorite favorite)
        {
            _context.Favorites.Remove(favorite);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

