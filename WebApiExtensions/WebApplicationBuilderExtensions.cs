using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WebApiExtensions;

public static class WebApplicationBuilderExtensions
{
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        builder.Logging.ClearProviders();
        builder.Logging.AddSerilog(Log.Logger);
        
        builder.Host.UseSerilog((_, config) =>
        {
            config.WriteTo.Console();
            config.WriteTo.File("/Logs/.log");
            config.Enrich.FromLogContext();
        });
    }
}