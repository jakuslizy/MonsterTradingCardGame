namespace MonsterTradingCardGame.Domain.Models;

public class MonsterCard : Card
{
    public MonsterCard(string name, int damage, ElementType elementType) 
        : base(name, damage, elementType)
    {
    }

    public override int CalculateDamage(Card opponent)
    {
        // Bei Monster vs Monster nur der Basisschaden wird berücksichtigt.
        // Elemente sind nicht wichtig
        return Damage;
    }
}

