using WeatherAPI.Models;

public partial class Query
{
    public async Task<WeatherForecast?> GetWeatherByLatAndLonAsync(double lat, double lon, [Service] IWeatherService weatherService)
    {
        return await weatherService.GetByLatAndLonAsync(lat, lon);
    }

    public async Task<WeatherForecast?> GetWeatherByCityAndCountryAsync(string city, string countryCode, [Service] IWeatherService weatherService)
    {
        return await weatherService.GetByCityAndCountryAsync(city, countryCode);
    }
}