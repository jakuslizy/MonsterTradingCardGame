namespace MonsterTradingCardGame.Domain.Models;

public enum ElementType
{
    Normal,
    Fire,
    Water
}

public abstract class Card
{
    private static readonly Random Random = new();

    public string Id { get; }
    public string Name { get; }
    public int Damage { get; }
    public ElementType ElementType { get; }
    public int UserId { get; set; }
    public bool InDeck { get; set; }

    protected Card(string id, string name, int damage, ElementType elementType)
    {
        Id = id;
        Name = name;
        Damage = damage;
        ElementType = elementType;
        InDeck = false;
    }

    public virtual int GetDamageWithCritical()
    {
        return Random.Next(0, 100) < 10 ? Damage * 2 : Damage;
    }
}