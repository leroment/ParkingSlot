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



            // get the carpark rates from the specified carpark
            var carparkRatesRecords = _carparkRatesRepository.GetCarparkRateById(id, vehicleType);

            // start time component
            var startTimeDayComponent = StartTime.DayOfWeek;
            var startTimeHourComponent = StartTime.Hour;
            var startTimeMinuteComponent = StartTime.Minute;


            // end time component
            var endTimeDayComponent = StartTime.DayOfWeek;
            var endTimeHourComponent = StartTime.Hour;
            var endTimeMinuteComponent = StartTime.Minute;

            // get duration
            var duration = EndTime - StartTime;

            // filter the carpark rates records where starttime is bigger or equal to record's starttime.
            carparkRatesRecords = carparkRatesRecords.Where(a => StartTime.TimeOfDay >= DateTime.Parse(a.StartTime).TimeOfDay && EndTime.TimeOfDay <= DateTime.Parse(a.EndTime).TimeOfDay);
            


            
            return 0.00;
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

            //filtering of price
            //if start date time and end date time are specified.
            if (carparkResourceParameters.StartDateTime != DateTime.MinValue || carparkResourceParameters.EndDateTime != DateTime.MinValue)
            {
                var carparks = collectionBeforePaging.ToList();


                carparks.ForEach(a =>
                {
                    if (carparkResourceParameters.VehType == "All")
                    {
                        double carPrice = 0.0, hvPrice = 0.0, mPrice = 0.0;

                        var asplit = a.LotType.Split(",");

                        foreach(var x in asplit)
                        {
                            if (x.Contains("C"))
                            {
                                carPrice = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), "Car");
                            }
                            else if (x.Contains("M"))
                            {
                                mPrice = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), "Motorcycle");
                            }
                            else if (x.Contains("H"))
                            {
                                hvPrice = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), "Heavy Vehicle");
                            }
                        }

                        if (asplit.Length == 1)
                        {
                            if (carPrice != 0.0)
                            {
                                a.Price = carPrice;
                            }
                            else if (mPrice != 0.0)
                            {
                                a.Price = mPrice;
                            }
                            else if (hvPrice != 0.0)
                            {
                                a.Price = hvPrice;
                            }
                        }
                        else if (asplit.Length == 2)
                        {
                            if (carPrice != 0.0 && mPrice != 0.0)
                            {
                                a.Price = Math.Min(carPrice, mPrice);
                            }
                            else if (carPrice != 0.0 && hvPrice != 0.0)
                            {
                                a.Price = Math.Min(carPrice, hvPrice);
                            }
                            else if (mPrice != 0.0 && hvPrice != 0.0)
                            {
                                a.Price = Math.Min(mPrice, hvPrice);
                            }
                        }
                        else if (asplit.Length == 3)
                        {
                            a.Price = Math.Min(Math.Min(carPrice, mPrice), hvPrice);
                        }
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

                        var asplit = a.LotType.Split(",");
                        foreach (var x in asplit)
                        {
                            if (x.Contains("C"))
                            {
                                carPrice = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), "Car");
                            }
                            else if (x.Contains("M"))
                            {
                                mPrice = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), "Motorcycle");
                            }
                            else if (x.Contains("H"))
                            {
                                hvPrice = CalculateCarparkPrice(a.Id, DateTime.Now, DateTime.Now.AddHours(1), "Heavy Vehicle");
                            }
                        }

                        if (asplit.Length == 1)
                        {
                            if (carPrice != 0.0)
                            {
                                a.Price = carPrice;
                            }
                            else if (mPrice != 0.0)
                            {
                                a.Price = mPrice;
                            }
                            else if (hvPrice != 0.0)
                            {
                                a.Price = hvPrice;
                            }
                        }
                        else if (asplit.Length == 2)
                        {
                            if (carPrice != 0.0 && mPrice != 0.0)
                            {
                                a.Price = Math.Min(carPrice, mPrice);
                            }
                            else if (carPrice != 0.0 && hvPrice != 0.0)
                            {
                                a.Price = Math.Min(carPrice, hvPrice);
                            }
                            else if (mPrice != 0.0 && hvPrice != 0.0)
                            {
                                a.Price = Math.Min(mPrice, hvPrice);
                            }
                        }
                        else if (asplit.Length == 3)
                        {
                            a.Price = Math.Min(Math.Min(carPrice, mPrice), hvPrice);
                        }

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
