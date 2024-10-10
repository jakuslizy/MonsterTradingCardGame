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

    public override string ToString()
    {
        return $"{Name} (Damage: {Damage}, Element: {ElementType})";
    }
}