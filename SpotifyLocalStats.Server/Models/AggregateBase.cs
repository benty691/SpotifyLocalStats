namespace SpotifyLocalStats.Server.Models
{
    public abstract class AggregateBase : BaseModel
    {
        public AggregateBase()
        {
            MinsListened = MsListened / 60000.0;
        }

        public User User { get; set; }
        public Guid userId { get; set; }
        public virtual int PlayCount { get; set; }
        public int MsListened { get; set; }
        public double MinsListened { get; set; }  // set from msListend
        public DateOnly TopListeningDate { get; set; } // date when the user listened to this track the most // maybe datetime??
        public string TimeOfDayStats { get; set; } // morning, afternoon, evening, night || Need to figure this out, map tod from imported then store somewhere?
        public string DateTimeFirstListened { get; set; } // date when user first listened to this artist
        public string DateTimeLastListened { get; set; } // date when user last listened to this artist
        public string LongestStreakDays { get; set; } // longest streak of days listened to this artist
        public string CurrentStreakDays { get; set; } // current streak of days listened to this artist
        public DateOnly LongestDrySpellStart { get; set; } // longest dry spell without listening to this artist and the date to and from 
        public DateOnly LongestDrySpellEnd { get; set; } // hasnt ended yet if current
        public int MostTimesIn24Hours { get =>

            // thinking here is use a formula to detrmine most times listend in 24 hours.. maybe too much logic to be in a model?
                GetMostTimeIn24Hours(){

            }


                }// could show all tracks that this was? List<Track>.Add(all) 
      
    }
}
