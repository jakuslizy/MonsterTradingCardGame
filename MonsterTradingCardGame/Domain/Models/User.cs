namespace MonsterTradingCardGame.Domain.Models;

public class User(
    string username,
    string passwordHash,
    int id = 0,
    DateTime? createdAt = null,
    int coins = 20)
{
    public int Id { get; set; } = id;
    public string Username { get; private set; } = username;
    public string PasswordHash { get; private set; } = passwordHash;
    public DateTime CreatedAt { get; private set; } = createdAt ?? DateTime.UtcNow;
    public string? Name { get; set; }
    public string? Bio { get; set; }
    public string? Image { get; set; }
    public int Coins { get; set; } = coins;
}