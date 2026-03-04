namespace SpotifyLocalStats.Server.Models
{
    public abstract class AggregateBase : BaseModel
    {
        public AggregateBase()
        {
        }

        public User User { get; set; } = null!;
        public Guid UserId { get; set; }
        public int PlayCount { get; set; }
        public long? MsListened { get; set; }
        public double MinsListened => MsListened.HasValue ? MsListened.Value / 60000.0 : 0;
        public DateTime TopListeningDate { get; set; } // date when the user listened to this track the most // maybe datetime??
        public DateTime DateTimeFirstListened { get; set; } // date when user first listened to this artist
        public DateTime DateTimeLastListened { get; set; } // date when user last listened to this artist
        public int LongestStreakDays { get; set; } // longest streak of days listened to this artist
        public DateTime LongestStreakStartDate { get; set; }
        public DateTime LongestStreakEndDate { get; set; }
        public string? CurrentStreakDays { get; set; } // current streak of days listened to this artist
        public DateTime LongestDrySpellStart { get; set; } // longest dry spell without listening to this artist and the date to and from 
        public DateTime LongestDrySpellEnd { get; set; } // hasnt ended yet if current
        public int LongestDrySpell { get; set; }
        public int MostTimesIn24Hours { get; set; }
    }
}
