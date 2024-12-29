using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Data.Repositories;

public interface IStatsRepository
{
    void CreateStats(Stats stats);
    Stats? GetStatsByUserId(int userId);
    void UpdateStats(Stats stats);
    List<Stats> GetAllStats();
}