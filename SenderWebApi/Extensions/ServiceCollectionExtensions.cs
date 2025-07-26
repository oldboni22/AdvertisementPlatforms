using AesService.Abstractions;
using PortManager.Abstractions;
using Sender.Services;
using Sender.Services.Abstractions;

namespace SenderWebApi.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddSender(this IServiceCollection collection)
    {
        collection.AddSingleton<ISenderService, SenderService>();
    }

    public static void AddPortManager(this IServiceCollection collection, IConfiguration configuration)
    {
        
        var route = configuration.GetSection("Port").GetSection("route").Value!;
        var address = configuration.GetSection("Port").GetSection("address").Value!;
        
        if (string.IsNullOrEmpty(route) || string.IsNullOrEmpty(address))
            throw new NullReferenceException();
        
        collection.AddSingleton<IPortManager>(new PortManager.PortManager(address, route));
    }

    public static void AddAes(this IServiceCollection collection, IConfiguration configuration)
    {
        var key = configuration.GetSection("Aes").Value;

        if(string.IsNullOrEmpty(key))
            throw new NullReferenceException();

        collection.AddSingleton<IAesService>(new AesService.AesService(key));
    }
}