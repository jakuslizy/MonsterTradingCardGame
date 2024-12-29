using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Data.Repositories;
using MonsterTradingCardGame.Business.Factories;
using MonsterTradingCardGame.Business.Services.Interfaces;

namespace MonsterTradingCardGame.Business.Services;

public class CardService(IUserRepository userRepository) : ICardService
{
    public IReadOnlyList<Card> GetUserCards(User user)
    {
        return userRepository.GetUserCards(user.Id);
    }

    public void CreatePackage(List<Card> cards)
    {
        var package = new Package();
        foreach (var card in cards)
        {
            package.AddCard(card);
        }
    }


    public void ConfigureDeck(User user, List<string> cardIds)
    {
        try
        {
            if (cardIds.Count != 4)
            {
                throw new InvalidOperationException("Deck must contain exactly 4 cards");
            }

            // Hole alle Karten des Users direkt aus der Datenbank
            var userCards = userRepository.GetUserCards(user.Id);

            // Debug-Ausgabe
            Console.WriteLine($"User {user.Username} hat {userCards.Count} Karten:");
            foreach (var card in userCards)
            {
                Console.WriteLine($"- {card.Id}: {card.Name}");
            }

            Console.WriteLine("\nVersuche folgende Karten ins Deck zu legen:");
            foreach (var cardId in cardIds)
            {
                Console.WriteLine($"- {cardId}");
            }

            // Prüfe, ob alle Karten dem User gehören
            var missingCards = cardIds.Where(id => !userCards.Any(c => c.Id == id)).ToList();
            if (missingCards.Any())
            {
                throw new InvalidOperationException(
                    $"Folgende Karten wurden nicht im Stack des Users gefunden: {string.Join(", ", missingCards)}");
            }

            // Update direkt in der Datenbank
            userRepository.UpdateUserDeck(user.Id, cardIds);
        }
        catch (InvalidOperationException)
        {
            throw;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fehler in ConfigureDeck: {ex}");
            throw new InvalidOperationException($"Fehler beim Konfigurieren des Decks: {ex.Message}");
        }
    }


    public IReadOnlyList<Card> GetUserDeck(User user)
    {
        try
        {
            return userRepository.GetUserDeck(user.Id);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUserDeck: {ex}");
            throw;
        }
    }

    public Card? CreateCard(string id, string name, int damage, ElementType elementType)
    {
        return CardFactory.CreateCard(id, name, damage, elementType);
    }
}