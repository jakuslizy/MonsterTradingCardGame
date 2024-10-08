using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Logic;

public class BattleLogic
{
    public int DetermineRoundWinner(Card card1, Card card2)
    {
        // Spezielle Regeln
        if (card1.Name.Contains("Goblin") && card2.Name.Contains("Dragon"))
            return 2;
        if (card2.Name.Contains("Goblin") && card1.Name.Contains("Dragon"))
            return 1;
        if (card1.Name.Contains("Wizzard") && card2.Name.Contains("Ork"))
            return 1;
        if (card2.Name.Contains("Wizzard") && card1.Name.Contains("Ork"))
            return 2;
        if (card1.Name.Contains("Knight") && card2 is SpellCard spellCard2 &&
            spellCard2.ElementType == ElementType.Water)
            return 2;
        if (card2.Name.Contains("Knight") && card1 is SpellCard spellCard1 &&
            spellCard1.ElementType == ElementType.Water)
            return 1;
        if (card1.Name.Contains("Kraken") && card2 is SpellCard)
            return 1;
        if (card2.Name.Contains("Kraken") && card1 is SpellCard)
            return 2;
        if (card1.Name.Contains("FireElve") && card2.Name.Contains("Dragon"))
            return 1;
        if (card2.Name.Contains("FireElve") && card1.Name.Contains("Dragon"))
            return 2;

        // Normale Schadensberechnung
        int damage1 = CalculateDamage(card1, card2);
        int damage2 = CalculateDamage(card2, card1);

        if (damage1 > damage2) return 1;
        if (damage2 > damage1) return 2;
        return 0; // Unentschieden
    }

    public int CalculateDamage(Card attackerCard, Card defenderCard)
    {
        int damage = attackerCard.Damage;

        if (attackerCard is SpellCard || defenderCard is SpellCard)
        {
            if (IsEffectiveAgainst(attackerCard.ElementType, defenderCard.ElementType))
                damage *= 2;
            else if (IsWeakAgainst(attackerCard.ElementType, defenderCard.ElementType))
                damage /= 2;
        }

        return damage;
    }

    private bool IsEffectiveAgainst(ElementType attacker, ElementType defender)
    {
        return (attacker == ElementType.Water && defender == ElementType.Fire) ||
               (attacker == ElementType.Fire && defender == ElementType.Normal) ||
               (attacker == ElementType.Normal && defender == ElementType.Water);
    }

    private bool IsWeakAgainst(ElementType attacker, ElementType defender)
    {
        return IsEffectiveAgainst(defender, attacker);
    }
}