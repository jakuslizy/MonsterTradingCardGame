using MonsterTradingCardGame.Data;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Domain.Models.MonsterCards;
using MonsterTradingCardGame.Data.Repositories;

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
        // Element aus dem Namen extrahieren
        elementType = ElementType.Normal;
        if (name.StartsWith("Water")) elementType = ElementType.Water;
        if (name.StartsWith("Fire")) elementType = ElementType.Fire;
        
        // Kartentyp aus dem Namen extrahieren
        if (name.EndsWith("Spell"))
        {
            return new SpellCard(id, name, damage, elementType);
        }
        
        // Monster-Karten
        if (name.EndsWith("Goblin")) return new Goblin(id, name, damage, elementType);
        if (name.EndsWith("Dragon")) return new Dragon(id, name, damage, elementType);
        if (name.EndsWith("Wizard")) return new Wizzard(id, name, damage, elementType);
        if (name.EndsWith("Ork")) return new Ork(id, name, damage, elementType);
        if (name.EndsWith("Knight")) return new Knight(id, name, damage, elementType);
        if (name.EndsWith("Kraken")) return new Kraken(id, name, damage, elementType);
        if (name.Contains("FireElf")) return new FireElf(id, name, damage, elementType);
        
        throw new InvalidOperationException($"Unknown card type: {name}");
    }
}