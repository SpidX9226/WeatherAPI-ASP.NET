using dotenv.net;
using StackExchange.Redis;
using WeatherAPI.Services;

namespace WeatherAPI;

internal sealed class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        var envVars = DotEnv.Read();
        
        builder.Configuration.AddInMemoryCollection(
            envVars
                .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
                .Select(pair => new KeyValuePair<string, string?>(pair.Key, pair.Value))
        );


        builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect("localhost"));
        builder.Services.AddHttpClient<IWeatherService, WeatherService>();
  

        builder
            .AddGraphQL()
            .AddQueryType<Query>()
            .ModifyRequestOptions(opt => opt.IncludeExceptionDetails = builder.Environment.IsDevelopment());

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            
        }

        app.MapGraphQL();
        app.Run();
    }
}