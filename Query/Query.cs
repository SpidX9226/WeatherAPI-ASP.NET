using WeatherAPI.Models;
using WeatherAPI.Services;

public partial class Query
{
    public async Task<WeatherForecast?> GetByLatAndLonAsync(double lat, double lon, [Service] WeatherService weatherService)
    {
        return await weatherService.GetByLatAndLonAsync(lat, lon);
    }

    public async Task<WeatherForecast?> GetWeatherByCityAndCountryAsync(string city, string countryCode, [Service] WeatherService weatherService)
    {
        return await weatherService.GetByCityAndCountryAsync(city, countryCode);
    }
}