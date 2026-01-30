using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using System;

namespace WebApi.Services
{
    public abstract class BaseService
    {
        protected readonly SpotifyStatsContext _context;

        // Constructor injection
        public BaseService(SpotifyStatsContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Common methods or properties can be added here
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
