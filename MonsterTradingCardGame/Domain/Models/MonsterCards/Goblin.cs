namespace MonsterTradingCardGame.Domain.Models.MonsterCards;

public class Goblin : MonsterCard
{
    public Goblin(string id, string name, int damage, ElementType elementType)
        : base(id, name, damage, elementType)
    {
    }
}