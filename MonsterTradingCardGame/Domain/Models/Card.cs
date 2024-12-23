namespace MonsterTradingCardGame.Domain.Models;

public enum ElementType
{
    Normal,
    Fire,
    Water
}

public abstract class Card(string id, string name, int damage, ElementType elementType)
{
    public string Id { get; } = id;
    public string Name { get; } = name;
    public int Damage { get; } = damage;
    public ElementType ElementType { get; } = elementType;
    public bool InDeck { get; set; } = false;

    public override string ToString()
    {
        return $"{Name} (Damage: {Damage}, Element: {ElementType})";
    }
    public override bool Equals(object? obj)
    {
        if (obj is Card other)
        {
            return Id == other.Id;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
}