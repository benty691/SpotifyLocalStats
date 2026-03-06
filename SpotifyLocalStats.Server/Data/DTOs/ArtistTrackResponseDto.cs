namespace WebApi.Data.DTOs
{
    public class ArtistTrackResponseDto
    {
        public required DateTime TimeStamp { get; set; }
        public int? MsPlayed { get; set; }
        public string? MasterMetadataTrackName { get; set; }
        public string? MasterMetadataArtistName { get; set; }
        public string? MasterMetadataAlbumName { get; set; }
    }
}
