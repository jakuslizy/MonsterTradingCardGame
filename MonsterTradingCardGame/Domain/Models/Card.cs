namespace MonsterTradingCardGame.Domain.Models;

public enum ElementType
{
    Normal,
    Fire,
    Water
}

public abstract class Card
{
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
}