using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Api;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Services.Interfaces.External;

namespace WebApi.Services.Implementations.External;

public class SpotifyCallerService : ISpotifyCallerService
{
    private readonly SpotifyApiClient _spotifyApiClient;
    private readonly SpotifyStatsContext _spotifyStatsContext;

    public SpotifyCallerService(SpotifyApiClient spotifyApiClient, SpotifyStatsContext spotifyStatsContext)
    {
        _spotifyApiClient = spotifyApiClient;
        _spotifyStatsContext = spotifyStatsContext;
    }

    public async Task ProcessAsync(string spotifyTrackId, CancellationToken cancellationToken)
    {
        var transaction = await _spotifyStatsContext.Database.BeginTransactionAsync();

        // create a dictionary for eveuyr track, album artist, pulling in history. then on update of our exisiting artist, we need to ensure that we ne do not update that artist again. no redundant work. ie wse should not be calling getalbum for an albuim in which we already have the details of, same as track, and artist. 
        // we also need to hanel rate limits here. 

        var spotifyTrack = await GetSpotifyTrack(spotifyTrackId, cancellationToken);
        var spotifyAlbum = await GetSpotifyAlbum(spotifyTrack.Album.Id, cancellationToken);
        foreach (var artist in spotifyTrack.Artists)
        {
            var spotifyArtist = await GetSpotifyArtist(artist.Id, cancellationToken);
        }

    }

    public async Task<TrackObject> GetSpotifyTrack(string spotifyTrackId, CancellationToken cancellationToken)
    {
        var spotifyTrack = await _spotifyApiClient.GetTrackAsync(spotifyTrackId.Split(":")[2], "AU", cancellationToken); // everthing after the last :

        // we now have the tarck information, we need to call another service to handle upsert of the db for the given track 
        var track = await _spotifyStatsContext.Tracks.Where(x => x.SpotifyTrackId == spotifyTrackId).SingleAsync();
        track.DiscNumber = spotifyTrack.Disc_number;
        track.TrackNumber = spotifyTrack.Track_number;
        //track.ReleaseDate
        //track.Images = spotifyTrack.
        track.IsExplicit = spotifyTrack.Explicit;
        track.ExternalIds.Upc = spotifyTrack.External_ids.Upc;
        track.ExternalIds.Isrc = spotifyTrack.External_ids.Isrc;
        track.ExternalIds.Ean = spotifyTrack.External_ids.Ean;
        track.SpotifyTrackId = spotifyTrackId;

        // then get the artist url and query that endpoint, and album...
        return spotifyTrack;
    }

    public async Task<ArtistObject> GetSpotifyArtist(string spotifyArtistId, CancellationToken cancellationToken)
    {
        var spotifyArtist = await _spotifyApiClient.GetAnArtistAsync(spotifyArtistId);

        var artist = await _spotifyStatsContext.Artists.Where(x => x.Name == spotifyArtist.Name).SingleAsync();

        var images = new List<Image>();

        foreach (var item in spotifyArtist.Images)
        {
            images.Add(new Image()
            {
                Height = item.Height,
                Width = item.Width,
                Url = item.Url,
            });
        }

        artist.Href = spotifyArtist.Href;
        artist.SpotifyId = spotifyArtistId;
        artist.Images = images;
        artist.Name = spotifyArtist.Name;

        return artist;

    }

    public async Task<AlbumObject> GetSpotifyAlbum(string spotifyAlbumId, CancellationToken cancellationToken)
    {
        var spotifyAlbum = await _spotifyApiClient.GetAnAlbumAsync(spotifyAlbumId, "AU");

        var album = await _spotifyStatsContext.Albums.Where(x => x.Name == spotifyAlbum.Name).SingleAsync();

        var images = new List<Image>();

        foreach (var item in spotifyAlbum.Images)
        {
            images.Add(new Image()
            {
                Height = item.Height,
                Width = item.Width,
                Url = item.Url,
            });
        }

        album.Href = spotifyAlbum.Href;
        album.SpotifyId = spotifyAlbumId;
        album.TotalTracks = spotifyAlbum.Total_tracks;
        // figure out a way to assign extra artist here. We just need to add the artists we have, ie we just get the artistid and assign, this logic will update that artist we added on its own. 
        album.Artist = spotifyAlbum.Artists.FirstOrDefault();
        album.Images = images;
        album.Name = spotifyAlbum.Name;
        album.RealeaseDate = spotifyAlbum.Release_date;
        album.ReleaseDatePrecision = spotifyAlbum.Release_date_precision;
        album.Tracks = spotifyAlbum.Tracks.Items;


        return artist;
    }


}
