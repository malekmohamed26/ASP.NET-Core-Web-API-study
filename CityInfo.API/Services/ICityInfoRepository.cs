using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync(); //To get all cities asynchronously
        Task<(IEnumerable<City>, PaginationMetadata)> GetCitiesAsync(
            string? name, string? searchQuery, int pageNumber, int pageSize); 
        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest);//To get one city asynchronously, also the result can be nullable
        Task<bool> CityExistsAsync(int cityId); //To check if city exists (asynchronously)
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestForCityAsync(int cityId); //To get all points of interest asynchronously
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId,
            int pointOfInterestId); //To get one point of interest for one city with cityId asynchronously
        Task AddPointOfInterestForCityAsync(int cityId, PointOfInterest pointOfInterest);
        void DeletePointOfInterest(PointOfInterest pointOfInterest);
        Task<bool> SaveChangesAsync();
    }
}
