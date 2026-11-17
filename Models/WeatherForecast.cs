namespace WeatherAPI.Models;

public sealed class WeatherForecast
{
    public string Country           { get; set; } = string.Empty;
    public string City              { get; set; } = string.Empty;

    public string Title             { get; set; } = string.Empty;
    public string Desc              { get; set; } = string.Empty;
    public string IconCode          { get; set; } = string.Empty;

    public double Temp              { get; set; }
    public double FeelsLike         { get; set; }
    public double TempMin           { get; set; }
    public double TempMax           { get; set; }
    public int Humidity /* % */     { get; set; }

    public double WindSpeed         { get; set; }
    public int WindDeg              { get; set; }
    public double WindGust          { get; set; }

    public double? RainInfo         { get; set; }
    public double? SnowInfo         { get; set; }
    
    public int Cloudiness /* % */   { get; set; } 

    public DateTimeOffset DateTime  { get; set; }
    public DateTimeOffset Sunrise   { get; set; }
    public DateTimeOffset Sunset    { get; set; }

    public TimeSpan UtcOffset       { get; set; } 
}