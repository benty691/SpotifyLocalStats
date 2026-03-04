using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using SpotifyLocalStats.Server.Models;
using WebApi.Data.DTOs.NewFolder;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public class UserAggregateService<TAggregate, TAggregateDto> : IUserAggregateService<TAggregateDto>
    where TAggregate : AggregateBase
    where TAggregateDto : AggregateBaseDto
{
    private readonly SpotifyStatsContext _context;
    private readonly ILogger<UserAggregateService<TAggregate, TAggregateDto>> _logger;
    private readonly Func<IQueryable<TAggregate>, IQueryable<TAggregate>> _includeBuilder;
    private readonly Func<TAggregate, TAggregateDto> _mapper;

    public UserAggregateService(
        ILogger<UserAggregateService<TAggregate, TAggregateDto>> logger,
        SpotifyStatsContext context,
        Func<IQueryable<TAggregate>, IQueryable<TAggregate>> includeBuilder,
        Func<TAggregate, TAggregateDto> mapper)
    {
        _logger = logger;
        _context = context;
        _includeBuilder = includeBuilder;
        _mapper = mapper;
    }

    public async Task<List<TAggregateDto>> GetAggregate(User user, int pageNumber)
    {
        var query = _context.Set<TAggregate>()
            .Where(x => x.UserId == user.Id);

        query = _includeBuilder(query);

        const int pageSize = 50;

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var clampedPage = Math.Clamp(pageNumber, 1, Math.Max(1, totalPages));

        var aggregates = await query
            .OrderByDescending(x => x.PlayCount)
            .Skip(pageSize * (clampedPage - 1))
            .Take(pageSize)
            .ToListAsync();

        //return //new AggergateRespnseDto<AggregateArtistDto>() { Aggregate =
        return aggregates.Select(_mapper).ToList();
        //, RecordCount = totalCount };
    }
}
