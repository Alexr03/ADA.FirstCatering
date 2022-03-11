using Ada.FirstCatering.API.Models.Responses;

namespace Ada.FirstCatering.API.Services;

public class CardSessionService
{
    private Dictionary<string, CardSession> CardSessions { get; set; } = new();

    public CardSession? GetCardSession(string cardId)
    {
        var session = CardSessions.TryGetValue(cardId, out var cardSession) ? cardSession : null;
        if (session != null)
        {
            if(session.Expiration < DateTime.UtcNow)
            {
                DeleteCardSession(cardId);
                return null;
            }
        }

        return session;
    }
    
    public CardSession CreateCardSession(string cardId)
    {
        var cardSession = new CardSession(cardId);
        CardSessions.Add(cardSession.CardId, cardSession);
        return cardSession;
    }
    
    public void DeleteCardSession(string cardId)
    {
        CardSessions.Remove(cardId);
    }

    public void UpdateCardSessionExpiration(string cardId)
    {
        GetCardSession(cardId)?.UpdateExpiration();
    }
}