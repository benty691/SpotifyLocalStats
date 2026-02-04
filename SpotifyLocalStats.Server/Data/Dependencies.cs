using Microsoft.Build.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SpotifyLocalStats.Server.Models;
using WebApi.Services;
using WebApi.Services.Implementations;
using WebApi.Services.Implementations.Helpers;
using WebApi.Services.Interfaces;
using WebApi.Services.Interfaces.Helpers;

namespace SpotifyLocalStats.Server.Data;

public static class Dependencies
{
    public static void ConfigureServices(IConfiguration configuration, IServiceCollection services)
    {
        /*bool useOnlyInMemoryDatabase = false;
        if (configuration["UseOnlyInMemoryDatabase"] != null)
        {
            useOnlyInMemoryDatabase = bool.Parse(configuration["UseOnlyInMemoryDatabase"]!);
        }

        if (useOnlyInMemoryDatabase)
        {
            services.AddDbContext<SpotifyStatsContext>(c =>
               c.UseInMemoryDatabase("SpotifyStats"));
        }*/
            // use SQL server

        services.AddDbContext<SpotifyStatsContext>(c =>
            c.UseSqlServer(configuration.GetConnectionString("SpotifyStatsConnection")));
        services.AddScoped<IImportedTrackService, ImportedTrackService>();
        //services.AddScoped(typeof(BaseService<>));
        services.AddScoped<IImportOrchestrationService, ImportOrchestrationService>();
        services.AddScoped<IArtistAggregationHelpersService, ArtistAggregationHelpersService>();
        services.AddScoped<IAlbumAggregationHelpersService, AlbumAggregationHelperService>();
        services.AddScoped<ITrackAggregationHelpersService, TrackAggregationHelpersService>();
        services.AddScoped<IAggregationService, AggreationService>();
        services.AddScoped<IModelPopulationService, ModelPopulationService>();
    }

    public static User DoesUserExist(IServiceCollection services)
    {
        var user = new User();

        using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var context = serviceProvider.GetRequiredService<SpotifyStatsContext>())
            {
                var userCount = context.Users.Count();
                if (userCount == 0)
                {
                    user.UserName = "DefaultUser";
                    user.HasImportedHistorical = false;
                    context.Users.Add(user);
                    context.SaveChanges();
                }
                else
                {
                    user = context.Users.Single();
                    user.LastTimeUsed = DateTime.UtcNow;
                    context.SaveChanges();
                }
            }
        }
        return user;
    }
}
