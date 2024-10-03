namespace MonsterTradingCardGame.Domain.Models.MonsterCards;

public class FireElf : MonsterCard
{
    public FireElf(string id, string name, int damage, ElementType elementType)
        : base(id, name, damage, elementType)
    {
    }
}