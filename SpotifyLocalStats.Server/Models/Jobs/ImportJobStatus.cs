namespace WebApi.Models.Jobs;

public class ImportJobStatus
{
    public ImportJobStatus()
    {
        Id = Guid.NewGuid();
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; set; }
    public JobStatus Status{ get; set;}
    public int ProgressPercent { get; set;}
    public string? ErrorMessage { get; set;}
    public DateTime CreatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public enum JobStatus
{
    Queued,
    Processing,
    Completed,
    Failed,
    Duplicate
}
