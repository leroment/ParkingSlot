using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Helpers;
using ParkingSlotAPI.Models;
using ParkingSlotAPI.PublicAPI;
using ParkingSlotAPI.Repository;

namespace ParkingSlotAPI.Controllers
{
    static class Timer
    {
        public static DateTime RequestedDT = DateTime.Now;
    }


    [Route("api/[controller]")]
    [ApiController]
    public class CarparkController : ControllerBase
    {
        private readonly IParkingRepository _parkingRepository;
        private readonly IMapper _mapper;

        FetchPublicAPI publicAPI = new FetchPublicAPI();

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

        private Carpark UpdateHDBAvailability(Carpark carparkFromRepo)
        {
            var task = Task.Run(async () => await publicAPI.GetHDBAvailabilityAsync());

            var result = task.Result;

            var v = result.FirstOrDefault(a => a.carpark_number.Equals(carparkFromRepo.CarparkId));

            if (v != null)
            {
                if (v.carpark_info.Length > 0)
                {
                    foreach (var x in v.carpark_info)
                    {
                        if (x.lot_type.Equals("C"))
                        {
                            carparkFromRepo.CarAvailability = int.Parse(x.lots_available);
                        }
                        else if (x.lot_type.Equals("H"))
                        {
                            carparkFromRepo.HVAvailability = int.Parse(x.lots_available);
                        }
                        else if (x.lot_type.Equals("Y"))
                        {
                            carparkFromRepo.MAvailability = int.Parse(x.lots_available);
                        }
                    }

                    carparkFromRepo.TotalAvailableLots = carparkFromRepo.CarAvailability + carparkFromRepo.HVAvailability + carparkFromRepo.MAvailability;

                    _parkingRepository.UpdateCarpark(carparkFromRepo);

                    _parkingRepository.SaveChanges();
                }
            }

            Timer.RequestedDT = DateTime.Now;

            return carparkFromRepo;
        }

        [HttpGet("{id}/availability")]
        public IActionResult GetCarparkAvailability(Guid id)
        {
            var carparkFromRepo = _parkingRepository.GetCarpark(id);

            if (carparkFromRepo == null)
            {
                return NotFound();
            }

            var minutes = (DateTime.Now - Timer.RequestedDT).TotalMinutes;

            if (minutes > 1)
            {
                if (carparkFromRepo.AgencyType == "HDB")
                {
                    carparkFromRepo = UpdateHDBAvailability(carparkFromRepo);

                    var carpark = _mapper.Map<CarparkDto>(carparkFromRepo);

                    return Ok(carpark);
                }
            }
            else
            {
                var carpark = _mapper.Map<CarparkDto>(carparkFromRepo);

                return Ok(carpark);
            }

            return BadRequest();
        }
    }
}