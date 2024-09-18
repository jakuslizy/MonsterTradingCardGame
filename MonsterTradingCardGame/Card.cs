namespace MonsterTradingCardGame;

public enum ElementType
{
    Normal,
    Fire,
    Water
} 

public abstract class Card
{
    public string name { get; protected set; }
    public int damage { get; protected set; }
    public ElementType elementType { get;  protected set; }

    public Card(string name, int damage, ElementType elementType)
    {
        this.name = name;
        this.damage = damage;
        this.elementType = elementType;
    }
}