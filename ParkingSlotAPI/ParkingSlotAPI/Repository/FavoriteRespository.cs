using System;
using System.Collections.Generic;
using System.Linq;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Database;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Respository
{
    public interface IFavoriteRespository
    {
        IEnumerable<Favorite> GetFavorites();
        Favorite GetFavorite(Guid favoriteId);
        void SaveFavorite(Favorite favorite);
        void DeleteFavorite(Favorite favorite);
        bool Save();
    }
    public class FavoriteRespository : IFavoriteRespository
    {
        private FavoriteContext _context;
        public FavoriteRespository(FavoriteContext context)
        {
            _context = context;
        }

        //User Entity has collection of favorite list.
        // So use Users database to store the favorite lists.

        public IEnumerable<Favorite> GetFavorites()
        {
            return _context.Users.OrderBy(a => a.Favorites);
        }

        public Favorite GetFavourite(Guid favoriteId)
        {
            return _context.Users.FirstOrDefault(a => a.Id == favoriteId);
        }

        public void SaveFavorite(Favorite favorite)
        {
            favorite = Guid.NewGuid();
            _context.Users.Add(favorite);
        }

        public void DeleteFavorite(Favorite favorite)
        {
            _context.Users.Remove(favorite);
        }

        public bool Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}

