using CityInfoAPI.Abstraction;
using CityInfoAPI.DbContexts;
using CityInfoAPI.Entities;
using CityInfoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CityInfoAPI.Services
{
    public class CityInfoRepository : ICityInfoRepository
    {
        private readonly CityInfoContext _context;

        public CityInfoRepository(CityInfoContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<City>> GetCitiesAsync(bool includePointOfIntrest)
        {
            if (includePointOfIntrest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest).OrderBy(c => c.Name).ToListAsync();
            }
            return await _context.Cities.OrderBy(c => c.Name).ToListAsync();
        }

        public async Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(string? name, string? searchQuery, bool includePointOfIntrest,
            int pageNumber, int pageSize)
        {

            var collection = _context.Cities as IQueryable<City>;

            if (!string.IsNullOrEmpty(name))
            {
                name = name.Trim();
                collection = collection.Include(c => c.PointsOfInterest).Where(c => c.Name == name);
            }

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.Trim();
                collection = collection.Include(c => c.PointsOfInterest)
                    .Where(c => c.Name.Contains(searchQuery)
                    || c.Description!=null && c.Description.Contains(searchQuery));
            }

            var totalItemCount = await collection.CountAsync();
            var paginationMetadata = new PaginationMetadata(totalItemCount, pageSize, pageNumber);


            var collectionToReturn = await collection.OrderBy(c => c.Name)
                .Skip(pageSize * (pageNumber - 1 ))
                .Take(pageSize)
                .ToListAsync();

            return (collectionToReturn, paginationMetadata);
        }

        public async Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest)
        {
            if (includePointsOfInterest)
            {
                return await _context.Cities.Include(c => c.PointsOfInterest)
                    .Where(c => c.Id == cityId).FirstOrDefaultAsync();
            }
            return await _context.Cities
                  .Where(c => c.Id == cityId).FirstOrDefaultAsync();
        }

        public async Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId)
        {
            return await _context.PointsOfInterest
                  .Where(p => p.CityId == cityId && p.Id == pointOfInterestId)
                  .FirstOrDefaultAsync();
        }

        public async Task<bool> CityExistsAsync(int cityId)
        {
            return await _context.Cities.AnyAsync(p => p.Id == cityId);
        }

        public async Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId)
        {
            return await _context.PointsOfInterest
                  .Where(p => p.CityId == cityId)
                  .ToListAsync();
        }

        public async Task AddAPointOfIntrestToACityAsync(int cityId, PointOfInterest pointOfInterest)
        {
            var city = await GetCityAsync(cityId, false);
            if (city != null)
            {
                city.PointsOfInterest.Add(pointOfInterest);
            }
        }

        public void DeletePointOfInterest(PointOfInterest pointOfInterest)
        {
            _context.PointsOfInterest.Remove(pointOfInterest);
        }

        public async Task<bool> SaveChnagesAsync()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }



    }
}
