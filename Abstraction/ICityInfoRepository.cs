using CityInfoAPI.Entities;
using CityInfoAPI.Models;

namespace CityInfoAPI.Abstraction
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync(bool includePointOfIntrest);
        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, bool includePointOfIntrest,
        int pageNumber, int pageSize);
        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId);
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId);
        Task<bool> CityExistsAsync(int cityId);
        Task AddAPointOfIntrestToACityAsync(int cityId, PointOfInterest pointOfInterest);
        void DeletePointOfInterest(PointOfInterest pointOfInterest);
        Task<bool> SaveChnagesAsync();

    }
}
