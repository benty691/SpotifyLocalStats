using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Models;
using WebApi.Data.DTOs;
using WebApi.Data.DTOs.NewFolder;
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
        services.AddScoped<IUserAggregateService<AggregateArtistDto>>(sp =>
            new UserAggregateService<AggregatedArtist, AggregateArtistDto>(
                sp.GetRequiredService<ILogger<UserAggregateService<AggregatedArtist, AggregateArtistDto>>>(), //
                sp.GetRequiredService<SpotifyStatsContext>(), //
                q => q.Include(x => x.Artist).Include(x => x.TimeOfDayStats), //inludeBuilder
                artist => new AggregateArtistDto // mapper
                {
                    Name = artist.Artist.Name,
                    PlayCount = artist.PlayCount,
                    MinsListened = artist.MinsListened,
                    TopListeningDate = artist.TopListeningDate,
                    FirstListened = artist.DateTimeFirstListened,
                    LastListened = artist.DateTimeLastListened,
                    LongestStreak = artist.LongestStreakDays,
                    LongestStreakStart = artist.LongestStreakStartDate,
                    LongestStreakEnd = artist.LongestStreakEndDate,
                    LongestDrySpell = artist.LongestDrySpell,
                    LongestDrySpellStart = artist.LongestDrySpellStart,
                    LongestDrySpellEnd = artist.LongestDrySpellEnd,
                    MostTimesIn24Hours = artist.MostTimesIn24Hours,
                    TimeOfDayStats = artist.TimeOfDayStats
                        .Select(t => new TimeOfDayStatDto<AggregatedArtist>
                        {
                            AggregateId = artist.Id,
                            TimeOfDay = t.TimeOfDay,
                            PlayCount = t.PlayCount,
                            LastUpdatedAt = t.LastUpdatedAt
                        }).ToList()
                }
            ));
        services.AddScoped<IUserAggregateService<AggregateAlbumDto>>(sp =>
            new UserAggregateService<AggregatedAlbum, AggregateAlbumDto>(
                sp.GetRequiredService<ILogger<UserAggregateService<AggregatedAlbum, AggregateAlbumDto>>>(), //
                sp.GetRequiredService<SpotifyStatsContext>(), //
                q => q.Include(x => x.Album).Include(x => x.TimeOfDayStats), //inludeBuilder
                album => new AggregateAlbumDto // mapper
                {
                    Name = album.Album.Name,
                    PlayCount = album.PlayCount,
                    MinsListened = album.MinsListened,
                    TopListeningDate = album.TopListeningDate,
                    FirstListened = album.DateTimeFirstListened,
                    LastListened = album.DateTimeLastListened,
                    LongestStreak = album.LongestStreakDays,
                    LongestStreakStart = album.LongestStreakStartDate,
                    LongestStreakEnd = album.LongestStreakEndDate,
                    LongestDrySpell = album.LongestDrySpell,
                    LongestDrySpellStart = album.LongestDrySpellStart,
                    LongestDrySpellEnd = album.LongestDrySpellEnd,
                    MostTimesIn24Hours = album.MostTimesIn24Hours,
                    TimeOfDayStats = album.TimeOfDayStats
                        .Select(t => new TimeOfDayStatDto<AggregatedAlbum>
                        {
                            AggregateId = album.Id,
                            TimeOfDay = t.TimeOfDay,
                            PlayCount = t.PlayCount,
                            LastUpdatedAt = t.LastUpdatedAt
                        }).ToList()
                }
            ));
        services.AddScoped<IUserAggregateService<AggregateTrackDto>>(sp =>
            new UserAggregateService<AggregatedTrack, AggregateTrackDto>(
                sp.GetRequiredService<ILogger<UserAggregateService<AggregatedTrack, AggregateTrackDto>>>(), //
                sp.GetRequiredService<SpotifyStatsContext>(), //
                q => q.Include(x => x.Track).Include(x => x.TimeOfDayStats), //inludeBuilder
                track => new AggregateTrackDto // mapper
                {
                    Name = track.Track.Name,
                    PlayCount = track.PlayCount,
                    MinsListened = track.MinsListened,
                    TopListeningDate = track.TopListeningDate,
                    FirstListened = track.DateTimeFirstListened,
                    LastListened = track.DateTimeLastListened,
                    LongestStreak = track.LongestStreakDays,
                    LongestStreakStart = track.LongestStreakStartDate,
                    LongestStreakEnd = track.LongestStreakEndDate,
                    LongestDrySpell = track.LongestDrySpell,
                    LongestDrySpellStart = track.LongestDrySpellStart,
                    LongestDrySpellEnd = track.LongestDrySpellEnd,
                    MostTimesIn24Hours = track.MostTimesIn24Hours,
                    TimeOfDayStats = track.TimeOfDayStats
                        .Select(t => new TimeOfDayStatDto<AggregatedTrack>
                        {
                            AggregateId = track.Id,
                            TimeOfDay = t.TimeOfDay,
                            PlayCount = t.PlayCount,
                            LastUpdatedAt = t.LastUpdatedAt
                        }).ToList()
                }
            ));
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
                    new ArtistAggregationHelperService(
                        sp.GetRequiredService<ILogger<ArtistAggregationHelperService>>(),
                        sp.GetRequiredService<SpotifyStatsContext>(),
                        artist => artist.MasterMetadataArtistName!,
                        artist => artist.Artist.Name,
                        tod => tod.TimeOfDay,
                        (id, hour) => new ArtistTimeOfDayStat(id, hour, 1)
                    ));
        services.AddScoped<IAggregationHelpersService<AggregatedTrack, TrackTimeOfDayStat>>(sp =>
                    new AggregationHelperService<AggregatedTrack, TrackTimeOfDayStat>(
                        sp.GetRequiredService<ILogger<AggregationHelperService<AggregatedTrack, TrackTimeOfDayStat>>>(),
                        sp.GetRequiredService<SpotifyStatsContext>(),
                        track => track.MasterMetadataTrackName!,
                        track => track.Track.Name,
                        tod => tod.TimeOfDay,
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
