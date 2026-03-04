namespace WebApi.Data.DTOs;

public class UploadHistoryResponseDto
{
    public string FileName { get; set; }
    public int ImportedTrackCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid Id { get; set; }
}
