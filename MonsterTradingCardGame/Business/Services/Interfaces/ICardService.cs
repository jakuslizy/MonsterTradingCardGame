using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services.Interfaces;

public interface ICardService
{
    void CreatePackage(List<Card> cards);
    IReadOnlyList<Card> GetUserCards(User user);
    void ConfigureDeck(User user, List<string> cardIds);
    IReadOnlyList<Card> GetUserDeck(User user);
    Card? CreateCard(string id, string name, int damage, ElementType elementType);
}