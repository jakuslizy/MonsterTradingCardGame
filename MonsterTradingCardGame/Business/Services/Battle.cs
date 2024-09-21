using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services;

public class Battle
{
    private User Player1 { get; set; }
    private User Player2 { get; set; }
    private const int MaxRounds = 100;
    
    public Battle(User player1, User player2)
    {
        Player1 = player1;
        Player2 = player2;
    }
}