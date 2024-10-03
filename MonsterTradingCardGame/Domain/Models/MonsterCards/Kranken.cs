namespace MonsterTradingCardGame.Domain.Models.MonsterCards;

public class Kraken : MonsterCard
{
    public Kraken(string id, string name, int damage, ElementType elementType)
        : base(id, name, damage, elementType)
    {
    }
}