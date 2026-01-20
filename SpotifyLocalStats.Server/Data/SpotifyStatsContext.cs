using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Models;
using System.Reflection;

namespace SpotifyLocalStats.Server.Data;

public class SpotifyStatsContext : DbContext
{
    public SpotifyStatsContext(DbContextOptions<SpotifyStatsContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<ImportedTrack> ImportedTracks { get; set; }
    public DbSet<Track> Tracks { get; set; }
    public DbSet<Album> Albums { get; set; }
    public DbSet<Artist> Artists { get; set; }
    public DbSet<AggregatedTrack> AggregatedTracks { get; set; }
    public DbSet<AggregatedAlbum> AggregatedAlbums { get; set; }
    public DbSet<AggregatedArtist> AggregatedArtists { get; set; }
    public DbSet<CopyrightContent> CopyrightContents { get; set; }
    public DbSet<Image> Images { get; set; }
    public DbSet<ExternalId> ExternalIds { get; set; }
    public DbSet<Genre> Genres { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}