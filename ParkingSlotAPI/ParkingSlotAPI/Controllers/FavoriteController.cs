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
        private readonly IMapper _mapper;
        
        public FavoriteController(IFavoriteRepository favoriteRepository, IMapper mapper)
        {
            _favoriteRepository = favoriteRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetFavorites()
        {
            var favoritesFromRepo = _favoriteRepository.GetFavorites();

            if(favoritesFromRepo == null)
            {
                return NotFound();
            }

            var favorites = _mapper.Map<IEnumerable<FavoriteDto>>(favoritesFromRepo);

            return Ok(favorites);
        }

        [HttpGet("{id}", Name = "GetFavorite")]
        public IActionResult GetFavorite(Guid id)
        {
            var favoriteFromRepo = _favoriteRepository.GetFavorite(id);

            if(favoriteFromRepo == null)
            {
                return NotFound();
            }

            var favorite = _mapper.Map<FavoriteDto>(favoriteFromRepo);

            return Ok(favorite);
        }

        [HttpPost]
        public IActionResult AddFavorite(Guid userId,[FromBody] FavoriteForCreationDto favorite)
        {
            if (favorite == null)
            {
                return BadRequest();
            }

            var favoriteEntity = _mapper.Map<Favorite>(favorite);

            _favoriteRepository.SaveFavorite(favoriteEntity);
            if (!_favoriteRepository.Save())
            {
                throw new Exception("Adding a favorite failed on save.");
            }

            var favoriteToReturn = _mapper.Map<FavoriteDto>(favoriteEntity);

            return CreatedAtRoute("GetFavorite", new {userId = favoriteToReturn.UserId, id = favoriteToReturn.Id }, favoriteToReturn);
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteFavorite(Guid id)
        {
            var favoriteFromRepo = _favoriteRepository.GetFavorite(id);
            if(favoriteFromRepo == null)
            {
                return NotFound();
            }

            _favoriteRepository.DeleteFavorite(favoriteFromRepo);

            if(!_favoriteRepository.Save())
            {
                throw new Exception($"Deleting favorite {id} failed on save");
            }

            return NoContent();
        }
    }
}

