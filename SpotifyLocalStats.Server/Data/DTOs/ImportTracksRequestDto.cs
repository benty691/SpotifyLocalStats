using SpotifyLocalStats.Server.Data;

namespace WebApi.Data.DTOs;

public class ImportTrackFilesDto
{
    public ImportTrackFilesDto()
    {
        FormFile = new List<IFormFile>();
    }
    public List<IFormFile> FormFile { get; set; }
}