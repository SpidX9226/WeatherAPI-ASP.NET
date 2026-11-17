using WeatherAPI.Models;

public interface IWeatherService
{
    public Task<WeatherForecast> GetByLatAndLonAsync(double lat, double lon);
    public Task<WeatherForecast> GetByCityAndCountryAsync(string city, string countryCode);
}