namespace MonsterTradingCardGame.Data.Repositories.Interfaces;

public interface ITradingRepository
{
    IEnumerable<Trading> GetAllTrades();
    Trading? GetTrade(string id);
    void CreateTrade(Trading trade);
    void DeleteTrade(string id);
}