namespace MonsterTradingCardGame.Domain.Models;

public class MonsterCard(string name, int damage, ElementType elementType) : Card(name, damage, elementType)
{
    public override int CalculateDamage(Card opponent)
    {
        // Wenn Gegner SpellCard ist, können Elementar-Effektivitäten berücksichtigt werden.
        if (opponent is SpellCard)
        {
            return base.CalculateDamage(opponent);
        }
        // Bei Monster vs Monster: Elemente sind nicht wichtig
        return Damage;
    }
}

