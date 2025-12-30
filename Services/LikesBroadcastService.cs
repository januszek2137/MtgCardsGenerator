using System.Collections.Concurrent;

namespace AiMagicCardsGenerator.Services;

public class LikesBroadcastService : ILikesBroadcastService {
    private readonly ConcurrentDictionary<string, StreamWriter> _clients = new();

    public void Subscribe(string connectionId, StreamWriter writer) =>
        _clients.TryAdd(connectionId, writer);

    public void Unsubscribe(string connectionId) =>
        _clients.TryRemove(connectionId, out _);

    public async Task BroadcastAsync(int cardId, int likes) {
        var message = $"data: {cardId}:{likes}\n\n";
        var dead    = new List<string>();

        foreach (var (id, writer) in _clients) {
            try {
                await writer.WriteAsync(message);
                await writer.FlushAsync();
            }
            catch {
                dead.Add(id);
            }
        }

        dead.ForEach(id => _clients.TryRemove(id, out _));
    }
}