namespace MonsterTradingCardGame.Domain.Models.MonsterCards;

public class Ork : MonsterCard
{
    public Ork(string id, int damage, ElementType elementType)
        : base(id, "Ork", damage, elementType)
    {
    }
}