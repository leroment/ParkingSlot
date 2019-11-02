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
using System.Collections;
using System.Globalization;

namespace ParkingSlotAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class CalculationController : ControllerBase
	{
		private readonly ICarparkRatesRepository ICarparkRateRepository;
		private readonly IMapper _mapper;

		public CalculationController(ICarparkRatesRepository calRepository, IMapper mapper)
		{
			ICarparkRateRepository = calRepository;
			_mapper = mapper;
		}

		[HttpGet("{id}")]
		public ActionResult Index(Guid id, [FromQuery] DateTime StartTime, [FromQuery] DateTime EndTime, [FromQuery] String vehicleType)
		{
			var duration = 0.0;
		
			double Price = 0;
			Boolean change = false;
			Boolean IsNUll = false,  NonExistenceCarparkRate= false;
			DateTime RateStartTimeFromDB = new DateTime(1, 1, 1); ;
				DateTime RateEndTimeFromDB= new DateTime(1, 1, 1);
			var carpark = ICarparkRateRepository.GetCarparkRateById(id, vehicleType);
			List<CarparkRate> CarParkRateList = carpark.ToList();

			if (CarParkRateList.Count!=0) { 

			

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



			var result = new Calculation(StartTime, (int)duration);
			List<HoursPerDay> dayOfWeek = result.getparkingDay(StartTime, EndTime);

			
			foreach (HoursPerDay EachHoursPerDay in dayOfWeek)
			{




				if (EachHoursPerDay.getDay() > 0 && EachHoursPerDay.getDay() < 6)
				{
						//weekdays calculation
						double TimeChecker = EachHoursPerDay.getDayDuration();
						Boolean checkAllIfFailed = false;
						while (TimeChecker != 0)
						{
							for (int i = 0; i < CarParkRateList.Count; i++)
							{
								RateStartTimeFromDB = DateTime.ParseExact(StartTime.Day + "/" + StartTime.Month + "/" + StartTime.Year + " " + CarParkRateList[i].StartTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
								RateEndTimeFromDB = DateTime.ParseExact(EndTime.Day + "/" + EndTime.Month + "/" + EndTime.Year + " " + CarParkRateList[i].EndTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
								double durationOfStaticTimeInMin = result.getPeriodDuration(RateStartTimeFromDB, RateEndTimeFromDB);
								double durationOfDynamicTimeInMin = result.getPeriodDuration(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay());


								if (RateStartTimeFromDB.TimeOfDay == EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay &&
									durationOfDynamicTimeInMin <= durationOfStaticTimeInMin && TimeChecker > 0)
								{
									result.setDuration((int)EachHoursPerDay.getDayDuration());
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].WeekdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].WeekdayMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= durationOfDynamicTimeInMin;
									checkAllIfFailed = false;
								}

								else if (TimeChecker > 0 && result.TimePeriodOverlaps(StartTime, EndTime, RateStartTimeFromDB, RateEndTimeFromDB) == true)
								{


									result.setDuration((int)(EndTime - StartTime).TotalMinutes);
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].WeekdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].WeekdayMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= (int)((EndTime - StartTime).TotalMinutes);
									EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (EndTime - StartTime).TotalMinutes);
									change = false;
									checkAllIfFailed = false;
								}
								else if (TimeChecker > 0 && result.TimePeriodOverlapsRight(StartTime, EndTime, RateStartTimeFromDB, RateEndTimeFromDB) == true)
								{
									double redundantValue = (double)(EndTime - RateEndTimeFromDB).TotalMinutes;
									result.setDuration((int)((EndTime - StartTime).TotalMinutes - redundantValue));

									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].WeekdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].WeekdayMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= ((int)((EndTime - StartTime).TotalMinutes - redundantValue));
									EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - ((EndTime - StartTime).TotalMinutes - redundantValue));
									EachHoursPerDay.setStartTimeOfTheDay(EachHoursPerDay.getStartTimeOfTheDay().AddMinutes(((EndTime - StartTime).TotalMinutes - redundantValue)));
									change = false;
									checkAllIfFailed = false;

								}
								if (result.TimePeriodOverlaps(StartTime, EndTime, RateStartTimeFromDB, RateEndTimeFromDB) == false)
								{
									checkAllIfFailed = true;


								}

							}
							if (checkAllIfFailed == true)
							{
								NonExistenceCarparkRate = true;

								break;
							}
						}


					}
				else if (EachHoursPerDay.getDay() == 6)
				{
						//Sat calculation
						double TimeChecker = EachHoursPerDay.getDayDuration();
						Boolean checkAllIfFailed = false;
						while (TimeChecker != 0)
						{
							for (int i = 0; i < CarParkRateList.Count; i++)
							{
								RateStartTimeFromDB = DateTime.ParseExact(StartTime.Day + "/" + StartTime.Month + "/" + StartTime.Year + " " + CarParkRateList[i].StartTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
								RateEndTimeFromDB = DateTime.ParseExact(EndTime.Day + "/" + EndTime.Month + "/" + EndTime.Year + " " + CarParkRateList[i].EndTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
								double durationOfStaticTimeInMin = result.getPeriodDuration(RateStartTimeFromDB, RateEndTimeFromDB);
								double durationOfDynamicTimeInMin = result.getPeriodDuration(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay());


								if (RateStartTimeFromDB.TimeOfDay == EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay &&
									durationOfDynamicTimeInMin <= durationOfStaticTimeInMin && TimeChecker > 0)
								{
									result.setDuration((int)EachHoursPerDay.getDayDuration());
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].SunPHRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].SunPHMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= durationOfDynamicTimeInMin;
									checkAllIfFailed = false;
								}

								else if (TimeChecker > 0 && result.TimePeriodOverlaps(StartTime, EndTime, RateStartTimeFromDB, RateEndTimeFromDB) == true)
								{


									result.setDuration((int)(EndTime - StartTime).TotalMinutes);
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].SunPHRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].SunPHMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= (int)((EndTime - StartTime).TotalMinutes);
									EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (EndTime - StartTime).TotalMinutes);
									change = false;
									checkAllIfFailed = false;
								}
								else if (TimeChecker > 0 && result.TimePeriodOverlapsRight(StartTime, EndTime, RateStartTimeFromDB, RateEndTimeFromDB) == true)
								{
									double redundantValue = (double)(EndTime - RateEndTimeFromDB).TotalMinutes;
									result.setDuration((int)((EndTime - StartTime).TotalMinutes - redundantValue));

									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].SunPHRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].SunPHMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= ((int)((EndTime - StartTime).TotalMinutes - redundantValue));
									EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - ((EndTime - StartTime).TotalMinutes - redundantValue));
									EachHoursPerDay.setStartTimeOfTheDay(EachHoursPerDay.getStartTimeOfTheDay().AddMinutes(((EndTime - StartTime).TotalMinutes - redundantValue)));
									change = false;
									checkAllIfFailed = false;

								}
								if (result.TimePeriodOverlaps(StartTime, EndTime, RateStartTimeFromDB, RateEndTimeFromDB) == false)
								{
									checkAllIfFailed = true;


								}

							}
							if (checkAllIfFailed == true)
							{
								NonExistenceCarparkRate = true;

								break;
							}
						}
					}
				else
				{
					//SUN and PH calculation

					double TimeChecker = EachHoursPerDay.getDayDuration();
						Boolean checkAllIfFailed = false;
						while (TimeChecker != 0)
						{
							for (int i = 0; i < CarParkRateList.Count; i++)
							{
								RateStartTimeFromDB = DateTime.ParseExact(StartTime.Day + "/" + StartTime.Month + "/" + StartTime.Year + " " + CarParkRateList[i].StartTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
								RateEndTimeFromDB = DateTime.ParseExact(EndTime.Day + "/" + EndTime.Month + "/" + EndTime.Year + " " + CarParkRateList[i].EndTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
								double durationOfStaticTimeInMin = result.getPeriodDuration(RateStartTimeFromDB, RateEndTimeFromDB);
								double durationOfDynamicTimeInMin = result.getPeriodDuration(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay());


								if (RateStartTimeFromDB.TimeOfDay == EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay &&
									durationOfDynamicTimeInMin <= durationOfStaticTimeInMin && TimeChecker > 0)
								{
									result.setDuration((int)EachHoursPerDay.getDayDuration());
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[0].SunPHRate.Trim('$')), Convert.ToDouble(CarParkRateList[0].SunPHMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= durationOfDynamicTimeInMin;
									checkAllIfFailed = false;
								}

								else if (TimeChecker > 0 && result.TimePeriodOverlaps(StartTime, EndTime, RateStartTimeFromDB, RateEndTimeFromDB) == true)
								{


									result.setDuration((int)(EndTime - StartTime).TotalMinutes);
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[0].SunPHRate.Trim('$')), Convert.ToDouble(CarParkRateList[0].SunPHMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= (int)((EndTime - StartTime).TotalMinutes);
							EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (EndTime - StartTime).TotalMinutes);
							change = false;
									checkAllIfFailed = false;

								}
								else if (TimeChecker > 0 && result.TimePeriodOverlapsRight(StartTime, EndTime, RateStartTimeFromDB, RateEndTimeFromDB) == true)
								{
									double redundantValue =(double) ( EndTime- RateEndTimeFromDB ).TotalMinutes;
									result.setDuration((int)((EndTime - StartTime).TotalMinutes- redundantValue));
									
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[0].SunPHRate.Trim('$')), Convert.ToDouble(CarParkRateList[0].SunPHMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= ((int)((EndTime - StartTime).TotalMinutes - redundantValue));
									EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - ((EndTime - StartTime).TotalMinutes - redundantValue));
									EachHoursPerDay.setStartTimeOfTheDay(EachHoursPerDay.getStartTimeOfTheDay().AddMinutes(((EndTime - StartTime).TotalMinutes - redundantValue)));
									change = false;
									checkAllIfFailed = false;


								}

								if (result.TimePeriodOverlaps(StartTime, EndTime, RateStartTimeFromDB, RateEndTimeFromDB) == false)
								{
									checkAllIfFailed = true;

									
								}

							}
							if (checkAllIfFailed == true)
							{
								NonExistenceCarparkRate = true;
								
								break;
							}
						}
				}
			}


			}
			else
			{
				IsNUll = true;
				Price = 0;
			}

			//return Ok(dayOfWeek);
			return Ok(new { id, duration, Price, StartTime, EndTime, vehicleType, IsNUll, NonExistenceCarparkRate });


			//return Ok(new {  Price });

			return Ok(carpark);
		
	}
	}

	}