namespace Ada.FirstCatering.API.Models.Responses;

public class CardSession
{
    public CardSession(string cardId)
    {
        CardId = cardId;
        UpdateExpiration();
    }

    public string CardId { get; set; }
    
    public Guid SessionId { get; set; } = Guid.NewGuid();

    public DateTime Expiration { get; set; }

    public void UpdateExpiration()
    {
        Expiration = DateTime.UtcNow.Add(TimeSpan.FromMinutes(2));
    }
}