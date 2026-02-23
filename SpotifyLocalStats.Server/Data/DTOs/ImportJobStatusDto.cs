using WebApi.Models.Jobs;

namespace WebApi.Data.DTOs;

public class ImportJobStatusDto
{
    public Guid JobId { get; set; }
    public JobStatus Status { get; set; }
    public int ProgressPercent { get; set; }
    public string ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}
