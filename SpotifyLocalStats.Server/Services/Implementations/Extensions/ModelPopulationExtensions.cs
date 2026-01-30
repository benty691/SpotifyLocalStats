using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Models;

namespace WebApi.Services.Implementations.Extensions
{
    public class ModelPopulationExtensions
    {
        private async Task<IEnumerable<T>> Generate(IEnumerable<ImportedTrack> tracks, T ) where T : class
        {
            var nullArtistCount = 0;

            // logic to generate artist model from imported track
            foreach (var track in tracks)
            {
                // for each track, we ideally try create an artist, if that artist already exists, we skip
                if (track.MasterMetadataArtistName != null)
                {
                    // something to note is in json data we do not get spotify artsist url. My thinking is here we should query the webapi and try get it, so we can be definitive in artits, because artist names overlaps, i am sure. 
                    if (_context.Artists.Select(x => x.Name == track.MasterMetadataArtistName).Single())
                    {
                        continue;
                    }
                    else
                    {
                        // we need spotify webapi to allow this to occur properly, as we neeed to hit the endpoint to get details, but we need the artist id from spotify to query??? 
                        // appears we can use the search endpoint and search artist nam, and then get aristid from that, then query artist endpoint for details

                        await _context.Artists.AddAsync(new Artist
                        {
                            Name = track.MasterMetadataArtistName
                        });
                    }
                }
                else
                {
                    nullArtistCount++;
                    _logger.LogWarning("Track with ID {TrackId} does not have an artist name.", track.Id);
                    continue;
                }
            }
            var result = _context.SaveChanges();
            _logger.LogInformation($"Generated {result} new artists from imported tracks.\n {nullArtistCount} tracks with null artist.");
        }
    }
}
