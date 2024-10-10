namespace MonsterTradingCardGame.Domain.Models
{
    public class SpellCard(string id, string name, int damage, ElementType elementType)
        : Card(id, name, damage, elementType);
}