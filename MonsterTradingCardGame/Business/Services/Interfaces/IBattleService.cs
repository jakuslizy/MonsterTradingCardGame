using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services.Interfaces;

public interface IBattleService
{
    string ExecuteBattle(User player1, User player2);
}