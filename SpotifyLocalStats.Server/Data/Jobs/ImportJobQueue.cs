using System.Threading.Channels;

namespace WebApi.Data.Jobs;

public class ImportJobQueue
{
    private readonly Channel<ImportJobData> _channel = Channel.CreateUnbounded<ImportJobData>();

    public async Task EnqueAsync(ImportJobData data) => await _channel.Writer.WriteAsync(data);

    public IAsyncEnumerable<ImportJobData> ReadAllAsync(CancellationToken cancellationToken) => _channel.Reader.ReadAllAsync(cancellationToken);
}
