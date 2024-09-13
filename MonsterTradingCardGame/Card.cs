namespace MonsterTradingCardGame;

public enum ElementType
{
    Normal,
    Fire,
    Water
} 

public abstract class Card
{
    public string Name { get; set; }
    public int Damage { get; set; }
    public ElementType ElementType { get; set; }
}