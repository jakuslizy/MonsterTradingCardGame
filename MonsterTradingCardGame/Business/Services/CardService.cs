using MonsterTradingCardGame.Data;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services;

public class CardService : ICardService
{
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

    public IReadOnlyList<Card> GetUserCards(User user)
    {
        return user.GetStack();
    }

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
}