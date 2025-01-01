using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Logic;

public class BattleLogic
{
    public int DetermineRoundWinner(Card card1, Card card2)
    {
        // Regel 1: Goblin vs Dragon - Goblin verliert immer
        if (card1.Name.Contains("Goblin") && card2.Name.Contains("Dragon"))
            return 2; // Dragon (card2) gewinnt
        if (card2.Name.Contains("Goblin") && card1.Name.Contains("Dragon"))
            return 1; // Dragon (card1) gewinnt

        // Regel 2: Wizard kontrolliert Ork
        if (card1.Name.Contains("Wizzard") && card2.Name.Contains("Ork"))
            return 1; // Wizard gewinnt
        if (card2.Name.Contains("Wizzard") && card1.Name.Contains("Ork"))
            return 2; // Wizard gewinnt

        // Regel 3: Knight vs WaterSpell - Knight ertrinkt
        if (card1.Name.Contains("Knight") && card2 is SpellCard spell2 && spell2.ElementType == ElementType.Water)
            return 2; // WaterSpell gewinnt
        if (card2.Name.Contains("Knight") && card1 is SpellCard spell1 && spell1.ElementType == ElementType.Water)
            return 1; // WaterSpell gewinnt

        // Regel 4: Kraken ist immun gegen Spells
        if (card1.Name.Contains("Kraken") && card2 is SpellCard)
            return 1; // Kraken gewinnt
        if (card2.Name.Contains("Kraken") && card1 is SpellCard)
            return 2; // Kraken gewinnt

        // Regel 5: FireElf weicht Dragon aus
        if (card1.Name.Contains("FireElf") && card2.Name.Contains("Dragon"))
            return card1.Damage > 0 ? 1 : 0; // FireElf gewinnt wenn Schaden > 0
        if (card2.Name.Contains("FireElf") && card1.Name.Contains("Dragon"))
            return card2.Damage > 0 ? 2 : 0; // FireElf gewinnt wenn Schaden > 0

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

        // Bei reinen Monster-Kämpfen keine Element-Modifikation
        if (attackerCard is MonsterCard && defenderCard is MonsterCard)
            return damage;

        // Wenn mindestens eine Spell-Karte beteiligt ist
        if (attackerCard is SpellCard || defenderCard is SpellCard)
        {
            if (attackerCard.ElementType == ElementType.Water && defenderCard.ElementType == ElementType.Fire)
                damage *= 2; // Wasser effektiv gegen Feuer
            else if (attackerCard.ElementType == ElementType.Fire && defenderCard.ElementType == ElementType.Normal)
                damage *= 2; // Feuer effektiv gegen Normal
            else if (attackerCard.ElementType == ElementType.Normal && defenderCard.ElementType == ElementType.Water)
                damage *= 2; // Normal effektiv gegen Wasser
            else if (attackerCard.ElementType == ElementType.Fire && defenderCard.ElementType == ElementType.Water)
                damage /= 2; // Feuer ineffektiv gegen Wasser
            else if (attackerCard.ElementType == ElementType.Normal && defenderCard.ElementType == ElementType.Fire)
                damage /= 2; // Normal ineffektiv gegen Feuer
            else if (attackerCard.ElementType == ElementType.Water && defenderCard.ElementType == ElementType.Normal)
                damage /= 2; // Wasser ineffektiv gegen Normal
        }

        return damage;
    }
}