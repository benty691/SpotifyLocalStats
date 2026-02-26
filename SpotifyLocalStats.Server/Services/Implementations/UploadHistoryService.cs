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
        var uploadHistoryList = await _context.UploadHistories.Where(x => x.UserId == userId).ToListAsync();

        var uploadHistoryResponse = new List<UploadHistoryResponseDto>();

        foreach (var uploadHistory in uploadHistoryList)
        {
            var importedTrackUploadHistory = await _context.ImportedTracks.Where(x => x.UserId == userId && x.UploadHistoryId == uploadHistory.Id).ToListAsync();

            uploadHistoryResponse.Add(
                new UploadHistoryResponseDto()
                {
                    FileName = uploadHistory.FileName,
                    ImportedTrackCount = importedTrackUploadHistory.Count,
                    CreatedAt = uploadHistory.CreatedAt
                }
            );
        }

        return uploadHistoryResponse;
    }
}
