using WebApi.Data.DTOs.NewFolder;

namespace WebApi.Data.DTOs.AggregateDtos;

public class AggergateRespnseDto<T> where T : AggregateBaseDto
{
    public List<T> Aggregate { get; set; }
    public int RecordCount { get; set; }
}
