using MonsterTradingCardGame.Data.Repositories;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.API.Server.DTOs;

namespace MonsterTradingCardGame.Business.Services;

public interface ITradingService
{
    void CreateTrade(string id, string cardId, string type, int? minimumDamage, User user);
    List<Trading> GetTrades();
    void ExecuteTrade(string tradeId, string offeredCardId, User user);
    void DeleteTrade(string tradeId, User user);
}

public class TradingService : ITradingService
{
    private readonly TradingRepository _tradingRepository;
    private readonly CardRepository _cardRepository;

    public TradingService(TradingRepository tradingRepository, CardRepository cardRepository)
    {
        _tradingRepository = tradingRepository;
        _cardRepository = cardRepository;
    }

    public void CreateTrade(string id, string cardId, string type, int? minimumDamage, User user)
    {
        try 
        {
            Console.WriteLine($"Creating trade for card {cardId} by user {user.Id}");
            var card = _cardRepository.GetCardById(cardId);
            Console.WriteLine($"Card found: {card != null}, Card UserId: {card?.UserId}");
            
            if (card == null || card.UserId != Convert.ToInt32(user.Id))
            {
                throw new InvalidOperationException("Card not found or not owned by user");
            }

            if (card.InDeck)
            {
                throw new InvalidOperationException("Card is in deck");
            }

            var trading = new Trading(
                id,
                cardId,
                type,
                minimumDamage,
                Convert.ToInt32(user.Id)
            );

            _tradingRepository.CreateTrade(trading);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CreateTrade: {ex}");
            throw;
        }
    }

    public List<Trading> GetTrades()
    {
        return _tradingRepository.GetAllTrades().ToList();
    }

    public void ExecuteTrade(string tradeId, string offeredCardId, User user)
    {
        var trade = _tradingRepository.GetTrade(tradeId);
        if (trade == null)
        {
            throw new InvalidOperationException("Trading deal not found");
        }

        // Prüfe, ob der User nicht mit sich selbst handelt
        if (trade.UserId == Convert.ToInt32(user.Id))
        {
            throw new InvalidOperationException("Cannot trade with yourself");
        }

        // Prüfe ob die angebotene Karte dem User gehört
        var offeredCard = _cardRepository.GetCardById(offeredCardId);
        if (offeredCard == null || offeredCard.UserId != Convert.ToInt32(user.Id))
        {
            throw new InvalidOperationException("Card not found or not owned by user");
        }

        // Prüfe den Kartentyp (Monster oder Spell)
        if (!string.Equals(trade.Type, offeredCard.Name.Contains("Spell") ? "spell" : "monster", StringComparison.OrdinalIgnoreCase) || 
            (trade.MinimumDamage.HasValue && offeredCard.Damage < trade.MinimumDamage.Value))
        {
            throw new InvalidOperationException("Card does not meet trading requirements");
        }

        // Führe den Tausch durch
        _cardRepository.TransferCard(trade.CardToTrade, trade.UserId, Convert.ToInt32(user.Id));
        _cardRepository.TransferCard(offeredCardId, Convert.ToInt32(user.Id), trade.UserId);
        
        // Lösche den Trading-Deal
        _tradingRepository.DeleteTrade(tradeId);
    }

    public void DeleteTrade(string tradeId, User user)
    {
        var trade = _tradingRepository.GetTrade(tradeId);
        if (trade == null)
        {
            throw new InvalidOperationException("Trading deal not found");
        }

        if (trade.UserId != Convert.ToInt32(user.Id))
        {
            throw new InvalidOperationException("Trading deal can only be deleted by its creator");
        }

        _tradingRepository.DeleteTrade(tradeId);
    }

    // Weitere Methoden implementieren...
}