namespace MonsterTradingCardGame.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; private set; }
    public string PasswordHash { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public string? Name { get; set; }
    public string? Bio { get; set; }
    public string? Image { get; set; }

    public User(
        string username, 
        string passwordHash, 
        int id = 0, 
        DateTime? createdAt = null, 
        int coins = 20)
    {
        Id = id;
        Username = username;
        PasswordHash = passwordHash;
        CreatedAt = createdAt ?? DateTime.UtcNow;
    }
}
