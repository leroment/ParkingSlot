using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkingSlotAPI.Helpers;
using ParkingSlotAPI.Models;
using ParkingSlotAPI.Repository;

namespace ParkingSlotAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CarparkController : ControllerBase
    {
        private readonly IParkingRepository _parkingRepository;
        private readonly IMapper _mapper;

        const int maxParkingPageSize = 20;

        public CarparkController(IParkingRepository parkingRepository, IMapper mapper)
        {
            _parkingRepository = parkingRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult GetCarparks([FromQuery] CarparkResourceParameters carparkResourceParameters)
        {

            var carparksFromRepo = _parkingRepository.GetCarparks(carparkResourceParameters);

            if (carparksFromRepo == null)
            {
                return NotFound();
            }

            var carparks = _mapper.Map<IEnumerable<CarparkDto>>(carparksFromRepo);

            return Ok(carparks);
        }


        [HttpGet("{id}")]
        public IActionResult GetCarpark(Guid id)
        {
            var carparkFromRepo = _parkingRepository.GetCarpark(id);

            if (carparkFromRepo == null)
            {
                return NotFound();
            }

            var carpark = _mapper.Map<CarparkDto>(carparkFromRepo);

            return Ok(carpark);
        }
    }
}