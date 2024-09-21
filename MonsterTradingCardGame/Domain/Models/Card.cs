namespace MonsterTradingCardGame.Domain.Models;

public enum ElementType
{
    Normal,
    Fire,
    Water
} 

public abstract class Card(string name, int damage, ElementType elementType)
{
    public string Name { get; protected set; } = name;
    protected int Damage { get; set; } = damage;
    public ElementType ElementType { get;  protected set; } = elementType;

    public virtual int CalculateDamage(Card opponent)
    {
        return Damage;
    }
}