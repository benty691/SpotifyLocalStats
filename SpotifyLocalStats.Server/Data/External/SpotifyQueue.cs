using System.Threading.Channels;

namespace WebApi.Data.External;

public class SpotifyArtistQueue
{
    // i want to create a queue, but dont want to specify what type of queue it is, so a spotify artist queue, or a alubm etc, without hvaing to crteate three seperate queues. my underatdning is we can use a factory pattern here, but not quite sure hoiw? because we cannot use geenrics as we dont have a base class for hese three types... 

    private readonly Channel<SpotifyArtistData> _channel = Channel.CreateUnbounded<SpotifyArtistData>();

    public async Task EnqueAsync(SpotifyArtistData data) => await _channel.Writer.WriteAsync(data);

    public IAsyncEnumerable<SpotifyArtistData> ReadAllAsync(CancellationToken cancellationToken) => _channel.Reader.ReadAllAsync(cancellationToken);
}
