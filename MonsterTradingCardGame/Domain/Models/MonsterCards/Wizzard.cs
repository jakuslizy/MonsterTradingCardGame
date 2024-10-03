namespace MonsterTradingCardGame.Domain.Models.MonsterCards;

public class Wizzard : MonsterCard
{
    public Wizzard(string id, string name, int damage, ElementType elementType)
        : base(id, name, damage, elementType)
    {
    }
}