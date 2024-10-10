namespace MonsterTradingCardGame.Domain.Models.MonsterCards;

public class Goblin : MonsterCard
{
    public Goblin(string id, int damage, ElementType elementType)
        : base(id, "Goblin", damage, elementType)
    {
    }
}