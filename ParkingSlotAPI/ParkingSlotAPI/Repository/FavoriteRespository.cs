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
        IEnumerable<Favorite> GetFavorites();
        Favorite GetFavorite(Guid favoriteId);
        void SaveFavorite(Favorite favorite);
        void DeleteFavorite(Favorite favorite);
        bool Save();
    }
    public class FavoriteRepository : IFavoriteRepository
    {
        private FavoriteContext _context;
        public FavoriteRepository(FavoriteContext context)
        {
            _context = context;
        }

        //User Entity has collection of favorite list.
        // So use Users database to store the favorite lists.
        //Supposedly to use favorite database...

        public IEnumerable<Favorite> GetFavorites()
        {
            return _context.Favorites.OrderBy(a => a.Favorites);
        }

        public Favorite GetFavourite(Guid favoriteId)
        {
            return _context.Favorites.FirstOrDefault(a => a.Id == favoriteId);
        }

        public void SaveFavorite(Favorite favorite)
        {
            favorite = Guid.NewGuid();
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

