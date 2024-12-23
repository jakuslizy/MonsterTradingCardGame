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

        //InMemoryDatabase.AddPackage(package);
    }

    // public List<Card> AcquirePackage(User user)
    // {
    //     if (user.Coins < Package.PackagePrice)
    //     {
    //         throw new InvalidOperationException("Not enough coins");
    //     }
    //
    //     var package = InMemoryDatabase.GetPackage();
    //     if (package == null)
    //     {
    //         throw new InvalidOperationException("No packages available");
    //     }
    //
    //     user.UpdateCoins(user.Coins - Package.PackagePrice);
    //     foreach (var card in package.GetCards())
    //     {
    //         user.AddCardToStack(card);
    //     }
    //
    //     return package.GetCards().ToList();
    // }


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
        
        // Erstelle eine neue Liste für das Deck
        var selectedCards = new List<Card>();
        foreach(var cardId in cardIds)
        {
            var card = userCards.FirstOrDefault(c => c.Id == cardId);
            if (card == null)
            {
                throw new InvalidOperationException($"Card {cardId} is not in user's stack");
            }
            selectedCards.Add(card);
        }

        // Setze das neue Deck
        user.SetDeck(selectedCards);
        _userRepository.UpdateUserDeck(user.Id, cardIds);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error in ConfigureDeck: {ex}");
        throw;
    }
}

    public Card CreateCard(string id, string name, int damage, ElementType elementType)
    {
        // Spell-Karten
        if (name.Contains("Spell"))
        {
            return new SpellCard(id, name, damage, elementType);
        }

        // Monster-Karten
        return name switch
        {
            var n when n.Contains("Dragon") => new Dragon(id, name, damage, elementType),
            var n when n.Contains("FireElf") => new FireElf(id, name, damage, elementType),
            var n when n.Contains("Kraken") => new Kraken(id, name, damage, elementType),
            var n when n.Contains("Knight") => new Knight(id, name, damage, elementType),
            var n when n.Contains("Wizard") => new Wizzard(id, name, damage, elementType),
            var n when n.Contains("Ork") => new Ork(id, name, damage, elementType),
            var n when n.Contains("Goblin") => new Goblin(id, name, damage, elementType),
            _ => new Dragon(id, name, damage, elementType)
        };
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
}