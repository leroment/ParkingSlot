using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkingSlotAPI.Models;
using ParkingSlotAPI.Services;

namespace ParkingSlotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingController : ControllerBase
    {

        private readonly IParkingInfoServices _services;
        private readonly IMapper _mapper;

        public ParkingController(IParkingInfoServices services, IMapper mapper)
        {
            _services = services;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCarparks()
        {
            return Ok();
        }

    }
}