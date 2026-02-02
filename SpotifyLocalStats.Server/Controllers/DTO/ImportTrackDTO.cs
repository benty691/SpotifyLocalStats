namespace WebApi.Controllers.DTO
{
    public class ImportTracksDTO
    {
        public ImportTracksDTO() 
        {
            ImportedAt = DateTime.UtcNow; 
        }    

        public int ArtistCount { get; set; }
        public int AlbumCount { get; set; }
        public int TrackCount { get; set; }
        public DateTime ImportedAt { get; set; }
    }
}
