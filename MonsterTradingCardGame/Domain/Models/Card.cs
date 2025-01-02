namespace MonsterTradingCardGame.Domain.Models;

public enum ElementType
{
    Normal,
    Fire,
    Water
}

public abstract class Card(string id, string name, int damage, ElementType elementType)
{
    private static readonly Random Random = new();

    public string Id { get; } = id;
    public string Name { get; } = name;
    public int Damage { get; } = damage;
    public ElementType ElementType { get; } = elementType;
    public int UserId { get; set; }
    public bool InDeck { get; set; } = false;

    public virtual int GetDamageWithCritical()
    {
        return Random.Next(0, 100) < 10 ? Damage * 2 : Damage;
    }
}