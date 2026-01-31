using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;

namespace WebApi.Services.Implementations.Helpers
{
    public class TrackAggregationHelpersService
    {

        private readonly ILogger<TrackAggregationHelpersService> _logger;
        private readonly SpotifyStatsContext _context;
        private readonly List<AggregatedTrack> _aggregatedTracks;

        // really thinking this can be a helper class, where we pass in the aggergate we want to calc for, instead of having three separte aggreggate helpers that do pretty muhc smae thing? 
        public TrackAggregationHelpersService(ILogger<TrackAggregationHelpersService> logger, SpotifyStatsContext context, List<AggregatedTrack> aggregatedTracks)
        {
            _logger = logger;
            _context = context;
            _aggregatedTracks = _context.AggregatedTracks.ToList(); // is this bad practice?, will this even compile?
        }

        public async Task RunCalculations()
        {
            await CalculateDrySpell();
            await CalculateLongestStreak();
            await CalculateMostTimesIn24Hours();
            await CalculateTopListeningDate();
        }

        public async Task CalculateTopListeningDate()
        {
            //var _aggregatedTracks = _context.AggregatedArtists.ToList();

            if (_aggregatedTracks.Count == 0)
            {
                throw new InvalidOperationException("No aggregated artists found.");
            }

            else
            {
                foreach (var aggTrack in _aggregatedTracks)
                {
                    var importedTracks = _context.ImportedTracks.Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.User.Id == aggTrack.User.Id);

                    var topDay = importedTracks.GroupBy(x => x.TimeStamp.Date).Select(g => new
                    {
                        Date = g.Key,
                        PlayCount = g.Count()
                    })
                         .OrderByDescending(g => g.PlayCount)
                         .FirstOrDefault();

                    aggTrack.TopListeningDate = topDay.Date;
                }
            }
        }

        public async Task CalculateMostTimesIn24Hours()
        {
            // have to determine if I want set 24 hours at 0000-2400 or rolling 24 hours (leaning rolling)
            var playsIn24Hours = 0;

            //int timesListend24Hours = 0;

            if (_aggregatedTracks.Count == 0)
            {
                throw new InvalidOperationException("No aggregated artists found.");
            }

            foreach (var aggTrack in _aggregatedTracks)
            {
                // need to get the max number of plays in any 24 hour period for this artist
                // set time frame from track time, then search 24 hours back, count numbver of times artist appears

                var tracksOfTracks = _context.ImportedTracks
                     .Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.User.Id == aggTrack.User.Id).ToList();

                foreach (var track in tracksOfTracks)
                {
                    var trackTime = track.TimeStamp;
                    var startTime = trackTime.AddHours(-24);
                    var nextPlaysIn24Hours = tracksOfTracks
                        .Where(x => x.TimeStamp >= startTime && x.TimeStamp < trackTime)
                        .Count();

                    if (nextPlaysIn24Hours > playsIn24Hours)
                    {
                        playsIn24Hours = nextPlaysIn24Hours;
                    }
                }
                aggTrack.MostTimesIn24Hours = playsIn24Hours;
            }
        }

        public async Task TimeOfDayStats()
        {
            // goal here is to get all tracks for artist then determine time of day stats by segmenting into morning, afternoon, evening, night?? or just hourly? BY the min? 
            // really need to determine how to split. 
            // also need to figure out whjat to return, maybe a dict for time of day and count?

            Dictionary<TimeSpan, int> timeOfDayCount;

            if (_aggregatedTracks.Count == 0)
            {
                throw new InvalidOperationException("No aggregated artists found.");
            }

            // loop through each artist, get artist tracks from imported tracks 
            foreach (var aggTrack in _aggregatedTracks)
            {
                var tracks = _context.ImportedTracks
                    .Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.User.Id == aggTrack.User.Id)
                    .ToList();

                // not sure if this will workl, as we need to span from 0000-1000 etc etc, if in this range, increment count

                timeOfDayCount = new Dictionary<TimeSpan, int> // thinking we use enum maybe iher instead of string?
            {
                { new TimeSpan(0, 0, 0) , 0}, // midnight
                { new TimeSpan(1,0,0), 0}, // 6am
                { new TimeSpan(2,0,0), 0}, // noon
                { new TimeSpan(3,0,0), 0}, // 6pm
                { new TimeSpan(4,0,0), 0}, // 11:59pm
                { new TimeSpan(5,0,0), 0}, // 11:59pm
                { new TimeSpan(6,0,0), 0}, // 11:59pm
                { new TimeSpan(7,0,0), 0}, // 11:59pm
                { new TimeSpan(8,0,0), 0}, // 11:59pm
                { new TimeSpan(9,0,0), 0}, // 11:59pm
                { new TimeSpan(10,0,0), 0}, // 11:59pm
                { new TimeSpan(11,0,0), 0} ,// 11:59pm
                { new TimeSpan(12,0,0), 0} ,// 11:59pm
                { new TimeSpan(13,0,0), 0}, // 11:59pm
                { new TimeSpan(14,0,0), 0}, // 11:59pm
                { new TimeSpan(15,0,0), 0}, // 11:59pm
                { new TimeSpan(16,0,0), 0}, // 11:59pm
                { new TimeSpan(17,0,0), 0}, // 11:59pm
                { new TimeSpan(18,0,0), 0} ,// 11:59pm
                { new TimeSpan(19,0,0), 0}, // 11:59pm
                { new TimeSpan(20,0,0), 0} ,// 11:59pm
                { new TimeSpan(21,0,0), 0} ,// 11:59pm
                { new TimeSpan(22,0,0), 0} ,// 11:59pm
                { new TimeSpan(23,0,0), 0} ,// 11:59pm
            };

                // then foreach track, determine time of day and increment count
                foreach (var track in tracks)
                {
                    var trackTime = track.TimeStamp.TimeOfDay;

                    // find closest hour slot
                    var hourSlot = new TimeSpan(trackTime.Hours, 0, 0);
                    if (timeOfDayCount.ContainsKey(hourSlot))
                    {
                        timeOfDayCount[hourSlot]++;
                    }
                }
                aggTrack.TimeOfDayStats = timeOfDayCount;
            }
        }

        // probably should get longest streak date start and end here. 
        public async Task CalculateLongestStreak()
        {
            // goal here is find the most amount of days in a row the artist was listened to
            var longestStreak = 0;
            var tempStreak = 0;

            var longestStreakEndDate = new DateTime();
            var longestStreakStartDate = new DateTime();


            if (_aggregatedTracks.Count == 0)
            {
                throw new InvalidOperationException("No aggregated artists found.");
            }

            foreach (var aggTrack in _aggregatedTracks)
            {

                // ideally ordered by date from oldest to newest
                var tracks = _context.ImportedTracks.Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.User.Id == aggTrack.User.Id).OrderBy(x => x.TimeStamp);

                var date = new DateTime();
                DateTime oneDateAhead = date.AddDays(1);

                foreach (var artistTrack in tracks)
                {
                    if (date.Date == DateTime.Parse("0001-01-01 00:00:00")) // ?? default date
                    {
                        tempStreak = longestStreak++;

                        date = artistTrack.TimeStamp;
                        oneDateAhead = date.Date.AddDays(1);
                        continue;
                    }
                    else if (date == artistTrack.TimeStamp)
                    {
                        // same day, we just move onto next track
                        continue;
                    }
                    else if (date == oneDateAhead)
                    {
                        tempStreak++;
                        if (tempStreak > longestStreak)
                        {
                            longestStreak = tempStreak;
                        }

                        date = artistTrack.TimeStamp;
                        oneDateAhead = date.AddDays(1);
                    }
                    else
                    {
                        longestStreakEndDate = date;
                        tempStreak = 0;
                    }
                }
                // hopefully works
                aggTrack.LongestStreakDays = longestStreak;
                aggTrack.LongestStreakEndDate = longestStreakEndDate;
                aggTrack.LongestStreakStartDate = longestStreakEndDate.AddDays(-longestStreak);
            }
        }

        public async Task CalculateDrySpell()
        {
            var dryStreak = 0;
            var dryStreakStartDate = new DateTime();
            var dryStreakEndDate = new DateTime();

            foreach (var aggTrack in _aggregatedTracks)
            {
                var tracks = _context.ImportedTracks.Where(x => x.MasterMetadataTrackName == aggTrack.Track.Name && x.User.Id == aggTrack.User.Id).OrderBy(x => x.TimeStamp).ToList();

                // we have list of tracks in order, we just have to fin dlongest date between date values.. 
                for (var i = 0; i < tracks.Count(); i++)
                {
                    if (dryStreak < (tracks[i].TimeStamp.Date - tracks[i - 1].TimeStamp.Date).Days)
                    {
                        dryStreak = (tracks[i].TimeStamp.Date - tracks[i - 1].TimeStamp.Date).Days;
                        dryStreakStartDate = tracks[i].TimeStamp.Date;
                        dryStreakEndDate = tracks[i - 1].TimeStamp.Date;
                    }
                }
                aggTrack.LongestDrySpellEnd = dryStreakEndDate;
                aggTrack.LongestStreakStartDate = dryStreakStartDate;
                aggTrack.LongestDrySpell = dryStreak;
            }
        }
    }
}
