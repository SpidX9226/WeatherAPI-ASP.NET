# WeatherAPI
A GraphQL weather API built with ASP.NET, Hot Chocolate and Redis

# 
|Technology         |Purpose                           |
|-------------------|----------------------------------|
|C#                 |Programming language              |
|.NET 10            |Application framework             |
|ASP.NET Core       |Web application framework         |
|Hot Chocolate 16   |GraphQL server                    |
|Redis              |Response caching                  |
|StackExchange.Redis|Redis client                      |
|OpenWeatherMap API |Weather data provider             |
|dotenv.net         |Environment variable configuration|

# Project Structure
``` bash
WeatherAPI/
├── .env                            # Environment variables (API keys)
├── appsettings.Development.json    # Development configuration (Ports, logging, etc.)
├── appsettings.json                # Base application configuration
├── LICENSE                         # Project license
├── Program.cs                      # Application entry point & service registration
├── WeatherAPI.csproj               # Project file (dependencies, target framework)
├── WeatherAPI.http                 # HTTP request samples for testing endpoints
├── WeatherAPI.slnx                 # Solution file
│
├── Models/
│   ├── OpenWeatherResponse.cs      # DTO mapping the external OpenWeather API response
│   └── WeatherForecast.cs          # Internal domain model for forecast data
│
├── Properties/
│   └── launchSettings.json         # Launch profiles (ports, environment variables)
│
├── Query/
│   └── Query.cs                    # Query parameters / request objects for weather lookups
│
└── Services/
    ├── IWeatherService.cs          # Interface defining weather service contract
    └── WeatherService.cs           # Implementation that calls OpenWeather API and maps results
```
