namespace MonsterTradingCardGame.Domain.Models
{
    public class SpellCard : Card
    {
        public SpellCard(string name, int damage, ElementType elementType)
            : base(name, damage, elementType)
        {
        }
    }
}