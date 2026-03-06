namespace WebApi.Data.DTOs
{
    public class ImportedTrackListDto
    {
        public DateTime TimeStamp { get; set; }
        public string Platform { get; set; }
        public int MsPlayed { get; set; }
        public string ConnCountry { get; set; }
        public string MasterMetadataTrackName { get; set; }
        public string MasterMetadataArtistName { get; set; }
        public string MasterMetadataAlbumName { get; set; }
        public bool IsShuffle { get; set; }
        public bool IsSkipped { get; set; }
        public bool IsOffline { get; set; }
        public DateTime OfflineTimestamp { get; set; }

    }
}
