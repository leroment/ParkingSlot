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
using ParkingSlotAPI.Services;
using ParkingSlotAPI.Entities;
namespace ParkingSlotAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CalculationController :ControllerBase
	{
		private readonly ICarparkRatesRepository ICarparkRateRepository;
		private readonly IMapper _mapper;

		public CalculationController(ICarparkRatesRepository calRepository, IMapper mapper)
		{
			ICarparkRateRepository = calRepository;
			_mapper = mapper;
		}


		// GET: Calculation
		[HttpGet]
		public ActionResult Index()
		{
			var carpark = ICarparkRateRepository.GetCarparkRates();
			//var carparkFromRepo = _parkingRepository.GetCarpark(id);
			var carparks = _mapper.Map<IEnumerable<CarparkRateDto>>(carpark);

			return Ok(carparks);
		}
		[HttpGet("{id}")]
		public ActionResult Index(Guid id, [FromQuery] DateTime StartTime, [FromQuery] DateTime EndTime, [FromQuery] String vehicleType )
		{
			
			var carpark = ICarparkRateRepository.GetCarparkRateById(id, vehicleType);
			List<CarparkRate> CarParkRateList = carpark.ToList();

			var duration = 0.0;
			int dayOfWeek = 0;
			if (EndTime.ToString() == "1/1/0001 12:00:00 AM" && StartTime.ToString() == "1/1/0001 12:00:00 AM")
			{
				duration = 60;
			}
			else if (EndTime.ToString() != "1/1/0001 12:00:00 AM" && StartTime.ToString() == "1/1/0001 12:00:00 AM")
			{
				StartTime = DateTime.Now;
				duration = (EndTime - StartTime).TotalMinutes;
			}
			else
			{
				duration = (EndTime - StartTime).TotalMinutes;
			}



			var result= new Calculation(StartTime,(int)duration);
						int day = result.parkingRate(StartTime);
						double dayRate;
						double  dayMin;
			
						if (day>0&&day<6)
						{


							dayRate = Convert.ToDouble(CarParkRateList[0].WeekdayRate.Trim('$'));
							dayMin = Convert.ToDouble(CarParkRateList[0].WeekdayMin.Trim('m','i','n','s'));

						}
						else if(day==6)
						{
							dayRate = Convert.ToDouble(CarParkRateList[0].SatdayRate.Trim('$'));
							dayMin = Convert.ToDouble(CarParkRateList[0].SatdayMin.Trim('m', 'i', 'n', 's'));
						}
						else
						{
							dayRate = Convert.ToDouble(CarParkRateList[0].SunPHRate.Trim('$'));
							dayMin = Convert.ToDouble(CarParkRateList[0].SunPHMin.Trim('m', 'i', 'n', 's'));
						}


						var Price = result.calculatePrice(dayRate, dayMin);
						

			return Ok(new { id, duration , Price, StartTime,EndTime });
			
		}

	}

}