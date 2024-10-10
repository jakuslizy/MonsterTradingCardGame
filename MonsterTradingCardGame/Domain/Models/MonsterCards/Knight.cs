namespace MonsterTradingCardGame.Domain.Models.MonsterCards;

public class Knight : MonsterCard
{
    public Knight(string id, string name, int damage, ElementType elementType)
        : base(id, "Knight", damage, elementType)
    {
    }
}