using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services;

public class Battle(User player1, User player2)
{
    private User Player1 { get; set; } = player1;
    private User Player2 { get; set; } = player2;
    private const int MaxRounds = 100;
}