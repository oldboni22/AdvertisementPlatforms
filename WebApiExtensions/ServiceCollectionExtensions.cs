using AesService.Abstractions;
using Feed.Services;
using Feed.Services.Abstractions;
using FeedData.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PortManager.Abstractions;
using Sender.Services;
using Sender.Services.Abstractions;

namespace WebApiExtensions;

public static class ServiceCollectionExtensions
{
    public static void AddSenderService(this IServiceCollection collection)
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

    public static void AddFeedService(this IServiceCollection collection)
    {
        collection.AddSingleton<IFeedService,FeedService>();
    }

    public static void AddFeedData(this IServiceCollection collection)
    {
        collection.AddSingleton<IFeedData, FeedData.FeedData>();
    }
    
}