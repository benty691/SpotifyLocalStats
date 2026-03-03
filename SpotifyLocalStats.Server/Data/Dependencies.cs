using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Models;
using WebApi.Data.Jobs;
using WebApi.Models.TimeOfDayConcretes;
using WebApi.Services.Implementations;
using WebApi.Services.Implementations.Helpers;
using WebApi.Services.Interfaces;
using WebApi.Services.Interfaces.Helpers;
using WebApi.Services.Workers;

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
        services.AddScoped<IAggregationService, AggreationService>();
        services.AddScoped<IImportOrchestrationService, ImportOrchestrationService>();
        services.AddScoped<IModelPopulationService, ModelPopulationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IUserBasicStatsService, UserBasicStatsService>();
        services.AddScoped<IUserAggregateArtistService, UserAggregateArtistService>();
        services.AddScoped<IUploadHistoryService, UploadHistoryService>();
        services.AddScoped<IAggregationHelpersService<AggregatedAlbum, AlbumTimeOfDayStat>>(sp =>
                    new AggregationHelperService<AggregatedAlbum, AlbumTimeOfDayStat>(
                        sp.GetRequiredService<ILogger<AggregationHelperService<AggregatedAlbum, AlbumTimeOfDayStat>>>(),
                        sp.GetRequiredService<SpotifyStatsContext>(),
                        album => album.MasterMetadataAlbumName!,
                        album => album.Album.Name,
                        tod => tod.TimeOfDay,
                        (id, hour) => new AlbumTimeOfDayStat(id, hour, 1)
                    ));
        services.AddScoped<IAggregationHelpersService<AggregatedArtist, ArtistTimeOfDayStat>>(sp =>
                    new AggregationHelperService<AggregatedArtist, ArtistTimeOfDayStat>(
                        sp.GetRequiredService<ILogger<AggregationHelperService<AggregatedArtist, ArtistTimeOfDayStat>>>(),
                        sp.GetRequiredService<SpotifyStatsContext>(),
                        artist => artist.MasterMetadataArtistName!, //group
                        artist => artist.Artist.Name, // name
                        tod => tod.TimeOfDay, // tod
                        (id, hour) => new ArtistTimeOfDayStat(id, hour, 1)
                    ));
        services.AddScoped<IAggregationHelpersService<AggregatedTrack, TrackTimeOfDayStat>>(sp =>
                    new AggregationHelperService<AggregatedTrack, TrackTimeOfDayStat>(
                        sp.GetRequiredService<ILogger<AggregationHelperService<AggregatedTrack, TrackTimeOfDayStat>>>(),
                        sp.GetRequiredService<SpotifyStatsContext>(),
                        track => track.MasterMetadataTrackName!, //group
                        track => track.Track.Name, // name
                        tod => tod.TimeOfDay, // tod
                        (id, hour) => new TrackTimeOfDayStat(id, hour, 1)
                    ));
        services.AddSingleton<ImportJobQueue>();
        services.AddHostedService<ImportBackgroundWorker>();

    }

    public static User DoesUserExist(IServiceCollection services)
    {
        var user = new User("DefaultUser", "TestUser");

        using (var serviceProvider = services.BuildServiceProvider())
        {
            using (var context = serviceProvider.GetRequiredService<SpotifyStatsContext>())
            {
                var userCount = context.Users.Count();
                if (userCount == 0)
                {
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
