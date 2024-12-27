namespace MonsterTradingCardGame.Domain.Models;

public class Stats
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int GamesPlayed { get; set; }
    public int GamesWon { get; set; }
    public int GamesLost { get; set; }
    public int Elo { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Stats(int userId, int gamesPlayed = 0, int gamesWon = 0, int gamesLost = 0, int elo = 100)
    {
        UserId = userId;
        GamesPlayed = gamesPlayed;
        GamesWon = gamesWon;
        GamesLost = gamesLost;
        Elo = elo;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}