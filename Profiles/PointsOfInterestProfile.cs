using AutoMapper;
using CityInfoAPI.Models;

namespace CityInfoAPI.Profiles
{
    public class PointsOfInterestProfile : Profile
    {
        public PointsOfInterestProfile()
        {
            CreateMap<Entities.PointOfInterest, PointOfInterestDto>();
            CreateMap<PointOfInterestForCreationDto, Entities.PointOfInterest>();
            CreateMap<PointOfInterestForUpdateDto, Entities.PointOfInterest>();
            CreateMap<Entities.PointOfInterest, PointOfInterestForUpdateDto>();
        }
    }
}
