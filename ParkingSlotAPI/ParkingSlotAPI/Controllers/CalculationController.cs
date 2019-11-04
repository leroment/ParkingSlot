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
			Boolean IsNUll = false,  NonExistenceCarparkRate= false, InvalidDate=false,FlatRateExistence=false;
			DateTime RateStartTimeFromDB = new DateTime(1, 1, 1); ;
			DateTime RateEndTimeFromDB= new DateTime(1, 1, 1);
			var carpark = ICarparkRateRepository.GetCarparkRateById(id, vehicleType);
			List<CarparkRate> CarParkRateList = carpark.ToList();

			

			

			if (EndTime.ToString() == "1/1/0001 12:00:00 AM" && StartTime.ToString() == "1/1/0001 12:00:00 AM")
			{
				duration = 60;
			}
			else if (EndTime.ToString() != "1/1/0001 12:00:00 AM" && StartTime.ToString() == "1/1/0001 12:00:00 AM")
			{
				StartTime = DateTime.Now;
				StartTime = new DateTime(StartTime.Year, StartTime.Month, StartTime.Day, StartTime.Hour, StartTime.Minute, 0);
				
					duration = (EndTime - StartTime).TotalMinutes;
			}
			else
			{
				duration = (EndTime - StartTime).TotalMinutes;
			}
			if (duration>=0) {
				if (CarParkRateList.Count != 0)
				{

					var result = new Calculation(StartTime, (int)duration);
					List<HoursPerDay> dayOfWeek = result.getparkingDay(StartTime, EndTime);


					foreach (HoursPerDay EachHoursPerDay in dayOfWeek)
					{




						if (EachHoursPerDay.getDay() > 0 && EachHoursPerDay.getDay() < 6)
						{
							//weekdays calculation
							if (vehicleType != "Motorcycle")
							{
								double TimeChecker = EachHoursPerDay.getDayDuration();

								while (TimeChecker != 0)
								{
									int checkAllIfFailed = 0;
									for (int i = 0; i < CarParkRateList.Count; i++)
									{
										if ((Convert.ToInt32(CarParkRateList[i].WeekdayMin.Trim('m', 'i', 'n', 's'))) <= 30)
										{
											double durationOfStaticTimeInMin = Convert.ToDouble(CarParkRateList[i].Duration) * 60;
											if (EachHoursPerDay.getEndTimeOfTheDay().TimeOfDay.TotalMinutes == 0)
											{
												RateStartTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getStartTimeOfTheDay().Day + "/" + EachHoursPerDay.getStartTimeOfTheDay().Month + "/" + EachHoursPerDay.getStartTimeOfTheDay().Year + " " + CarParkRateList[i].StartTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

												RateEndTimeFromDB = RateStartTimeFromDB.AddMinutes(durationOfStaticTimeInMin);
											}
											else if (EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay.TotalMinutes == 0)
											{

												RateEndTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getEndTimeOfTheDay().Day + "/" + EachHoursPerDay.getEndTimeOfTheDay().Month + "/" + EachHoursPerDay.getEndTimeOfTheDay().Year + " " + CarParkRateList[i].EndTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
												RateStartTimeFromDB = RateEndTimeFromDB.Subtract(new TimeSpan(0, (int)durationOfStaticTimeInMin, 0));
											}
											else
											{
												RateStartTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getStartTimeOfTheDay().Day + "/" + EachHoursPerDay.getStartTimeOfTheDay().Month + "/" + EachHoursPerDay.getStartTimeOfTheDay().Year + " " + CarParkRateList[i].StartTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
												RateEndTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getEndTimeOfTheDay().Day + "/" + EachHoursPerDay.getEndTimeOfTheDay().Month + "/" + EachHoursPerDay.getEndTimeOfTheDay().Year + " " + CarParkRateList[i].EndTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
											}


											double durationOfDynamicTimeInMin = result.getPeriodDuration(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay());


											if (RateStartTimeFromDB.TimeOfDay == EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay &&
												durationOfDynamicTimeInMin <= durationOfStaticTimeInMin && TimeChecker > 0)
											{
												result.setDuration((int)EachHoursPerDay.getDayDuration());
												Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].WeekdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].WeekdayMin.Trim('m', 'i', 'n', 's')));
												TimeChecker -= durationOfDynamicTimeInMin;
												checkAllIfFailed--;
											}

											else if (TimeChecker > 0 && result.TimePeriodOverlaps(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay(), RateStartTimeFromDB, RateEndTimeFromDB) == true)
											{


												result.setDuration((int)(EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes);
												Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].WeekdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].WeekdayMin.Trim('m', 'i', 'n', 's')));
												TimeChecker -= (int)(((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes));
												EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes);

												checkAllIfFailed--;

											}
											else if (TimeChecker > 0 && result.TimePeriodOverlapsRight(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay(), RateStartTimeFromDB, RateEndTimeFromDB) == true)
											{
												double redundantValue = (double)(EachHoursPerDay.getEndTimeOfTheDay() - RateEndTimeFromDB).TotalMinutes;
												result.setDuration((int)((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue));

												Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].WeekdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].WeekdayMin.Trim('m', 'i', 'n', 's')));
												TimeChecker -= ((int)((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue));
												EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - ((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue));
												EachHoursPerDay.setStartTimeOfTheDay(EachHoursPerDay.getStartTimeOfTheDay().AddMinutes(((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue)));

												checkAllIfFailed--;

											}


											if (result.TimePeriodOverlaps(StartTime, EndTime, RateStartTimeFromDB, RateEndTimeFromDB) == false)
											{
												checkAllIfFailed++;


											}
											if (TimeChecker == 0)
											{
												break;
											}
										}
										else if ((Convert.ToInt32(CarParkRateList[i].WeekdayMin.Trim('m', 'i', 'n', 's'))) > 30)
										{
											FlatRateExistence = true;
										}

									}
									if (checkAllIfFailed >= CarParkRateList.Count)
									{
										NonExistenceCarparkRate = true;

										break;
									}
									/*	else if(FlatRateExistence==true&& TimeChecker!=0)
										{

										}*/
								}
							}
							else
							{
								//////motorcycle 
								///

							}

						}
						else if (EachHoursPerDay.getDay() == 6)
						{
							//Sat calculation
							if (vehicleType != "Motorcycle")
							{
								double TimeChecker = EachHoursPerDay.getDayDuration();

								while (TimeChecker != 0)
								{
									int checkAllIfFailed = 0;
									for (int i = 0; i < CarParkRateList.Count; i++)
									{
										if ((Convert.ToInt32(CarParkRateList[i].SatdayMin.Trim('m', 'i', 'n', 's'))) <= 30)
										{
											double durationOfStaticTimeInMin = Convert.ToDouble(CarParkRateList[i].Duration) * 60;
											if (EachHoursPerDay.getEndTimeOfTheDay().TimeOfDay.TotalMinutes == 0)
											{
												RateStartTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getStartTimeOfTheDay().Day + "/" + EachHoursPerDay.getStartTimeOfTheDay().Month + "/" + EachHoursPerDay.getStartTimeOfTheDay().Year + " " + CarParkRateList[i].StartTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

												RateEndTimeFromDB = RateStartTimeFromDB.AddMinutes(durationOfStaticTimeInMin);
											}
											else if (EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay.TotalMinutes == 0)
											{

												RateEndTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getEndTimeOfTheDay().Day + "/" + EachHoursPerDay.getEndTimeOfTheDay().Month + "/" + EachHoursPerDay.getEndTimeOfTheDay().Year + " " + CarParkRateList[i].EndTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
												RateStartTimeFromDB = RateEndTimeFromDB.Subtract(new TimeSpan(0, (int)durationOfStaticTimeInMin, 0));
											}
											else
											{
												RateStartTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getStartTimeOfTheDay().Day + "/" + EachHoursPerDay.getStartTimeOfTheDay().Month + "/" + EachHoursPerDay.getStartTimeOfTheDay().Year + " " + CarParkRateList[i].StartTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
												RateEndTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getEndTimeOfTheDay().Day + "/" + EachHoursPerDay.getEndTimeOfTheDay().Month + "/" + EachHoursPerDay.getEndTimeOfTheDay().Year + " " + CarParkRateList[i].EndTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
											}
											double durationOfDynamicTimeInMin = result.getPeriodDuration(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay());


											if (RateStartTimeFromDB.TimeOfDay == EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay &&
												durationOfDynamicTimeInMin <= durationOfStaticTimeInMin && TimeChecker > 0)
											{
												result.setDuration((int)EachHoursPerDay.getDayDuration());
												Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].SatdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].SatdayMin.Trim('m', 'i', 'n', 's')));
												TimeChecker -= durationOfDynamicTimeInMin;
												checkAllIfFailed--;
											}

											else if (TimeChecker > 0 && result.TimePeriodOverlaps(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay(), RateStartTimeFromDB, RateEndTimeFromDB) == true)
											{


												result.setDuration((int)(EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes);
												Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].SatdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].SatdayMin.Trim('m', 'i', 'n', 's')));
												TimeChecker -= (int)(((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes));
												EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes);

												checkAllIfFailed--;

											}
											else if (TimeChecker > 0 && result.TimePeriodOverlapsRight(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay(), RateStartTimeFromDB, RateEndTimeFromDB) == true)
											{
												double redundantValue = (double)(EachHoursPerDay.getEndTimeOfTheDay() - RateEndTimeFromDB).TotalMinutes;
												result.setDuration((int)((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue));

												Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].SatdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].SatdayMin.Trim('m', 'i', 'n', 's')));
												TimeChecker -= ((int)((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue));
												EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - ((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue));
												EachHoursPerDay.setStartTimeOfTheDay(EachHoursPerDay.getStartTimeOfTheDay().AddMinutes(((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue)));

												checkAllIfFailed--;

											}


											if (result.TimePeriodOverlaps(StartTime, EndTime, RateStartTimeFromDB, RateEndTimeFromDB) == false)
											{
												checkAllIfFailed++;


											}
											if (TimeChecker == 0)
											{
												break;
											}
										}
										else if ((Convert.ToInt32(CarParkRateList[i].SatdayMin.Trim('m', 'i', 'n', 's'))) > 30)
										{
											FlatRateExistence = true;
										}


									}
									if (checkAllIfFailed >= CarParkRateList.Count)
									{
										NonExistenceCarparkRate = true;

										break;
									}
									/*	else if(FlatRateExistence==true&& TimeChecker!=0)
									{

									}*/
								}
							}
							else
							{
								//motorcycle
								for (int i = 0; i < CarParkRateList.Count; i++)
								{
									double durationOfStaticTimeInMin = Convert.ToDouble(CarParkRateList[i].Duration) * 60;
									if (EachHoursPerDay.getEndTimeOfTheDay().TimeOfDay.TotalMinutes == 0)
									{
										RateStartTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getStartTimeOfTheDay().Day + "/" + EachHoursPerDay.getStartTimeOfTheDay().Month + "/" + EachHoursPerDay.getStartTimeOfTheDay().Year + " " + CarParkRateList[i].StartTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

										RateEndTimeFromDB = RateStartTimeFromDB.AddMinutes(durationOfStaticTimeInMin);
									}
									else if (EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay.TotalMinutes == 0)
									{

										RateEndTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getEndTimeOfTheDay().Day + "/" + EachHoursPerDay.getEndTimeOfTheDay().Month + "/" + EachHoursPerDay.getEndTimeOfTheDay().Year + " " + CarParkRateList[i].EndTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
										RateStartTimeFromDB = RateEndTimeFromDB.Subtract(new TimeSpan(0, (int)durationOfStaticTimeInMin, 0));
									}
									else
									{
										RateStartTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getStartTimeOfTheDay().Day + "/" + EachHoursPerDay.getStartTimeOfTheDay().Month + "/" + EachHoursPerDay.getStartTimeOfTheDay().Year + " " + CarParkRateList[i].StartTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
										RateEndTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getEndTimeOfTheDay().Day + "/" + EachHoursPerDay.getEndTimeOfTheDay().Month + "/" + EachHoursPerDay.getEndTimeOfTheDay().Year + " " + CarParkRateList[i].EndTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
									}

									if ( result.TimePeriodOverlaps(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay(), RateStartTimeFromDB, RateEndTimeFromDB) == true)
									{


										/*result.setDuration((int)(EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes);
										Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].SatdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].SatdayMin.Trim('m', 'i', 'n', 's')));
										
										EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes);*/

										

									}
									else if ( result.TimePeriodOverlapsRight(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay(), RateStartTimeFromDB, RateEndTimeFromDB) == true)
									{
									/*	double redundantValue = (double)(EachHoursPerDay.getEndTimeOfTheDay() - RateEndTimeFromDB).TotalMinutes;
										result.setDuration((int)((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue));

										Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].SatdayRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].SatdayMin.Trim('m', 'i', 'n', 's')));
										
										EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - ((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue));
										EachHoursPerDay.setStartTimeOfTheDay(EachHoursPerDay.getStartTimeOfTheDay().AddMinutes(((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue)));*/
								

									}
								}

							}
						}
						else
						{
							//SUN and PH calculation
							if (vehicleType != "Motorcycle")
							{
								double TimeChecker = EachHoursPerDay.getDayDuration();

							while (TimeChecker != 0)
							{
								int checkAllIfFailed = 0;
								for (int i = 0; i < CarParkRateList.Count; i++)
								{

									if ((Convert.ToInt32(CarParkRateList[i].SunPHMin.Trim('m', 'i', 'n', 's'))) <= 30)
									{
										double durationOfStaticTimeInMin = Convert.ToDouble(CarParkRateList[i].Remarks) * 60;
										if (EachHoursPerDay.getEndTimeOfTheDay().TimeOfDay.TotalMinutes == 0)
										{
											RateStartTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getStartTimeOfTheDay().Day + "/" + EachHoursPerDay.getStartTimeOfTheDay().Month + "/" + EachHoursPerDay.getStartTimeOfTheDay().Year + " " + CarParkRateList[i].StartTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);

											RateEndTimeFromDB = RateStartTimeFromDB.AddMinutes(durationOfStaticTimeInMin);
										}
										else if (EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay.TotalMinutes == 0)
										{

											RateEndTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getEndTimeOfTheDay().Day + "/" + EachHoursPerDay.getEndTimeOfTheDay().Month + "/" + EachHoursPerDay.getEndTimeOfTheDay().Year + " " + CarParkRateList[i].EndTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
											RateStartTimeFromDB = RateEndTimeFromDB.Subtract(new TimeSpan(0, (int)durationOfStaticTimeInMin, 0));
										}
										else
										{
											RateStartTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getStartTimeOfTheDay().Day + "/" + EachHoursPerDay.getStartTimeOfTheDay().Month + "/" + EachHoursPerDay.getStartTimeOfTheDay().Year + " " + CarParkRateList[i].StartTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
											RateEndTimeFromDB = DateTime.ParseExact(EachHoursPerDay.getEndTimeOfTheDay().Day + "/" + EachHoursPerDay.getEndTimeOfTheDay().Month + "/" + EachHoursPerDay.getEndTimeOfTheDay().Year + " " + CarParkRateList[i].EndTime, "d/M/yyyy HH:mm:ss", CultureInfo.InvariantCulture);
										}
										double durationOfDynamicTimeInMin = result.getPeriodDuration(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay());


										if (RateStartTimeFromDB.TimeOfDay == EachHoursPerDay.getStartTimeOfTheDay().TimeOfDay &&
											durationOfDynamicTimeInMin <= durationOfStaticTimeInMin && TimeChecker > 0)
										{
											result.setDuration((int)EachHoursPerDay.getDayDuration());
											Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].SunPHRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].SunPHMin.Trim('m', 'i', 'n', 's')));
											TimeChecker -= durationOfDynamicTimeInMin;
											checkAllIfFailed--;
										}

										else if (TimeChecker > 0 && result.TimePeriodOverlaps(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay(), RateStartTimeFromDB, RateEndTimeFromDB) == true)
										{


											result.setDuration((int)(EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes);
											Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].SunPHRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].SunPHMin.Trim('m', 'i', 'n', 's')));
											TimeChecker -= (int)(((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes));
											EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - (EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes);

											checkAllIfFailed--;

										}
										else if (TimeChecker > 0 && result.TimePeriodOverlapsRight(EachHoursPerDay.getStartTimeOfTheDay(), EachHoursPerDay.getEndTimeOfTheDay(), RateStartTimeFromDB, RateEndTimeFromDB) == true)
										{
											double redundantValue = (double)(EachHoursPerDay.getEndTimeOfTheDay() - RateEndTimeFromDB).TotalMinutes;
											result.setDuration((int)((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue));

											Price += result.calculatePrice(Convert.ToDouble(CarParkRateList[i].SunPHRate.Trim('$')), Convert.ToDouble(CarParkRateList[i].SunPHMin.Trim('m', 'i', 'n', 's')));
											TimeChecker -= ((int)((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue));
											EachHoursPerDay.setDayDuration(EachHoursPerDay.getDayDuration() - ((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue));
											EachHoursPerDay.setStartTimeOfTheDay(EachHoursPerDay.getStartTimeOfTheDay().AddMinutes(((EachHoursPerDay.getEndTimeOfTheDay() - EachHoursPerDay.getStartTimeOfTheDay()).TotalMinutes - redundantValue)));

											checkAllIfFailed--;

										}


										if (result.TimePeriodOverlaps(StartTime, EndTime, RateStartTimeFromDB, RateEndTimeFromDB) == false)
										{
											checkAllIfFailed++;


										}
										if (TimeChecker == 0)
										{
											break;
										}
									}
									else if ((Convert.ToInt32(CarParkRateList[i].SunPHMin.Trim('m', 'i', 'n', 's'))) > 30)
									{
										FlatRateExistence = true;
									}

								}
								if (checkAllIfFailed >= CarParkRateList.Count)
								{
									NonExistenceCarparkRate = true;

									break;
								}
								/*	else if(FlatRateExistence==true&& TimeChecker!=0)
								{

								}*/
							}
						}
							else
							{
								// motorcycle
							}
						}

					}


				}
				else
				{
					IsNUll = true;
					Price = 0;
				}
			}
			else if (0<duration)
			{
				InvalidDate = true;
			}
		

			return Ok(new { id, duration, Price, StartTime, EndTime, vehicleType, IsNUll, NonExistenceCarparkRate,InvalidDate });


	
		
	}
	}

	}