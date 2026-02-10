using SpotifyLocalStats.Server.Models;

namespace WebApi.Services.Interfaces;

public interface IUserService
{
    Task<User> GetUserById(Guid id);
    Task<User> CreateUser(string userName, string firstName);
}
