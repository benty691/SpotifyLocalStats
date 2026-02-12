using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public sealed class UserService : IUserService
{
    private readonly SpotifyStatsContext _context;

    public UserService(SpotifyStatsContext context) 
    {
        _context = context ?? throw new ArgumentNullException(nameof(_context));
    }

    public async Task<User?> GetUserById(Guid id)
    {
        return await _context.Users.SingleOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User> CreateUser(string userName, string firstName)
    {
        var user = new User(userName, firstName);

        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();

        return user;
    }
}
