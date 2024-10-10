namespace MonsterTradingCardGame.Domain.Models.MonsterCards;

public class Dragon : MonsterCard
{
    public Dragon(string id, string name, int damage, ElementType elementType)
        : base(id, "Dragon", damage, elementType)
    {
    }
}