namespace SpotifyLocalStats.Server.Models;

public class AggregatedTrack
{
    public Guid Id { get; set; }
    public User User { get; }
    public Track Track { get; }
    public Artist Artist { get; }
    public Album Album { get; }
    public int PlayCount { get; set; }
    public int MsListend { get; set; }
    public double MinsListened { get; set; } // set from msListend
    public string TopListeningDate { get; set; } // date when the user listened to this track the most // maybe datetime??
    public string TimeOfDayStats { get; set; } // morning, afternoon, evening, night || Need to figure this out, map tod from imported then store somewhere?
    public string DateTimeFirstListened { get; set; } // date when user first listened to this artist
    public string DateTimeLastListened { get; set; } // date when user last listened to this artist
    public string LongestStreakDays { get; set; } // longest streak of days listened to this artist
    public string CurrentStreakDays { get; set; } // current streak of days listened to this artist
    public DateOnly LongestDrySpellStart { get; set; } // longest dry spell without listening to this artist and the date to and from 
    public DateOnly LongestDrySpellEnd { get; set; } // hasnt ended yet if current

}
