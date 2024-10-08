namespace MonsterTradingCardGame.Domain.Models;

public enum ElementType
{
    Normal,
    Fire,
    Water
}

public abstract class Card
{
    public string Name { get; }
    public int Damage { get; }
    public ElementType ElementType { get; }

    protected Card(string name, int damage, ElementType elementType)
    {
        Name = name;
        Damage = damage;
        ElementType = elementType;
    }

    public override string ToString()
    {
        return $"{Name} (Damage: {Damage}, Element: {ElementType})";
    }
}