using System.Net;
using System.Runtime.Serialization;
using System.Text.Json;
using StackExchange.Redis;
using WeatherAPI.Models;

namespace WeatherAPI.Services;

public class WeatherService : IWeatherService
{
    private readonly HttpClient _httpClient;
    private readonly IDatabase _redis;
    private readonly string _weatherApiKey;

    public WeatherService(HttpClient httpClient, IConnectionMultiplexer connectionMultiplexer, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _redis = connectionMultiplexer.GetDatabase();
        _weatherApiKey = configuration["WEATHER_API_KEY"] ?? throw new InvalidDataException("API_KEY is not configured.");
    }

    public async Task<WeatherForecast> GetByLatAndLonAsync(double lat, double lon)
    {
        string cacheKey = $"{lat}:{lon}";
        var cachedData = await _redis.StringGetAsync(cacheKey);

        if (!cachedData.IsNullOrEmpty)
        {
            var res = JsonSerializer.Deserialize<WeatherForecast>(cachedData.ToString());

            if (res is null)
            {
                throw new InvalidCastException("Unable to deserialize cached data.");
            }

            return res;
        }

        string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={_weatherApiKey}&units=metric";

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

        var result = FromWeatherResponse(response);

        var serializedData = JsonSerializer.Serialize(result);

        await _redis.StringSetAsync(cacheKey, serializedData, new Expiration(TimeSpan.FromMinutes(10)));

        return result;
    }

    public async Task<WeatherForecast> GetByCityAndCountryAsync(string city, string countryCode)
    {
        string normalizedCity = city.Trim().ToLower();
        string normalizedCountryCode = countryCode.Trim().ToUpper();

        string cacheKey = $"{normalizedCity}:{normalizedCountryCode}";

        var cachedData = await _redis.StringGetAsync(cacheKey);

        if (!cachedData.IsNullOrEmpty)
        {
            var res = JsonSerializer.Deserialize<WeatherForecast>(cachedData.ToString());
            
            if (res is null)
            {
                throw new InvalidCastException("Unable to deserialize cached data.");
            }

            return res;
        }

        string safeCity = Uri.EscapeDataString(normalizedCity);
        string safeCountryCode = Uri.EscapeDataString(normalizedCountryCode);

        string openWeatherUrl = $"https://api.openweathermap.org/data/2.5/weather?q={safeCity},{safeCountryCode}&appid={_weatherApiKey}&units=metric";

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

        var result = FromWeatherResponse(response);

        var serializedData = JsonSerializer.Serialize(result);

        await _redis.StringSetAsync(cacheKey, serializedData, new Expiration(TimeSpan.FromMinutes(10)));

        return result;
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