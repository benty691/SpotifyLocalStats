namespace WebApi.Data.DTOs;

public class AggregateArtistDto
{
    public string Name { get; set; }
    public int PlayCount { get; set; }
    public double MinsListened { get; set; }
    public DateTime TopListeningDate { get; set; }
    public DateTime FirstListened { get; set; }
    public DateTime LastListened { get; set; }
    public int LongestStreak { get; set; }
    public DateTime LongestStreakStart { get; set; }
    public DateTime LongestStreakEnd { get; set; }
    public int LongestDrySpell { get; set; }
    public DateTime LongestDrySpellStart { get; set; }
    public DateTime LongestDrySpellEnd { get; set; }
    public int MostTimesIn24Hours { get; set; }
}
