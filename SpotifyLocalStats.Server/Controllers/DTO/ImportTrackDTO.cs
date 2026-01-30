namespace WebApi.Controllers.DTO
{
    public class ImportTracksDTO
    {
        public int Count { get; set; }
        public DateTime ImportedAt { get; set; } = DateTime.Now;
    }
}
