namespace WebApi.Data.DTOs
{
    public class ImportTrackResponseDto
    {
        public ImportTrackResponseDto()
        {
            ImportedAt = DateTime.UtcNow;
        }

        public int ArtistCount { get; set; }
        public int AlbumCount { get; set; }
        public int TrackCount { get; set; }
        public DateTime ImportedAt { get; set; }
    }
}
