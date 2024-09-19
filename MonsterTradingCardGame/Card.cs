namespace MonsterTradingCardGame;

public enum ElementType
{
    Normal,
    Fire,
    Water
} 

public abstract class Card
{
    public string Name { get; protected set; }
    public int Damage { get; protected set; }
    public ElementType ElementType { get;  protected set; }

    public Card(string name, int damage, ElementType elementType)
    {
        Name = name;
        Damage = damage;
        ElementType = elementType;
    }
}