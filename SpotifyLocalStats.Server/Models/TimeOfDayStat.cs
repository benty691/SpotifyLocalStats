using SpotifyLocalStats.Server.Models;
using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class TimeOfDayStat<T> where T : class
    {
        public TimeOfDayStat(Guid aggregateId, int timeOfDay, int playCount) 
        {
            Id = new Guid();
            AggregateId = aggregateId;
            TimeOfDay = timeOfDay;
            PlayCount = playCount;
            CreatedAt = DateTime.UtcNow;
            LastUpdatedAt = DateTime.UtcNow;

            //CreatedAt = dateTime ?? throw new ArgumentNullException(nameof(dateTime));
        }

        public Guid Id { get; private set; }
        public T Aggregate { get; set; } = null!;
        public Guid AggregateId { get; private set; }
        public int TimeOfDay { get; set; }
        public int PlayCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
