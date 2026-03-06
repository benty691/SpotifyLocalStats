using WebApi.Data.DTOs.NewFolder;

namespace WebApi.Data.DTOs.AggregateDtos;

public class AggregateResponseDto<TAggregateDto> where TAggregateDto : AggregateBaseDto
{
    public List<TAggregateDto> Aggregate { get; set; }
    public int RecordCount { get; set; }
}
