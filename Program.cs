using dotenv.net;
using WeatherAPI.Services;

namespace WeatherAPI;

internal sealed class Program {
    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        var envVars = DotEnv.Read();
                builder.Services.AddHttpClient();
        builder.Configuration.AddInMemoryCollection(
            envVars
                .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
                .Select(pair => new KeyValuePair<string, string?>(pair.Key, pair.Value))
        );
        builder.Services.AddHttpClient<WeatherService>();
  

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