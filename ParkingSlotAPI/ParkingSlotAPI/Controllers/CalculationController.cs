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
	public class CalculationController :ControllerBase
	{
		private readonly ICarparkRatesRepository ICarparkRateRepository;
		private readonly IMapper _mapper;

		public CalculationController(ICarparkRatesRepository calRepository, IMapper mapper)
		{
			ICarparkRateRepository = calRepository;
			_mapper = mapper;
		}

		[HttpGet("{id}")]
		public ActionResult Index(Guid id, [FromQuery] DateTime StartTime, [FromQuery] DateTime EndTime, [FromQuery] String vehicleType )
		{
			
			var carpark = ICarparkRateRepository.GetCarparkRateById(id, vehicleType);
			List<CarparkRate> CarParkRateList = carpark.ToList();
			
			var duration = 0.0;
			
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
			List<HoursPerDay> dayOfWeek = result.getparkingDay(StartTime,EndTime);

			double startComparison = 1;
			double Price = 0;
			Boolean change = false;
				foreach (HoursPerDay EachHoursPerDay in dayOfWeek)
				{
				


					if (EachHoursPerDay.getDay() > 0 && EachHoursPerDay.getDay() < 6)
					{
					//weekdays calculation
						double TimeChecker = EachHoursPerDay.getDayDuration();

						while (TimeChecker!=0)
						{
							for(int i=0;i< CarParkRateList.Count;i++)
							{
							DateTime RateStartTimeFromDB = DateTime.ParseExact(CarParkRateList[i].StartTime, "HH:mm:ss", CultureInfo.InvariantCulture);
							DateTime RateEndTimeFromDB = DateTime.ParseExact(CarParkRateList[i].EndTime, "HH:mm:ss", CultureInfo.InvariantCulture);
							double durationOfStaticTimeInMin = result.getPeriodDuration(RateStartTimeFromDB, RateEndTimeFromDB);
							double durationOfDynamicTimeInMin= result.getPeriodDuration(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay());


							if (RateStartTimeFromDB.TimeOfDay== EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay&&
								durationOfDynamicTimeInMin <= durationOfStaticTimeInMin&& TimeChecker > 0)
								{
								result.setDuration((int)EachHoursPerDay.getDayDuration());
								Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].WeekdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].WeekdayMin.Trim('m', 'i', 'n', 's')));
								TimeChecker -= durationOfDynamicTimeInMin;

								}

								else if(TimeChecker > 0)
								{
								double updatedDurationOfStaticTimeInMin = result.getPeriodDurationBasedOnDayTime(EachHoursPerDay.getStartTimeOfTheDay(), RateEndTimeFromDB);
								if (updatedDurationOfStaticTimeInMin < 0)
								{
									updatedDurationOfStaticTimeInMin = updatedDurationOfStaticTimeInMin + 1440;
								}
								if (change != true)
								{
									 startComparison = result.getPeriodDurationBasedOnDayTime(RateStartTimeFromDB, EachHoursPerDay.getStartTimeOfTheDay());
								}
								
								double EndComparison = result.getPeriodDurationBasedOnDayTime(  EachHoursPerDay.getEndTimeOfTheDay(), RateEndTimeFromDB);
								if (startComparison>=0&& EndComparison>=0&&durationOfDynamicTimeInMin <= updatedDurationOfStaticTimeInMin&& (durationOfStaticTimeInMin - startComparison - EndComparison) <= durationOfStaticTimeInMin)
								{
									result.setDuration((int)(durationOfStaticTimeInMin-startComparison-EndComparison));
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].WeekdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].WeekdayMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= (durationOfStaticTimeInMin - startComparison - EndComparison);
									EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (durationOfStaticTimeInMin - startComparison - EndComparison));
									change = false;
								}
								else if(startComparison >= 0 && EndComparison < 0 && durationOfDynamicTimeInMin>updatedDurationOfStaticTimeInMin && (durationOfStaticTimeInMin - startComparison ) <= durationOfStaticTimeInMin)
								{
									result.setDuration((int)(durationOfStaticTimeInMin - startComparison));
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].WeekdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].WeekdayMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= (durationOfStaticTimeInMin - startComparison );
									EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (durationOfStaticTimeInMin - startComparison ));
									EachHoursPerDay.setStartTimeOfTheDay(EachHoursPerDay.getStartTimeOfTheDay().AddMinutes(durationOfStaticTimeInMin - startComparison ));
									change = false;
								}
								else if(startComparison < 0 && EndComparison >= 0 && durationOfDynamicTimeInMin <= updatedDurationOfStaticTimeInMin)
								{

									startComparison = startComparison + (24 * 60);
									change = true;
									break;
								}
								
							}
						}
							}
						

					}
					else if (EachHoursPerDay.getDay() == 6)
					{
					//Sat calculation
					double TimeChecker = EachHoursPerDay.getDayDuration();

					while (TimeChecker != 0)
					{
						for (int i = 0; i < CarParkRateList.Count; i++)
						{
							DateTime RateStartTimeFromDB = DateTime.ParseExact(CarParkRateList[i].StartTime, "HH:mm:ss", CultureInfo.InvariantCulture);
							DateTime RateEndTimeFromDB = DateTime.ParseExact(CarParkRateList[i].EndTime, "HH:mm:ss", CultureInfo.InvariantCulture);
							double durationOfStaticTimeInMin = result.getPeriodDuration(RateStartTimeFromDB, RateEndTimeFromDB);
							double durationOfDynamicTimeInMin = result.getPeriodDuration(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay());


							if (RateStartTimeFromDB.TimeOfDay == EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay &&
								durationOfDynamicTimeInMin <= durationOfStaticTimeInMin && TimeChecker > 0)
							{
								result.setDuration((int)EachHoursPerDay.getDayDuration());
								Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[0].SatdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[0].SatdayMin.Trim('m', 'i', 'n', 's')));
								TimeChecker -= durationOfDynamicTimeInMin;

							}

							else if (TimeChecker > 0)
							{
								
								double updatedDurationOfStaticTimeInMin = result.getPeriodDurationBasedOnDayTime(EachHoursPerDay.getStartTimeOfTheDay(), RateEndTimeFromDB);
								if (updatedDurationOfStaticTimeInMin<0)
								{
									updatedDurationOfStaticTimeInMin = updatedDurationOfStaticTimeInMin + 1440;
								}
									if (change != true)
								{
									startComparison = result.getPeriodDurationBasedOnDayTime(RateStartTimeFromDB, EachHoursPerDay.getStartTimeOfTheDay());
								}

								double EndComparison = result.getPeriodDurationBasedOnDayTime(EachHoursPerDay.getEndTimeOfTheDay(), RateEndTimeFromDB);
								if (startComparison >= 0 && EndComparison >= 0 && durationOfDynamicTimeInMin <= updatedDurationOfStaticTimeInMin && (durationOfStaticTimeInMin - startComparison - EndComparison) <= durationOfStaticTimeInMin)
								{
									result.setDuration((int)(durationOfStaticTimeInMin - startComparison - EndComparison));
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[0].SatdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[0].SatdayMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= (durationOfStaticTimeInMin - startComparison - EndComparison);
									EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (durationOfStaticTimeInMin - startComparison - EndComparison));
									change = false;
								}
								else if (startComparison >= 0 && EndComparison < 0 && durationOfDynamicTimeInMin > updatedDurationOfStaticTimeInMin && (durationOfStaticTimeInMin - startComparison ) <= durationOfStaticTimeInMin)
								{
									result.setDuration((int)(durationOfStaticTimeInMin - startComparison));
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[0].SatdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[0].SatdayMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= (durationOfStaticTimeInMin - startComparison);
									EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (durationOfStaticTimeInMin - startComparison));
									EachHoursPerDay.setStartTimeOfTheDay(EachHoursPerDay.getStartTimeOfTheDay().AddMinutes(durationOfStaticTimeInMin - startComparison));
									change = false;
								}
								else if (startComparison < 0 && EndComparison >= 0 && durationOfDynamicTimeInMin <= updatedDurationOfStaticTimeInMin)
								{

									startComparison = startComparison + (24 * 60);
									change = true;
									break;
								}

							}
						}
					}
				}
					else
					{
					//SUN and PH calculation

					double TimeChecker = EachHoursPerDay.getDayDuration();

					while (TimeChecker != 0)
					{
						for (int i = 0; i < CarParkRateList.Count; i++)
						{
							DateTime RateStartTimeFromDB = DateTime.ParseExact(CarParkRateList[i].StartTime, "HH:mm:ss", CultureInfo.InvariantCulture);
							DateTime RateEndTimeFromDB = DateTime.ParseExact(CarParkRateList[i].EndTime, "HH:mm:ss", CultureInfo.InvariantCulture);
							double durationOfStaticTimeInMin = result.getPeriodDuration(RateStartTimeFromDB, RateEndTimeFromDB);
							double durationOfDynamicTimeInMin = result.getPeriodDuration(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay());


							if (RateStartTimeFromDB.TimeOfDay == EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay &&
								durationOfDynamicTimeInMin <= durationOfStaticTimeInMin && TimeChecker > 0)
							{
								result.setDuration((int)EachHoursPerDay.getDayDuration());
								Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[0].SunPHRate.Trim('$')), Convert.ToDouble(CarParkRateList[0].SunPHMin.Trim('m', 'i', 'n', 's')));
								TimeChecker -= durationOfDynamicTimeInMin;

							}

							else if (TimeChecker > 0)
							{
								double updatedDurationOfStaticTimeInMin = result.getPeriodDurationBasedOnDayTime(EachHoursPerDay.getStartTimeOfTheDay(), RateEndTimeFromDB);
								if (updatedDurationOfStaticTimeInMin < 0)
								{
									updatedDurationOfStaticTimeInMin = updatedDurationOfStaticTimeInMin + 1440;
								}
								if (change != true)
								{
									startComparison = result.getPeriodDurationBasedOnDayTime(RateStartTimeFromDB, EachHoursPerDay.getStartTimeOfTheDay());
								}

								double EndComparison = result.getPeriodDurationBasedOnDayTime(EachHoursPerDay.getEndTimeOfTheDay(), RateEndTimeFromDB);
								if (startComparison >= 0 && EndComparison >= 0 && durationOfDynamicTimeInMin <= updatedDurationOfStaticTimeInMin && (durationOfStaticTimeInMin - startComparison - EndComparison) <= durationOfStaticTimeInMin)
								{
									result.setDuration((int)(durationOfStaticTimeInMin - startComparison - EndComparison));
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[0].SunPHRate.Trim('$')), Convert.ToDouble(CarParkRateList[0].SunPHMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= (durationOfStaticTimeInMin - startComparison - EndComparison);
									EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (durationOfStaticTimeInMin - startComparison - EndComparison));
									change = false;
								}
								else if (startComparison >= 0 && EndComparison < 0 && durationOfDynamicTimeInMin > updatedDurationOfStaticTimeInMin && (durationOfStaticTimeInMin - startComparison ) <= durationOfStaticTimeInMin)
								{
									result.setDuration((int)(durationOfStaticTimeInMin - startComparison));
									Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[0].SunPHRate.Trim('$')), Convert.ToDouble(CarParkRateList[0].SunPHMin.Trim('m', 'i', 'n', 's')));
									TimeChecker -= (durationOfStaticTimeInMin - startComparison);
									EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (durationOfStaticTimeInMin - startComparison));
									EachHoursPerDay.setStartTimeOfTheDay(EachHoursPerDay.getStartTimeOfTheDay().AddMinutes(durationOfStaticTimeInMin - startComparison));
									change = false;
								}
								else if (startComparison < 0 && EndComparison >= 0 && durationOfDynamicTimeInMin <= updatedDurationOfStaticTimeInMin)
								{

									startComparison = startComparison + (24 * 60);
									change = true;
									break;
								}

							}
						}
					}
				}
				}



			//return Ok(dayOfWeek);
		return Ok(new { id, duration , Price, StartTime,EndTime, vehicleType });
			
			
			//return Ok(new {  Price });

		}

	}

}