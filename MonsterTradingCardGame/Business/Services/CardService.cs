using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Data.Repositories;
using MonsterTradingCardGame.Business.Factories;

namespace MonsterTradingCardGame.Business.Services;

public class CardService : ICardService
{
    private readonly IUserRepository _userRepository;  

    public CardService(IUserRepository userRepository)  
    {
        _userRepository = userRepository;
    }
    public IReadOnlyList<Card> GetUserCards(User user)
    {
        return _userRepository.GetUserCards(user.Id); 
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
        var userCards = _userRepository.GetUserCards(user.Id);
        
        // Prüfe, ob alle Karten dem User gehören
        foreach(var cardId in cardIds)
        {
            if (!userCards.Any(c => c.Id == cardId))
            {
                throw new InvalidOperationException($"Card {cardId} is not in user's stack");
            }
        }

        // Update direkt in der Datenbank
        _userRepository.UpdateUserDeck(user.Id, cardIds);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in ConfigureDeck: {ex}");
        throw;
    }
}


    public IReadOnlyList<Card> GetUserDeck(User user)
    {
        try
        {
            return _userRepository.GetUserDeck(user.Id);
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