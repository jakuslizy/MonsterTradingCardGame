using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server;

public interface IBattleQueue
{
    User? GetWaitingPlayer();
    void AddPlayer(User player);
    void RemovePlayer(User player);
}