namespace SpotifyLocalStats.Server.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public DateTime FirstListen { get; set; } // maybe new name later (how old is there spotify streaming history
        public string[] Platforms { get; set; } // maybe enum later??
        public string[] Countries { get; set; }
        public List<Genre> Top3Genres { get; set; }
        public List<Artist> Top5Artists { get; set; }
        public List<Track> Top5Songs { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime LastUpdatedAt { get; set; }
        public DateTime LastTimeUsed { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Country { get; set; } // either let user fill in or auto detect from ip
        public string Phone { get; set; }
        public bool Auth { get; set; } // is user authenticated with spotify
        public string[] SpotifyPermissions { get; set; } // maybe enum later
        public string SpotifyId { get; set; } // users spotify id
        public string SpotifyUri { get; set; } // users spotify profile url
        public string SpotifyDisplayName { get; set; } // users spotify display name
        public string SpotifyHref { get; set; } // users spotify href
        public List<Image> Images { get; set; }
        public bool IsPremium { get; set; } // is user premium
        public bool HasImportedHistorical { get; set; } // has the user uploaded historical data
        public int TracksListened { get; set; }
        public string TimeOfDayMostActive { get; set; } // morning, afternoon, evening, night || Need to figure this out, map tod from imported then store somewhere?
        public string ShuffleData { get; set; } // based on times on shuffle vs not, in ms and number, eg 100 times on shuffle for 5 hours total vs 50 times not on shuffle for 2 hours total
        public string OnlineData { get; set; } // based on times onlin vs offline, ttime spemnt online vs offlie, etc 
    }
