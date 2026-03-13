using SpotifyLocalStats.Server.Data;
using WebApi.Data.External;
using WebApi.Models.Jobs;
using WebApi.Services.Interfaces;
using WebApi.Services.Interfaces.External;

namespace WebApi.Services.Implementations.External;

public class SpotifyArtistBackgroundWorker : BackgroundService, ISpotifyBackgroundWorker
{

    private readonly SpotifyArtistQueue _queue;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SpotifyArtistBackgroundWorker> _logger;

    public SpotifyArtistBackgroundWorker(SpotifyArtistQueue queue, IServiceScopeFactory serviceScopeFactory, ILogger<SpotifyArtistBackgroundWorker> logger)
    {
        _queue = queue;
        _scopeFactory = serviceScopeFactory;
        _logger = logger;
    }


    // this wokrer, i want to take in a type (artist, album etc) and then call an endpoint based upon the type. which i dont think geenrics are a good use for, as the enspoints will be different and the resturn will be aswell... 

    // we actually have to work this in an order, so we get the trackid form the imported track, use that to call the endpoint get track, this will return:
    /*
    {
  "album": {
    "album_type": "album",
    "artists": [
      {
        "external_urls": {
          "spotify": "https://open.spotify.com/artist/4Z8W4fKeB5YxbusRsdQVPb"
        },
        "href": "https://api.spotify.com/v1/artists/4Z8W4fKeB5YxbusRsdQVPb",
        "id": "4Z8W4fKeB5YxbusRsdQVPb",
        "name": "Radiohead",
        "type": "artist",
        "uri": "spotify:artist:4Z8W4fKeB5YxbusRsdQVPb"
      }
    ],
    "available_markets": [
      "AR",
      "AU",
      "AT",
      "BE",
      "BO",
      "BR",
      "BG",
      "CA",
      "CL",
      "CO",
      "CR",
      "CY",
      "CZ",
      "DK",
      "DO",
      "DE",
      "EC",
      "EE",
      "SV",
      "FI",
      "FR",
      "GR",
      "GT",
      "HN",
      "HK",
      "HU",
      "IS",
      "IE",
      "IT",
      "LV",
      "LT",
      "LU",
      "MY",
      "MT",
      "MX",
      "NL",
      "NZ",
      "NI",
      "NO",
      "PA",
      "PY",
      "PE",
      "PH",
      "PL",
      "PT",
      "SG",
      "SK",
      "ES",
      "SE",
      "CH",
      "TW",
      "TR",
      "UY",
      "US",
      "GB",
      "AD",
      "LI",
      "MC",
      "ID",
      "JP",
      "TH",
      "VN",
      "RO",
      "IL",
      "ZA",
      "SA",
      "AE",
      "BH",
      "QA",
      "OM",
      "KW",
      "EG",
      "MA",
      "DZ",
      "TN",
      "LB",
      "JO",
      "PS",
      "IN",
      "BY",
      "KZ",
      "MD",
      "UA",
      "AL",
      "BA",
      "HR",
      "ME",
      "MK",
      "RS",
      "SI",
      "KR",
      "BD",
      "PK",
      "LK",
      "GH",
      "KE",
      "NG",
      "TZ",
      "UG",
      "AG",
      "AM",
      "BS",
      "BB",
      "BZ",
      "BT",
      "BW",
      "BF",
      "CV",
      "CW",
      "DM",
      "FJ",
      "GM",
      "GE",
      "GD",
      "GW",
      "GY",
      "HT",
      "JM",
      "KI",
      "LS",
      "LR",
      "MW",
      "MV",
      "ML",
      "MH",
      "FM",
      "NA",
      "NR",
      "NE",
      "PW",
      "PG",
      "PR",
      "WS",
      "SM",
      "ST",
      "SN",
      "SC",
      "SL",
      "SB",
      "KN",
      "LC",
      "VC",
      "SR",
      "TL",
      "TO",
      "TT",
      "TV",
      "VU",
      "AZ",
      "BN",
      "BI",
      "KH",
      "CM",
      "TD",
      "KM",
      "GQ",
      "SZ",
      "GA",
      "GN",
      "KG",
      "LA",
      "MO",
      "MR",
      "MN",
      "NP",
      "RW",
      "TG",
      "UZ",
      "ZW",
      "BJ",
      "MG",
      "MU",
      "MZ",
      "AO",
      "CI",
      "DJ",
      "ZM",
      "CD",
      "CG",
      "IQ",
      "LY",
      "TJ",
      "VE",
      "ET",
      "XK"
    ],
    "external_urls": {
    "spotify": "https://open.spotify.com/album/6GjwtEZcfenmOf6l18N7T7"
    },
    "href": "https://api.spotify.com/v1/albums/6GjwtEZcfenmOf6l18N7T7",
    "id": "6GjwtEZcfenmOf6l18N7T7",
    "images": [
      {
        "url": "https://i.scdn.co/image/ab67616d0000b2736c7112082b63beefffe40151",
        "width": 640,
        "height": 640
      },
      {
    "url": "https://i.scdn.co/image/ab67616d00001e026c7112082b63beefffe40151",
        "width": 300,
        "height": 300
      },
      {
    "url": "https://i.scdn.co/image/ab67616d000048516c7112082b63beefffe40151",
        "width": 64,
        "height": 64
      }
    ],
    "name": "Kid A",
    "release_date": "2000-10-02",
    "release_date_precision": "day",
    "total_tracks": 11,
    "type": "album",
    "uri": "spotify:album:6GjwtEZcfenmOf6l18N7T7"
  },
  "artists": [
    {
      "external_urls": {
        "spotify": "https://open.spotify.com/artist/4Z8W4fKeB5YxbusRsdQVPb"
      },
      "href": "https://api.spotify.com/v1/artists/4Z8W4fKeB5YxbusRsdQVPb",
      "id": "4Z8W4fKeB5YxbusRsdQVPb",
      "name": "Radiohead",
      "type": "artist",
      "uri": "spotify:artist:4Z8W4fKeB5YxbusRsdQVPb"
    }
  ],
  "available_markets": [
    "AR",
    "AU",
    "AT",
    "BE",
    "BO",
    "BR",
    "BG",
    "CA",
    "CL",
    "CO",
    "CR",
    "CY",
    "CZ",
    "DK",
    "DO",
    "DE",
    "EC",
    "EE",
    "SV",
    "FI",
    "FR",
    "GR",
    "GT",
    "HN",
    "HK",
    "HU",
    "IS",
    "IE",
    "IT",
    "LV",
    "LT",
    "LU",
    "MY",
    "MT",
    "MX",
    "NL",
    "NZ",
    "NI",
    "NO",
    "PA",
    "PY",
    "PE",
    "PH",
    "PL",
    "PT",
    "SG",
    "SK",
    "ES",
    "SE",
    "CH",
    "TW",
    "TR",
    "UY",
    "US",
    "GB",
    "AD",
    "LI",
    "MC",
    "ID",
    "JP",
    "TH",
    "VN",
    "RO",
    "IL",
    "ZA",
    "SA",
    "AE",
    "BH",
    "QA",
    "OM",
    "KW",
    "EG",
    "MA",
    "DZ",
    "TN",
    "LB",
    "JO",
    "PS",
    "IN",
    "BY",
    "KZ",
    "MD",
    "UA",
    "AL",
    "BA",
    "HR",
    "ME",
    "MK",
    "RS",
    "SI",
    "KR",
    "BD",
    "PK",
    "LK",
    "GH",
    "KE",
    "NG",
    "TZ",
    "UG",
    "AG",
    "AM",
    "BS",
    "BB",
    "BZ",
    "BT",
    "BW",
    "BF",
    "CV",
    "CW",
    "DM",
    "FJ",
    "GM",
    "GE",
    "GD",
    "GW",
    "GY",
    "HT",
    "JM",
    "KI",
    "LS",
    "LR",
    "MW",
    "MV",
    "ML",
    "MH",
    "FM",
    "NA",
    "NR",
    "NE",
    "PW",
    "PG",
    "PR",
    "WS",
    "SM",
    "ST",
    "SN",
    "SC",
    "SL",
    "SB",
    "KN",
    "LC",
    "VC",
    "SR",
    "TL",
    "TO",
    "TT",
    "TV",
    "VU",
    "AZ",
    "BN",
    "BI",
    "KH",
    "CM",
    "TD",
    "KM",
    "GQ",
    "SZ",
    "GA",
    "GN",
    "KG",
    "LA",
    "MO",
    "MR",
    "MN",
    "NP",
    "RW",
    "TG",
    "UZ",
    "ZW",
    "BJ",
    "MG",
    "MU",
    "MZ",
    "AO",
    "CI",
    "DJ",
    "ZM",
    "CD",
    "CG",
    "IQ",
    "LY",
    "TJ",
    "VE",
    "ET",
    "XK"
  ],
  "disc_number": 1,
  "duration_ms": 351693,
  "explicit": false,
  "external_ids": {
    "isrc": "GBAYE0000812"
  },
  "external_urls": {
    "spotify": "https://open.spotify.com/track/4Wgj6jzoI2gYlumXdYAB8U"
  },
  "href": "https://api.spotify.com/v1/tracks/4Wgj6jzoI2gYlumXdYAB8U",
  "id": "4Wgj6jzoI2gYlumXdYAB8U",
  "is_local": false,
  "name": "The National Anthem",
  "popularity": 59,
  "preview_url": null,
  "track_number": 3,
  "type": "track",
  "uri": "spotify:track:4Wgj6jzoI2gYlumXdYAB8U"
}
    */

    //this in turn will retunr the album and artist, in which we will query thsi seperately, after we have returned them. 
    // we can then populate the artist values, track values and album values. 
    // we will have to ensure these are called in order
    // flow ill be:
    // call track endpoint -> get artist and album endpoint -> call those two seperately via seperate queues
    // in each queue, we then lookup the respective table (first looked up and create a dict)
    // find the item we have associated with the calls, and populate. 
    // We then need to ensure we are not duplicating wokr here, so once we get a track and query, we add to a shared state dictionary so we then know not to look up again. we dont call this worker from anywherre but isndie the generate track method. we probs return requeue the track object for the orchestrator to ingest so we know which tracks we do not need to lookup again. 
    // importnatly, this needs to be a global state and worker, such that if two users were to impor tracks simultaneouls the wokrer would work on a seprate thread an read from a queue, and remove dupliactes etc idk 




    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await foreach (var data in _queue.ReadAllAsync(stoppingToken))
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SpotifyStatsContext>();
            var importService = scope.ServiceProvider.GetRequiredService<IImportOrchestrationService>();




            var importJob = await context.ImportJobStatuses.FindAsync(data.JobId);
            importJob.Status = JobStatus.Processing;
            await context.SaveChangesAsync();

            try
            {
                await importService.ProcessAsync(data.Json, data.File, data.User, data.JobId, stoppingToken);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Import Job failed for id: {JobId}", data.JobId);

                await context.Entry(importJob).ReloadAsync();
                importJob.Status = JobStatus.Failed;
                importJob.ErrorMessage = ex.Message;
                await context.SaveChangesAsync();
            }
        }
    }
}

