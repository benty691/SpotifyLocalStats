using Microsoft.EntityFrameworkCore;
using SpotifyLocalStats.Server.Data;
using WebApi.Data.DTOs;
using WebApi.Services.Interfaces;

namespace WebApi.Services.Implementations;

public class UploadHistoryService : IUploadHistoryService
{
    private readonly SpotifyStatsContext _context;
    private readonly ILogger<UploadHistoryService> _logger;

    public UploadHistoryService(SpotifyStatsContext context, ILogger<UploadHistoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<List<UploadHistoryResponseDto>> GetUploadHistory(Guid userId)
    {
        return await _context.UploadHistories
        .Where(x => x.UserId == userId)
        .Select(x => new UploadHistoryResponseDto
        {
            CreatedAt = x.CreatedAt,
            FileName = x.FileName,
            ImportedTrackCount = _context.ImportedTracks
                .Count(t => t.UploadHistoryId == x.Id)
        })
        .ToListAsync();
    }
}
