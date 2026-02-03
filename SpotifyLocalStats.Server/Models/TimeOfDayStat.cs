using SpotifyLocalStats.Server.Models;

namespace WebApi.Models
{
    public class TimeOfDayStat<T> where T : class
    {
        public TimeOfDayStat() 
        {
            Id = new Guid();
            //CreatedAt = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        public Guid Id { get; set; }
        public T Aggregate { get; set; }
        public Guid AggregateId { get; set; }
        public int TimeOfDay { get; set; }
        public int PlayCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
