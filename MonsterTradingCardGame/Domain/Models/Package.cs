namespace MonsterTradingCardGame.Domain.Models;

public class Package
{
    public int Id { get; set; }
    public int Price { get; set; } = PackagePrice;
    private List<Card> Cards { get; } = new List<Card>();
    public const int PackagePrice = 5;
    private const int CardsPerPackage = 5;

    public int? PurchasedBy { get; set; }
    public DateTime? PurchasedAt { get; set; }
    public DateTime CreatedAt { get; set; }

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

    public bool IsComplete() => Cards.Count == CardsPerPackage;

    public IReadOnlyList<Card> GetCards() => Cards.AsReadOnly();
}