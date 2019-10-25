using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using AutoMapper;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Models;
using ParkingSlotAPI.Repository;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace ParkingSlotAPI.Controllers
{
    [Route("api/users/{UserId}/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {

        private readonly IFavoriteRepository _favoriteRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
   
        
        public FavoriteController(IFavoriteRepository favoriteRepository, IUserRepository userRepository, IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        //[HttpGet]
        //public IActionResult GetFavorites()
        //{
        //    var favoritesFromRepo = _favoriteRepository.GetFavorites();

        //    if(favoritesFromRepo == null)
        //    {
        //        return NotFound();
        //    }

        //    var favorites = _mapper.Map<IEnumerable<FavoriteDto>>(favoritesFromRepo);

        //    return Ok(favorites);
        //}

        [HttpGet("{id}", Name = "GetFavoriteForUser")]
        public IActionResult GetFavoriteForUser(Guid userId, Guid id)
        {
            if (!_userRepository.UserExists(userId))
            {
                return NotFound();
            }

            var favoriteForAuthorFromRepo = _favoriteRepository.GetFavoriteForUser(userId, id);

            if (favoriteForAuthorFromRepo == null)
            {
                return NotFound();
            }

            var favoriteForAuthor = _mapper.Map<FavoriteDto>(favoriteForAuthorFromRepo);

            return Ok(favoriteForAuthor);
        }

        [HttpPost]
        public IActionResult AddFavoriteForUser(Guid userId,[FromBody] FavoriteForCreationDto favorite)
        {
            if (favorite == null)
            {
                return BadRequest();
            }

            if (!_userRepository.UserExists(userId))
            {
                return NotFound();
            }

            var favoriteEntity = _mapper.Map<Favorite>(favorite);

            _favoriteRepository.AddFavoriteForUser(userId, favoriteEntity);

            if (!_favoriteRepository.Save())
            {
                throw new Exception($"Adding a favorite for user {userId} failed on save.");
            }

            var favoriteToReturn = _mapper.Map<FavoriteDto>(favoriteEntity);

            return CreatedAtRoute("GetFavoriteForUser", new {userId = userId, id = favoriteToReturn.Id }, favoriteToReturn);
        }

        //[HttpDelete("{id}")]
        //public IActionResult DeleteFavorite(Guid id)
        //{
        //    var favoriteFromRepo = _favoriteRepository.GetFavorite(id);
        //    if(favoriteFromRepo == null)
        //    {
        //        return NotFound();
        //    }

        //    _favoriteRepository.DeleteFavorite(favoriteFromRepo);

        //    if(!_favoriteRepository.Save())
        //    {
        //        throw new Exception($"Deleting favorite {id} failed on save");
        //    }

        //    return NoContent();
        //}
    }
}

