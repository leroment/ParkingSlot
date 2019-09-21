using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkingSlotAPI.Models;
using ParkingSlotAPI.Repository;

namespace ParkingSlotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ParkingController : ControllerBase
    {

        private readonly IParkingRepository _parkingRepository;
        private readonly IMapper _mapper;

        public ParkingController(IParkingRepository parkingRepository, IMapper mapper)
        {
            _parkingRepository = parkingRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCarparks()
        {
            var caparksFromRepo = _parkingRepository.GetCarparks();

            var carparks = _mapper.Map<IEnumerable<CarparkDto>>(caparksFromRepo);

            return Ok(carparks);
        }

    }
}