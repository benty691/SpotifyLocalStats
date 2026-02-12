using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Models;
using System.Reflection;
using System.Reflection.Emit;
using WebApi.Models;

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
    public DbSet<TimeOfDayStat<AggregatedArtist>> ArtistTimeOfDaysStats { get; set; }
    public DbSet<TimeOfDayStat<AggregatedTrack>> TrackTimeOfDaysStats { get; set; }
    public DbSet<TimeOfDayStat<AggregatedAlbum>> AlbumTimeOfDaysStats { get; set; }
    //public DbSet<CopyrightContent> CopyrightContents { get; set; }
    //public DbSet<Image> Images { get; set; }
    //public DbSet<ExternalId> ExternalIds { get; set; }
    //public DbSet<Genre> Genres { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        foreach (var entity in builder.Model.GetEntityTypes()
         .Where(e => typeof(AggregateBase).IsAssignableFrom(e.ClrType)))
        {
            builder.Entity(entity.ClrType)
                .HasOne("User")
                .WithMany()
                .HasForeignKey(nameof(AggregateBase.UserId))
                .OnDelete(DeleteBehavior.NoAction);
        }

        builder.Entity<AggregateBase>().Ignore(x => x.MinsListened);
    }
}