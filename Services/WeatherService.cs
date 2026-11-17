using System.Net;
using System.Runtime.Serialization;
using HotChocolate.Execution;
using WeatherAPI.Models;

namespace WeatherAPI.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;

    public WeatherService(HttpClient httpClient, IConfiguration config)
    {
        _httpClient = httpClient;
        _config = config;
    }

    public async Task<WeatherForecast> GetByLatAndLonAsync(double lat, double lon)
    {
        string? API_KEY = _config["WEATHER_API_KEY"];

        if (string.IsNullOrWhiteSpace(API_KEY))
        {
            throw new InvalidDataException("API_KEY is not configured.");
        }

        string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={API_KEY}";

        OpenWeatherResponse? response;

        try {
            response = await _httpClient.GetFromJsonAsync<OpenWeatherResponse>(url);
        } catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound) {
            throw new InvalidDataContractException($"Weather for latitude: {lat} and/or longitude: {lon} was not found by OpenWeather.", ex);
        } catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.BadRequest) {
            throw new BadHttpRequestException($"Latitude: {lat} and/or longitude: {lon} is invalid for request.", ex);
        }

        if (response is null)
        {
            throw new InvalidOperationException("OpenWeather returned an empty response body.");
        }

        return FromWeatherResponse(response);
    }

    public async Task<WeatherForecast> GetByCityAndCountryAsync(string city, string countryCode)
    {
        string safeCity = Uri.EscapeDataString(city.Trim());
        string safeCountryCode = Uri.EscapeDataString(countryCode.Trim());
        string? API_KEY = _config["WEATHER_API_KEY"];

        if (string.IsNullOrWhiteSpace(API_KEY))
        {
            throw new InvalidDataException("API_KEY is not configured.");
        }

        string openWeatherUrl = $"https://api.openweathermap.org/data/2.5/weather?q={safeCity},{safeCountryCode}&appid={API_KEY}";

        OpenWeatherResponse? response;

        try {
            response = await _httpClient.GetFromJsonAsync<OpenWeatherResponse>(openWeatherUrl);
        } catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.NotFound){
            throw new InvalidOperationException($"City '{city}, {countryCode}' was not found by OpenWeather.", ex);
        }

        if (response is null)
        {
            throw new InvalidOperationException("OpenWeather returned an empty response body.");
        }

        return FromWeatherResponse(response);
    }

    private WeatherForecast FromWeatherResponse(OpenWeatherResponse response)
    {
        ArgumentNullException.ThrowIfNull(response);

        var utcOffset = TimeSpan.FromSeconds(response.Timezone);

        static DateTimeOffset ToLocalTime(long unixSeconds, TimeSpan offset) =>
            DateTimeOffset.FromUnixTimeSeconds(unixSeconds).ToOffset(offset);

        var weather = response.Weather.FirstOrDefault();

        return new WeatherForecast
        {
            Country  = response.Sys.Country,
            City     = response.City,

            Title    = weather?.Title ?? string.Empty,
            Desc     = weather?.Desc  ?? string.Empty,
            IconCode = weather?.Icon  ?? string.Empty,

            Temp      = response.Forecast.Temp,
            FeelsLike = response.Forecast.FeelsLike,
            TempMin   = response.Forecast.TempMin,
            TempMax   = response.Forecast.TempMax,
            Humidity  = response.Forecast.Humidity,

            WindSpeed = response.Wind.Speed,
            WindDeg   = response.Wind.Deg,
            WindGust  = response.Wind.Gust,

            RainInfo = response.Rain?.OneHour,
            SnowInfo = response.Snow?.OneHour,

            Cloudiness = response.Clouds.Cloudiness,

            DateTime = ToLocalTime(response.UnixTime,     utcOffset),
            Sunrise  = ToLocalTime(response.Sys.Sunrise,  utcOffset),
            Sunset   = ToLocalTime(response.Sys.Sunset,   utcOffset),

            UtcOffset = utcOffset
        };
    }
}