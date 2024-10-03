namespace MonsterTradingCardGame.Domain.Models;

public class MonsterCard : Card
{
    public MonsterCard(string id, string name, int damage, ElementType elementType) 
        : base(name, damage, elementType)
    {
    }
}

