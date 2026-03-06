namespace WebApi.Data.DTOs
{
    public class ArtistTimeframeRangeDto
    {
        public Guid userId { get; set; }
        public Guid aggArtistId { get; set; }
        public DateTime rangeStart { get; set; }
        public DateTime rangeEnd { get; set; }
    }
}
