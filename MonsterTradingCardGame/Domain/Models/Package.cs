namespace MonsterTradingCardGame.Domain.Models;

public class Package
{
    private List<Card> Cards { get; } = []; //= new List<Card>()
    public const int PackagePrice = 5;
    private const int CardsPerPackage = 5;
    
    public void AddCard(Card card)
    {
        if (Cards.Count < CardsPerPackage)
        {
            Cards.Add(card);
        }
        else
        {
            throw new InvalidOperationException("Package is already full");
        }
    }
    
    //public bool IsComplete()
    // {
    //     return Cards.Count == CardsPerPackage;
    // }
    public bool IsComplete() => Cards.Count == CardsPerPackage;

    //vordefiniertes Interface, AsReadOnly erstellt keine Kopie der Liste
    public IReadOnlyList<Card> GetCards() => Cards.AsReadOnly();
}