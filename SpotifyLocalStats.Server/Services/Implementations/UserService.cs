using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public sealed class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly SpotifyStatsContext _context;

    public UserService(SpotifyStatsContext context, ILogger<UserService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(_context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<User?> GetUserById(Guid id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            _logger.LogWarning($"Could not find user with id ${id}");
        }

        _logger.LogInformation(user.UserName);

        return user;
    }

    public async Task<User> CreateUser(string userName, string firstName)
    {
        var user = new User(userName, firstName);
        user.LastUpdatedAt = DateTime.Now;
        user.LastTimeUsed = DateTime.Now;

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }
}
