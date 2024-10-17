namespace MonsterTradingCardGame.Domain.Models;

public class Session
{
    public string Token { get; set; }
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }

    public Session(string token, int userId, DateTime createdAt, DateTime expiresAt)
    {
        Token = token;
        UserId = userId;
        CreatedAt = createdAt;
        ExpiresAt = expiresAt;
    }
}
