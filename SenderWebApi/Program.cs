using SenderWebApi.Extensions;

namespace SenderWebApi;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.ConfigureLogging();
        
        builder.Services.AddPortManager(builder.Configuration);
        builder.Services.AddAes(builder.Configuration);
        
        builder.Services.AddSender();

        builder.Services.AddControllers();
        
        var app = builder.Build();

        app.UseHttpsRedirection();
        
        app.MapControllers();
        
        await app.RunAsync();
    }
}