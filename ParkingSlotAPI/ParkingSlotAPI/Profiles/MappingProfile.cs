using AutoMapper;
using ParkingSlotAPI.Entities;
using ParkingSlotAPI.Models;
using ParkingSlotAPI.PublicAPIEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ParkingSlotAPI.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Carpark, CarparkDto>();
            CreateMap<Carpark, CarparkMarkerDto>();
            CreateMap<User, UserDto>();
            CreateMap<UserForCreationDto, User>();
            CreateMap<FavoriteForCreationDto, Favorite>();
            CreateMap<Favorite, FavoriteDto>();
        }
    }
}
