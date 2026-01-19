using System.Runtime.InteropServices.Marshalling;

namespace SpotifyLocalStats.Server.Models;

public class AggregatedArtist
{
    public Guid Id { get; set; }
    public User User { get; set; }
    public Artist Artist { get; set; }
    public Track TopTrack { get; set; } // list instead?
    public Album TopAlbum { get; set; } // list instead? aggregatedAlnbum instead?
    public int TimesPlayed { get; set; }
    public int UniqueTracksPlayed { get; set; }
    public int MsListened { get; set; }
    public double MinsListened { get; set; } // set from TimeListendMs
    public string TimeOfDayStats { get; set; } // morning, afternoon, evening, night || Need to figure this out, map tod from imported then store somewhere?
    public string TopListeningDate { get; set; } // date when the user listened to this artist the most
    public int MostTracksIn24Hours { get; set; } // could show all tracks that this was? List<Track>.Add(all) 
    public int AlbumsListened { get; set; }
    public string DateTimeFirstListened { get; set; } // date when user first listened to this artist
    public string DateTimeLastListened { get; set; } // date when user last listened to this artist
    public string LongestStreakDays { get; set; } // longest streak of days listened to this artist
    public string CurrentStreakDays { get; set; } // current streak of days listened to this artist
    public DateOnly LongestDrySpellStart { get; set; } // longest dry spell without listening to this artist and the date to and from 
    public DateOnly LongestDrySpellEnd { get; set; }


}
