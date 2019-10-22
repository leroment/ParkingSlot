using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Helpers;
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
        private readonly AppSettings _appSettings;
        private readonly IEmailSender _emailSender;

        public UsersController(IUserRepository userRepository, IMapper mapper, IOptions<AppSettings> appSettings, IEmailSender emailSender)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _appSettings = appSettings.Value;
            _emailSender = emailSender;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Authenticate([FromBody] UserForAuthenticationDto userDto)
        {
            var user = _userRepository.Authenticate(userDto.Username, userDto.Password);

            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });

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

            var userFromRepo = _mapper.Map<UserDto>(user);

            userFromRepo.Token = tokenHandler.WriteToken(token);

            return Ok(userFromRepo);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public IActionResult GetUsers()
        {
            var users = _userRepository.GetUsers();

            var userDtos = _mapper.Map<IList<UserDto>>(users);

            return Ok(userDtos);
        }

        [AllowAnonymous]
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

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register([FromBody]UserForCreationDto userForCreationDto)
        {
            // map dto to entity
            var user = _mapper.Map<User>(userForCreationDto);

            try
            {
                // save
                var userFromRepo = _userRepository.Create(user, userForCreationDto.Password);

                var userToReturn = _mapper.Map<UserDto>(userFromRepo);

                return Ok(userToReturn);
            }
            catch (AppException ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public IActionResult UpdateUser(Guid id, [FromBody]UserForUpdateDto userForUpdateDto)
        {
            // map dto to entity and set id
            var user = _mapper.Map<User>(userForUpdateDto);

            user.Id = id;

            try
            {
                _userRepository.UpdateUser(user);
                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
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

        [AllowAnonymous]
        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] UserForUpdatePasswordDto userForUpdatePasswordDto)
        {
            if (userForUpdatePasswordDto.OldPassword == null)
            {
                var user = _userRepository.GetUserByEmail(userForUpdatePasswordDto.Email);

                if (user == null)
                    return BadRequest(new { message = "Cannot find user. Please check the username." });

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                    }),
                    Expires = DateTime.UtcNow.AddDays(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);

                var userFromRepo = _mapper.Map<UserDto>(user);

                var tokenString = tokenHandler.WriteToken(token);

                string url = HttpContext.Request.Scheme + "://" + HttpContext.Request.Host + "/resetpassword/" + user.Id + "/" + tokenString;

                await _emailSender.SendEmailAsync(user.Email, "ParkingSlot Password Reset Link", "Please reset your password by clicking <a href=\"" + url + "\">here</a>");

                return Ok(userFromRepo);
            }
            else
            {
                if (userForUpdatePasswordDto.username == null)
                {
                    return BadRequest("Please provide a username");
                }

                var user = _userRepository.Authenticate(userForUpdatePasswordDto.username, userForUpdatePasswordDto.OldPassword);

                if (user == null)
                {
                    return BadRequest("User does not exist.");
                }

                _userRepository.UpdatePassword(user, userForUpdatePasswordDto.NewPassword);

                return Ok();
            }
        }

        [AllowAnonymous]
        [HttpPost("ConfirmPassword")]
        public IActionResult ConfirmPassword([FromBody] UserForUpdatePasswordDto userForUpdatePasswordDto)
        {
            var jwt = userForUpdatePasswordDto.Token;
            var handler = new JwtSecurityTokenHandler();
            var userId = "";
            try
            {
                var token = handler.ReadToken(jwt) as JwtSecurityToken;

                if (token == null)
                {
                    throw new AppException("Token is null.");
                }

                foreach (var payload in token.Payload)
                {
                    if (payload.Key == "unique_name")
                    {
                        userId = payload.Value.ToString();
                        break;
                    }
                }

                var user = _userRepository.GetUser(Guid.Parse(userId));

                if (user == null)
                {
                    return NotFound();
                }

                _userRepository.UpdatePassword(user, userForUpdatePasswordDto.NewPassword);

                return Ok();
            }
            catch (AppException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            
        }
    }
}