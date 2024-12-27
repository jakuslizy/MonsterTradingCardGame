using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Data.Repositories;

public interface ICardRepository
{
    void AddCard(Card card, int userId);
    void UpdateCard(Card card);
    Card? GetCardById(string id);
    List<Card> GetCardsByUserId(int userId);
    void TransferCard(string cardId, int fromUserId, int toUserId);
    void UpdateCardDeckStatus(string cardId, bool inDeck);
    Card? CreateCard(string id, string name, int damage, ElementType elementType);
}