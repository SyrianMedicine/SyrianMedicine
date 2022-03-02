using AutoMapper;
using DAL.Entities;
using Models.Common;

namespace Services.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<City, OptionDto>();
        }

    }
}