using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Logic;

public class BattleLogic
{
    public int CalculateDamage(Card attackerCard, Card defenderCard)
    {
        // Basis-Schaden berechnen (mit möglichem kritischen Treffer)
        int damage = attackerCard.GetDamageWithCritical();

        // Bei reinen Monster-Kämpfen keine Element-Modifikation
        if (attackerCard is MonsterCard && defenderCard is MonsterCard)
            return damage;

        // Element-Effektivität bei Spell-Karten
        if (attackerCard is SpellCard || defenderCard is SpellCard)
        {
            double multiplier = GetElementalMultiplier(attackerCard.ElementType, defenderCard.ElementType);
            damage = (int)(damage * multiplier);
        }

        return damage;
    }

    public int DetermineRoundWinner(Card card1, Card card2)
    {
        // Spezialregeln prüfen
        if (card1.Name.Contains("Goblin") && card2.Name.Contains("Dragon"))
            return 2; // Dragon gewinnt automatisch
        if (card2.Name.Contains("Goblin") && card1.Name.Contains("Dragon"))
            return 1; // Dragon gewinnt automatisch

        if (card1.Name.Contains("Wizzard") && card2.Name.Contains("Ork"))
            return 1; // Wizard kontrolliert Ork
        if (card2.Name.Contains("Wizzard") && card1.Name.Contains("Ork"))
            return 2; // Wizard kontrolliert Ork

        if (card1.Name.Contains("Knight") && card2 is SpellCard { ElementType: ElementType.Water })
            return 2; // WaterSpell ertränkt Knight
        if (card2.Name.Contains("Knight") && card1 is SpellCard { ElementType: ElementType.Water })
            return 1; // WaterSpell ertränkt Knight

        if (card1.Name.Contains("Kraken") && card2 is SpellCard)
            return 1; // Kraken ist immun gegen Spells
        if (card2.Name.Contains("Kraken") && card1 is SpellCard)
            return 2; // Kraken ist immun gegen Spells

        if (card1.Name.Contains("FireElf") && card2.Name.Contains("Dragon"))
            return 1; // FireElf weicht Dragon aus
        if (card2.Name.Contains("FireElf") && card1.Name.Contains("Dragon"))
            return 2; // FireElf weicht Dragon aus

        // Normaler Schadenvergleich mit kritischen Treffern
        int damage1 = CalculateDamage(card1, card2);
        int damage2 = CalculateDamage(card2, card1);

        if (damage1 > damage2) return 1;
        if (damage2 > damage1) return 2;
        return 0; // Unentschieden
    }

    private double GetElementalMultiplier(ElementType attacker, ElementType defender)
    {
        return (attacker, defender) switch
        {
            (ElementType.Water, ElementType.Fire) => 2.0, // Wasser ist effektiv gegen Feuer
            (ElementType.Fire, ElementType.Normal) => 2.0, // Feuer ist effektiv gegen Normal
            (ElementType.Normal, ElementType.Water) => 2.0, // Normal ist effektiv gegen Wasser
            (ElementType.Fire, ElementType.Water) => 0.5, // Feuer ist schwach gegen Wasser
            (ElementType.Normal, ElementType.Fire) => 0.5, // Normal ist schwach gegen Feuer
            (ElementType.Water, ElementType.Normal) => 0.5, // Wasser ist schwach gegen Normal
            _ => 1.0 // Keine spezielle Effektivität
        };
    }
}