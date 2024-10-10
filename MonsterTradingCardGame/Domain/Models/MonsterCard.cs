namespace MonsterTradingCardGame.Domain.Models;

public class MonsterCard : Card
{
    protected MonsterCard(string id, string name, int damage, ElementType elementType)
        : base(id, name, damage, elementType)
    {
    }
}