using System.Diagnostics.Contracts;
using System.Text.Json.Serialization;

namespace WeatherAPI.Models;

public sealed class OpenWeatherResponse
{
    [JsonPropertyName("name")]
    public string City { get; set; } = string.Empty;
    [JsonPropertyName("weather")]
    public List<Weather> Weather { get; set; } = new();
    [JsonPropertyName("main")]
    public Forecast Forecast { get; set; } = null!;
    [JsonPropertyName("dt")]
    public long UnixTime { get; set; }
    [JsonPropertyName("wind")]
    public Wind Wind { get; set; } = null!;
    [JsonPropertyName("rain")]
    public Rain? Rain { get; set; }
    [JsonPropertyName("snow")]
    public Snow? Snow { get; set; }
    [JsonPropertyName("clouds")]
    public Clouds Clouds { get; set; } = null!;
    [JsonPropertyName("sys")]
    public Sys Sys { get; set; } = null!;
    [JsonPropertyName("timezone")]
    public int Timezone { get; set; }   
}

public sealed class Weather {
    [JsonPropertyName("main")]
    public string Title { get; set; } = string.Empty;
    [JsonPropertyName("description")]
    public string Desc { get; set; } = string.Empty;
    [JsonPropertyName("icon")]
    public string Icon { get; set; } = string.Empty;
}

public sealed class Forecast
{
    [JsonPropertyName("temp")]
    public double Temp { get; set; }
    [JsonPropertyName("feels_like")]
    public double FeelsLike { get; set; }
    [JsonPropertyName("temp_min")]
    public double TempMin { get; set; }
    [JsonPropertyName("temp_max")]
    public double TempMax { get; set; }
    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }
}

public class Rain
{
    [JsonPropertyName("1h")]
    public double OneHour { get; set; }
}

public class Snow
{
    [JsonPropertyName("1h")]
    public double OneHour { get; set; }
}

public class Wind
{
    [JsonPropertyName("speed")]
    public double Speed { get; set; }
    [JsonPropertyName("deg")]
    public int Deg { get; set; }
    [JsonPropertyName("gust")]
    public double Gust { get; set; }
}

public class Clouds
{
    [JsonPropertyName("all")]
    public int Cloudiness { get; set; }
}

public class Sys
{
    [JsonPropertyName("country")]
    public string Country { get; set; } = string.Empty;
    [JsonPropertyName("sunrise")]
    public long Sunrise {  get; set; }
    [JsonPropertyName("sunset")]
    public long Sunset { get; set; }
}