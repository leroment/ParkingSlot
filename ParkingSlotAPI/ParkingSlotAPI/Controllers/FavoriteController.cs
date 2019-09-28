using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Sytem.Threading.Task;
using AutoMapper;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Models;
using ParkingSlotAPI.Respository;

namespace ParkingSlotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteRespository _favoriteRespository;
        private readonly IMapper _mapper;
        
        public FavoriteController(IFavoriteRespository favoriteRespository, IMapper mapper)
        {
            _favoriteRespository = favoriteRespository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetFavorites()
        {
            var favoritesFromRepo = _favoriteRespository.GetFavorites();

            if(favoritesFromRepo == null)
            {
                return NotFound();
            }

            var favorites = _mapper.Map<IEnumberable<FavoriteDto>>(favoritesFromRepo);

            return ok(favorites);
        }

        [HttpGet("{id}", Name = "GetFavorite")]
        public IActionResult GetFavorite(Guid id)
        {
            var favoriteFromRepo = _favoriteRespository.GetFavorite(id);

            if(favoriteFromRepo == null)
            {
                return NotFound();
            }

            var favorite = _mapper.Map<FavoriteDto>(favoriteFromRepo);

            return Ok(favorite);
        }

        [HttpPost]
        public IActionResult AddFavorite([FromBody] FavoriteDto favorite)
        {
            if (favorite == null)
            {
                return BadRequest();
            }

            var favoriteEntity = _mapper.Map<Favorite>(favorite);

            _favoriteRespository.SaveFavorite(favoriteEntity);
            if (!_favoriteRespository.Save())
            {
                throw new Exception("Adding a favorite failed on save.");
            }

            var favoriteToReturn = _mapper.Map<FavoriteDto>(favoriteEntity);

            return CreatedAtRoute("GetFavorite", new { id = favoriteToReturn.Id }, favoriteToReturn);
        }

        [HttpPost]
        public IActionResult DeleteFavorite(Guid id)
        {
            var favoriteFromRepo = _favoriteRespository.GetFavorite(id);
            if(favoriteFromRepo == null)
            {
                return NotFound();
            }

            _favoriteRespository.DeleteFavorite(favoriteFromRepo);

            if(!_favoriteRespository.Save())
            {
                throw new Exception($"Deleting favorite {id} failed on save");
            }

            return NoContent();
        }
    }
}

