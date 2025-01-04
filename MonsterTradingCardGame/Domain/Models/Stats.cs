namespace MonsterTradingCardGame.Domain.Models;

public class Stats(int userId, int gamesPlayed = 0, int gamesWon = 0, int gamesLost = 0, int elo = 100)
{
    public int Id { get; set; }
    public int UserId { get; set; } = userId;
    public string Name { get; init; } = string.Empty;
    public int GamesPlayed { get; set; } = gamesPlayed;
    public int GamesWon { get; set; } = gamesWon;
    public int GamesLost { get; set; } = gamesLost;
    public int Elo { get; set; } = elo;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}