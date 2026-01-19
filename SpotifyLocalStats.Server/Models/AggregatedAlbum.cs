namespace SpotifyLocalStats.Server.Models;

public class AggregatedAlbum
{
    public Guid Id { get; set; }
    public Album Album { get; set; }
    public User User { get; set; }
    public int TimesPlayed { get; set; }
    public int TimesCompleted { get; set; } // ehhh really hard to figure out? have to model album, figure out how many times played through, no shufffle, no skipping,???
    public string TopListeningDate { get; set; } // date when the user listened to this album the most
    public string TimeOfDayStats { get; set; } // morning, afternoon, evening, night || Need to figure this out, map tod from imported then store somewhere?
    public int MsListened { get; set; }
    public double MinsListened { get; set; }
    public string DateTimeFirstListened { get; set; } // date when user first listened to this artist
    public string DateTimeLastListened { get; set; } // date when user last listened to this artist
    public string LongestStreakDays { get; set; } // longest streak of days listened to this artist
    public string CurrentStreakDays { get; set; } // current streak of days listened to this artist
    public DateOnly LongestDrySpellStart { get; set; } // longest dry spell without listening to this artist and the date to and from 
    public DateOnly LongestDrySpellEnd { get; set; }


}
