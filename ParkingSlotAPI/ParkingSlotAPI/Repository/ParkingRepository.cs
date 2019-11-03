using AutoMapper;
using ParkingSlotAPI.Controllers;
using ParkingSlotAPI.Database;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Helpers;
using ParkingSlotAPI.Models;
using ParkingSlotAPI.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Repository
{
    public interface IParkingRepository
    {
        PagedList<Carpark> GetCarparks(CarparkResourceParameters carparkResourceParameters);
        Carpark GetCarpark(Guid carparkId);
        IEnumerable<Carpark> GetAllCarparks();
        void AddCarpark(Carpark carpark);
        void DeleteCarpark(Carpark carpark);
        void UpdateCarpark(Carpark carpark);
        void UpdateCarparks(List<Carpark> carparks);
        void SaveChanges();
    }

    public class ParkingRepository : IParkingRepository
    {
        private ParkingContext _context;
        private IMapper _mapper;
        private ICarparkRatesRepository _carparkRatesRepository;
        public ParkingRepository(ParkingContext context, IMapper mapper, ICarparkRatesRepository carparkRatesRepository)
        {
            _context = context;
            _mapper = mapper;
            _carparkRatesRepository = carparkRatesRepository;
        }

        public IEnumerable<Carpark> GetAllCarparks()
        {
            return _context.Carparks.OrderBy(a => a.CarparkId);
        }

        public double CalculateCarparkPrice(Guid id, DateTime StartTime, DateTime EndTime, string vehicleType)
        {
            var duration = 0.0;

            double Price = 0;
            Boolean change = false;
            Boolean IsNUll = false, NonExistenceCarparkRate = false;
            DateTime RateStartTimeFromDB = new DateTime(1, 1, 1); ;
            DateTime RateEndTimeFromDB = new DateTime(1, 1, 1);
            var carpark = _carparkRatesRepository.GetCarparkRateById(id, vehicleType);
            List<CarparkRate> CarParkRateList = carpark.ToList();

            if (CarParkRateList.Count != 0)
            {



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
                                    double redundantValue = (double)(EndTime - RateEndTimeFromDB).TotalMinutes;
                                    result.setDuration((int)((EndTime - StartTime).TotalMinutes - redundantValue));

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

            return Price;
        }

        public PagedList<Carpark> GetCarparks(CarparkResourceParameters carparkResourceParameters)
        {
            var error = "";
            var collectionBeforePaging = _context.Carparks.AsQueryable();

            // filter agency type
            if (!string.IsNullOrEmpty(carparkResourceParameters.AgencyType))
            {
                var agencyTypeForWhereClause = carparkResourceParameters.AgencyType.Trim();

                if (agencyTypeForWhereClause == "HDB" || agencyTypeForWhereClause == "LTA" || agencyTypeForWhereClause == "URA")
                {
                    collectionBeforePaging = collectionBeforePaging.Where(a => a.AgencyType == agencyTypeForWhereClause);
                }
                else
                {
                    error += $"Cannot find the specified agency type {carparkResourceParameters.AgencyType}. ";
                }
            }

            // filter vehicle type
            if (!string.IsNullOrEmpty(carparkResourceParameters.VehType))
            {
                if (carparkResourceParameters.VehType == "Car")
                {
                    collectionBeforePaging = collectionBeforePaging.Where(a => a.LotType.Contains('C'));
                }
                else if (carparkResourceParameters.VehType == "Motorcycle")
                {
                    collectionBeforePaging = collectionBeforePaging.Where(a => a.LotType.Contains('M'));
                }
                else if (carparkResourceParameters.VehType == "Heavy Vehicle")
                {
                    collectionBeforePaging = collectionBeforePaging.Where(a => a.LotType.Contains('H'));
                }
                else if (carparkResourceParameters.VehType == "All")
                {
                    // do nothing.
                }
                else
                {
                    error += $"Cannot find the specified vehicle type {carparkResourceParameters.VehType}. ";
                }
            }
            
            // filtering of price
            // if start date time and end date time are specified.
            if (carparkResourceParameters.StartDateTime != null || carparkResourceParameters.EndDateTime != null)
            {
                var carparks = collectionBeforePaging.Skip((carparkResourceParameters.PageNumber - 1) * carparkResourceParameters.PageSize).Take(carparkResourceParameters.PageSize).ToList();


                carparks.ForEach(a =>
                {
                    if (carparkResourceParameters.VehType == "All")
                    {
                        double carPrice = 0.0, hvPrice = 0.0, mPrice = 0.0;
                        for (int i = 0; i <= 3; i++)
                        {
                            if (a.LotType.Contains("C"))
                            {
                                carPrice = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), "Car");
                            }
                            else if (a.LotType.Contains("M"))
                            {
                                mPrice = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), "Motorcycle");
                            }
                            else if (a.LotType.Contains("H"))
                            {
                                hvPrice = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), "Heavy Vehicle");
                            }
                        }

                        a.Price = Math.Min(Math.Min(carPrice, mPrice), hvPrice);

                    }
                    else
                    {
                        var price = CalculateCarparkPrice(a.Id, carparkResourceParameters.StartDateTime, carparkResourceParameters.EndDateTime, carparkResourceParameters.VehType);

                        a.Price = price;
                    }
                });

                // price is defaulted to max double, but if specified then it will set to lower or equal to that price.
                collectionBeforePaging = collectionBeforePaging.Where(a => a.Price <= carparkResourceParameters.Price).OrderBy(a => a.Price);

            }
            else
            // if datetime not specified.
            {
                var carparks = collectionBeforePaging.ToList();

                carparks.ForEach(a =>
                {
                    if (carparkResourceParameters.VehType == "All")
                    {
                        double carPrice = 0.0, hvPrice = 0.0, mPrice = 0.0;
                        for (int i = 0; i <= 3; i++)
                        {
                            if (a.LotType.Contains("C"))
                            {
                                carPrice = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), "Car");
                            }
                            else if (a.LotType.Contains("M"))
                            {
                                mPrice = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), "Motorcycle");
                            }
                            else if (a.LotType.Contains("H"))
                            {
                                hvPrice = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), "Heavy Vehicle");
                            }
                        }

                        a.Price = Math.Min(Math.Min(carPrice, mPrice), hvPrice);

                    }
                    else
                    {
                        // set start datetime to now and end datetime to one hour after now
                        var price = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), carparkResourceParameters.VehType);

                        a.Price = price;
                    }



                });

                // price is defaulted to max double, but if specified then it will set to lower or equal to that price.
                collectionBeforePaging = collectionBeforePaging.Where(a => a.Price <= carparkResourceParameters.Price).OrderBy(a => a.Price);
            }

            if (!string.IsNullOrEmpty(carparkResourceParameters.SearchQuery))
            {
                var searchQueryForWhereClause = carparkResourceParameters.SearchQuery
                    .Trim().ToLowerInvariant();

                collectionBeforePaging = collectionBeforePaging.Where(a => a.CarparkName.ToLowerInvariant().Contains(searchQueryForWhereClause));
            }

            if (error.Any())
            {
                throw new AppException(error);
            }

            return PagedList<Carpark>.Create(collectionBeforePaging, carparkResourceParameters.PageNumber, carparkResourceParameters.PageSize);
        }

        

        public Carpark GetCarpark(Guid carparkId)
        {
            return _context.Carparks.FirstOrDefault(a => a.Id == carparkId);
        }

        public Carpark GetCarparkById(string carparkId)
        {
            return _context.Carparks.FirstOrDefault(a => a.CarparkId == carparkId);
        }

        public void AddCarpark(Carpark carpark)
        {
            carpark.Id = Guid.NewGuid();
            _context.Carparks.Add(carpark);
        }

        public void UpdateCarpark(Carpark carpark)
        {
            _context.Carparks.Update(carpark);
        }

        public void UpdateCarparks(List<Carpark> carparks)
        {
            _context.Carparks.AddRange(carparks);
        }

        public void SaveChanges()
        {
            _context.SaveChanges();
        }

        public void DeleteCarpark(Carpark carpark)
        {
            _context.Carparks.Remove(carpark);
        }
    }
}
