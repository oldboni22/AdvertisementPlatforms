using WebApiExtensions;

namespace FeedWebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.ConfigureLogging();
        
        builder.Services.AddAes(builder.Configuration);
        
        builder.Services.AddFeedData();
        builder.Services.AddFeedService();
        
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<StringValidationFilter>();
        });
        
        var app = builder.Build();

        app.UseHttpsRedirection();
        app.MapControllers();

        await app.RunAsync();
    }
}