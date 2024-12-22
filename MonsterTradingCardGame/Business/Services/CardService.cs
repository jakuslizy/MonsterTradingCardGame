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
        if (cardIds.Count != 4)
        {
            throw new InvalidOperationException("Deck must contain exactly 4 cards");
        }

        user.Deck.Clear();
        foreach (var cardId in cardIds)
        {
            var card = user.GetStack().FirstOrDefault(c => c.Id == cardId);
            if (card == null)
            {
                throw new InvalidOperationException($"Card with ID {cardId} not found in user's stack");
            }

            user.AddCardToDeck(card);
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
}