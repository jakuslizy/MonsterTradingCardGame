namespace MonsterTradingCardGame.Domain.Models.MonsterCards;

public class Ork : MonsterCard
{
    public Ork(string id, string name, int damage, ElementType elementType)
        : base(id, name, damage, elementType)
    {
    }
}