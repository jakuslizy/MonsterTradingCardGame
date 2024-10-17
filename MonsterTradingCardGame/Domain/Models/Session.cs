namespace MonsterTradingCardGame.Domain.Models;

public class Session(string token, int userId, DateTime createdAt, DateTime expiresAt)
{
    public string Token { get; set; } = token;
    public int UserId { get; set; } = userId;
    public DateTime CreatedAt { get; set; } = createdAt;
    public DateTime ExpiresAt { get; set; } = expiresAt;
}