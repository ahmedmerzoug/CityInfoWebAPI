using AutoMapper;
using CityInfoAPI.Models;

namespace CityInfoAPI.Profiles
{
    public class CityProfile : Profile
    {
        public CityProfile()
        {
            CreateMap<Entities.City, CityWithoutPointsOfInterestDto>();
            CreateMap<Entities.City, CityDto>();
        }
    }
}
