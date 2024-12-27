namespace MonsterTradingCardGame.Data.Repositories;

public interface ITradingRepository
{
    IEnumerable<Trading> GetAllTrades();
    Trading? GetTrade(string id);
    void CreateTrade(Trading trade);
    void DeleteTrade(string id);
}