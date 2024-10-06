using CityInfo.API.Entities;

namespace CityInfo.API.Services
{
    public interface ICityInfoRepository
    {
        Task<IEnumerable<City>> GetCitiesAsync(); //To get all cities asynchronously
        Task<City?> GetCityAsync(int cityId, bool includePointsOfInterest); //To get one city asynchronously, also the result can be nullable
        Task<IEnumerable<PointOfInterest>> GetPointsOfInterestsForCityAsync(int cityId); //To get all R asynchronously
        Task<PointOfInterest?> GetPointOfInterestForCityAsync(int cityId, int pointOfInterestId); //To get one point of interest for one city with cityId asynchronously
    }
}
