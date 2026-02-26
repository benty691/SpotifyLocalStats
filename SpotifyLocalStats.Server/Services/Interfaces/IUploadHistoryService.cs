using WebApi.Data.DTOs;

namespace WebApi.Services.Interfaces;

public interface IUploadHistoryService
{
    Task<List<UploadHistoryResponseDto>> GetUploadHistory(Guid userId);

}
