namespace WebApi.Data.DTOs;

public class ArtistTimeframeResponseDto
{
    public List<DateTime> TimeStamp { get; set; }
    public int PlayCount { get; set; }
    public DateTime RangeStart { get; set; }
    public DateTime RangeEnd { get; set; }
}
