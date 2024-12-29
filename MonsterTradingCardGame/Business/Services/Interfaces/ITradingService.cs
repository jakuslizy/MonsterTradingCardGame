using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services.Interfaces;

public interface ITradingService
{
    void CreateTrade(string id, string cardId, string type, int? minimumDamage, User user);
    List<Trading> GetTrades();
    void ExecuteTrade(string tradeId, string offeredCardId, User user);
    void DeleteTrade(string tradeId, User user);
}