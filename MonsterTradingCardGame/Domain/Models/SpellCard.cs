namespace MonsterTradingCardGame.Domain.Models
{
    public class SpellCard(string name, int damage, ElementType elementType) : Card(name, damage, elementType)
    {
        public override int CalculateDamage(Card opponent)
        {
            int baseDamage = base.CalculateDamage(opponent);
            return CalculateElementalDamage(opponent, baseDamage);
        }
        
        private int CalculateElementalDamage(Card opponent, int baseDamage)
        {
            if (IsEffectiveAgainst(opponent.ElementType))
            {
                return baseDamage * 2;
            }
            else if (IsWeakAgainst(opponent.ElementType))
            {
                return baseDamage / 2;
            }
            return baseDamage;
        }
        
        private bool IsEffectiveAgainst(ElementType opponentElement)
        {
            return (ElementType == ElementType.Water && opponentElement == ElementType.Fire) ||
                   (ElementType == ElementType.Fire && opponentElement == ElementType.Normal) ||
                   (ElementType == ElementType.Normal && opponentElement == ElementType.Water);
        }
        private bool IsWeakAgainst(ElementType opponentElement)
        {
            return (ElementType == ElementType.Fire && opponentElement == ElementType.Water) ||
                   (ElementType == ElementType.Normal && opponentElement == ElementType.Fire) ||
                   (ElementType == ElementType.Water && opponentElement == ElementType.Normal);
        }
    }
}