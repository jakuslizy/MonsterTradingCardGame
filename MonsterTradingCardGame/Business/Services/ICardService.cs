using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services;

public interface ICardService
{
    void CreatePackage(List<Card> cards);
    IReadOnlyList<Card> GetUserCards(User user);
    void ConfigureDeck(User user, List<string> cardIds);
    Card CreateCard(string id, string name, int damage, ElementType elementType);
}