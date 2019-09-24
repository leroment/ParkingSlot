using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Models;
using ParkingSlotAPI.Repository;

namespace ParkingSlotAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] User userParam)
        {
            var user = _userRepository.Authenticate(userParam.Username, userParam.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

            return Ok(user);
        }

        [HttpGet("{id}", Name = "GetUser")]
        public IActionResult GetUser(Guid id)
        {
            var userFromRepo = _userRepository.GetUser(id);

            if (userFromRepo == null)
            {
                return NotFound();
            }

            var user = _mapper.Map<UserDto>(userFromRepo);

            return Ok(user);
        }

        [HttpPost]
        public IActionResult CreateUser([FromBody] UserForCreationDto user)
        {
            if (user == null)
            {
                return BadRequest();
            }

            var userEntity = _mapper.Map<User>(user);

            _userRepository.AddUser(userEntity);

            if (!_userRepository.Save())
            {
                throw new Exception("Creating a user failed on save.");
            }

            var userToReturn = _mapper.Map<UserDto>(userEntity);

            return CreatedAtRoute("GetUser", new { id = userToReturn.Id }, userToReturn);

        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(Guid id)
        {
            var userFromRepo = _userRepository.GetUser(id);
            if (userFromRepo == null)
            {
                return NotFound();
            }

            _userRepository.DeleteUser(userFromRepo);

            if (!_userRepository.Save())
            {
                throw new Exception($"Deleting user {id} failed on save");
            }

            return NoContent();
        }
    }
}