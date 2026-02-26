using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Models;
using WebApi.Models;
using WebApi.Models.Jobs;

namespace SpotifyLocalStats.Server.Data;

public class SpotifyStatsContext : DbContext
{
    public SpotifyStatsContext(DbContextOptions<SpotifyStatsContext> options) : base(options) { }

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
    public DbSet<ImportJobStatus> ImportJobStatuses { get; set; }
    public DbSet<UploadHistory> UploadHistories { get; set; }

    //public DbSet<CopyrightContent> CopyrightContents { get; set; }
    //public DbSet<Image> Images { get; set; }
    //public DbSet<ExternalId> ExternalIds { get; set; }
    //public DbSet<Genre> Genres { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AggregateBase>()
            .HasOne(a => a.User)  // point to the actual nav property
            .WithMany()
            .HasForeignKey(a => a.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<AggregateBase>().Ignore(x => x.MinsListened);

        builder.Entity<Album>()
            .HasOne(a => a.Artist)
            .WithMany(a => a.Albums)
            .HasForeignKey("ArtistId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Track>()
            .HasOne(t => t.Album)
            .WithMany(a => a.Tracks)
            .HasForeignKey("AlbumId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<Track>()
            .HasOne(t => t.Artist)
            .WithMany()
            .HasForeignKey("ArtistId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<ImportedTrack>()
            .HasOne(t => t.UploadHistory)
            .WithMany()
            .HasForeignKey("UploadHistoryId")
            .OnDelete(DeleteBehavior.NoAction);

        builder.Entity<AggregatedArtist>().ToTable("AggregatedArtists");
        builder.Entity<AggregatedAlbum>().ToTable("AggregatedAlbums");
        builder.Entity<AggregatedTrack>().ToTable("AggregatedTracks");
    }

    public async Task<(List<T>, int)> SaveChangesWithResultAsync<T>() where T : class
    {
        var changedEntities = ChangeTracker.Entries()
            .Where(x => x.State is EntityState.Added or EntityState.Modified)
            .Select(x => x.Entity)
            .OfType<T>()
            .ToList();

        var count = await base.SaveChangesAsync();

        return (changedEntities, count);
    }
}