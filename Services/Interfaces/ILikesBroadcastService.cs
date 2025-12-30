namespace AiMagicCardsGenerator.Services;

public interface ILikesBroadcastService {
    void Subscribe(string   connectionId, StreamWriter writer);
    void Unsubscribe(string connectionId);
    Task BroadcastAsync(int cardId, int likes);
}