using MonsterTradingCardGame.Business.Logic;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Domain.Models.MonsterCards;

namespace MonsterTradingCardGameTests;

[TestFixture]
public class BattleLogicTests
{
    private BattleLogic _battleLogic;

    [SetUp]
    public void Setup()
    {
        _battleLogic = new BattleLogic();
    }

    [Test]
    public void DetermineRoundWinner_GoblinVsDragon_DragonWinsAutomatically()
    {
        try
        {
            var goblin = new Goblin("1", "WaterGoblin", 50, ElementType.Water);
            var dragon = new Dragon("2", "Dragon", 30, ElementType.Normal);

            TestContext.WriteLine("\nTest: Goblin vs Dragon");
            TestContext.WriteLine($"Goblin: {goblin.Name} (Damage: {goblin.Damage}, Element: {goblin.ElementType})");
            TestContext.WriteLine($"Dragon: {dragon.Name} (Damage: {dragon.Damage}, Element: {dragon.ElementType})");

            var result = _battleLogic.DetermineRoundWinner(goblin, dragon);

            TestContext.WriteLine($"Ergebnis: Spieler {result} gewinnt");
            Assert.That(result, Is.EqualTo(2), "Dragon should win against Goblin regardless of damage");
        }
        catch (Exception ex)
        {
            TestContext.WriteLine($"Fehler: {ex.Message}");
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_WizzardVsOrk_WizzardWinsAutomatically()
    {
        var wizzard = new Wizzard("1", "Wizzard", 20, ElementType.Normal);
        var ork = new Ork("2", "Ork", 50, ElementType.Normal);

        TestContext.WriteLine("\nTest: Wizzard vs Ork");
        TestContext.WriteLine($"Wizzard: {wizzard.Name} (Damage: {wizzard.Damage}, Element: {wizzard.ElementType})");
        TestContext.WriteLine($"Ork: {ork.Name} (Damage: {ork.Damage}, Element: {ork.ElementType})");

        try
        {
            var result = _battleLogic.DetermineRoundWinner(wizzard, ork);
            TestContext.WriteLine($"Ergebnis: Spieler {result} gewinnt");
            Assert.That(result, Is.EqualTo(1), "Wizzard should win against Ork regardless of damage");
        }
        catch (Exception ex)
        {
            TestContext.WriteLine($"Fehler: {ex.Message}");
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_WaterSpellVsKnight_SpellWinsAutomatically()
    {
        var waterSpell = new SpellCard("1", "WaterSpell", 10, ElementType.Water);
        var knight = new Knight("2", "Knight", 50, ElementType.Normal);

        TestContext.WriteLine("\nTest: WaterSpell vs Knight");
        TestContext.WriteLine(
            $"WaterSpell: {waterSpell.Name} (Damage: {waterSpell.Damage}, Element: {waterSpell.ElementType})");
        TestContext.WriteLine($"Knight: {knight.Name} (Damage: {knight.Damage}, Element: {knight.ElementType})");

        try
        {
            var result = _battleLogic.DetermineRoundWinner(waterSpell, knight);
            TestContext.WriteLine($"Ergebnis: Spieler {result} gewinnt");
            Assert.That(result, Is.EqualTo(1), "WaterSpell should instantly defeat Knight");
        }
        catch (Exception ex)
        {
            TestContext.WriteLine($"Fehler: {ex.Message}");
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_SpellVsKraken_KrakenWinsAutomatically()
    {
        var spell = new SpellCard("1", "FireSpell", 50, ElementType.Fire);
        var kraken = new Kraken("2", "Kraken", 30, ElementType.Water);

        TestContext.WriteLine("\nTest: Spell vs Kraken");
        TestContext.WriteLine($"Spell: {spell.Name} (Damage: {spell.Damage}, Element: {spell.ElementType})");
        TestContext.WriteLine($"Kraken: {kraken.Name} (Damage: {kraken.Damage}, Element: {kraken.ElementType})");

        try
        {
            var result = _battleLogic.DetermineRoundWinner(spell, kraken);
            TestContext.WriteLine($"Ergebnis: Spieler {result} gewinnt");
            Assert.That(result, Is.EqualTo(2), "Kraken should win against any spell");
        }
        catch (Exception ex)
        {
            TestContext.WriteLine($"Fehler: {ex.Message}");
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_DragonVsFireElf_FireElfWins()
    {
        var dragon = new Dragon("1", "Dragon", 50, ElementType.Fire);
        var fireElf = new FireElf("2", "FireElf", 25, ElementType.Fire);

        TestContext.WriteLine("\nTest: Dragon vs FireElf");
        TestContext.WriteLine($"Dragon: {dragon.Name} (Damage: {dragon.Damage}, Element: {dragon.ElementType})");
        TestContext.WriteLine($"FireElf: {fireElf.Name} (Damage: {fireElf.Damage}, Element: {fireElf.ElementType})");

        try
        {
            var result = _battleLogic.DetermineRoundWinner(fireElf, dragon);
            TestContext.WriteLine($"Ergebnis: Spieler {result} gewinnt");
            Assert.That(result, Is.EqualTo(1), "FireElf evades Dragon's attack and deals 25 damage to win");
        }
        catch (Exception ex)
        {
            TestContext.WriteLine($"Fehler: {ex.Message}");
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_WaterSpellVsFireSpell_WaterSpellDealsDoubleDamage()
    {
        var waterSpell = new SpellCard("1", "WaterSpell", 20, ElementType.Water);
        var fireSpell = new SpellCard("2", "FireSpell", 30, ElementType.Fire);

        TestContext.WriteLine("\nTest: WaterSpell vs FireSpell");
        TestContext.WriteLine(
            $"WaterSpell: {waterSpell.Name} (Damage: {waterSpell.Damage}, Element: {waterSpell.ElementType})");
        TestContext.WriteLine(
            $"FireSpell: {fireSpell.Name} (Damage: {fireSpell.Damage}, Element: {fireSpell.ElementType})");

        try
        {
            var result = _battleLogic.DetermineRoundWinner(waterSpell, fireSpell);
            TestContext.WriteLine($"Ergebnis: Spieler {result} gewinnt");
            Assert.That(result, Is.EqualTo(1), "Water spell should win due to double damage against fire");
        }
        catch (Exception ex)
        {
            TestContext.WriteLine($"Fehler: {ex.Message}");
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_MonsterVsMonster_HigherDamageWins()
    {
        var strongMonster = new Goblin("1", "StrongGoblin", 50, ElementType.Normal);
        var weakMonster = new Goblin("2", "WeakGoblin", 30, ElementType.Normal);

        TestContext.WriteLine("\nTest: StrongGoblin vs WeakGoblin");
        TestContext.WriteLine(
            $"StrongGoblin: {strongMonster.Name} (Damage: {strongMonster.Damage}, Element: {strongMonster.ElementType})");
        TestContext.WriteLine(
            $"WeakGoblin: {weakMonster.Name} (Damage: {weakMonster.Damage}, Element: {weakMonster.ElementType})");

        // Mehrere Durchläufe durchführen
        var results = new Dictionary<int, int>();
        const int iterations = 100;

        for (int i = 0; i < iterations; i++)
        {
            var result = _battleLogic.DetermineRoundWinner(strongMonster, weakMonster);
            results.TryAdd(result, 0);
            results[result]++;
        }

        TestContext.WriteLine("\nErgebnisse nach " + iterations + " Durchläufen:");
        foreach (var kvp in results)
        {
            TestContext.WriteLine($"Spieler {kvp.Key}: {kvp.Value} Siege ({kvp.Value * 100.0 / iterations:F1}%)");
        }

        // Das stärkere Monster sollte häufiger gewinnen
        Assert.That(results.GetValueOrDefault(1, 0), Is.GreaterThan(results.GetValueOrDefault(2, 0)),
            "Stärkeres Monster sollte häufiger gewinnen");
    }

    [Test]
    public void DetermineRoundWinner_EqualDamage_ResultsInDraw()
    {
        var monster1 = new Goblin("1", "Goblin", 30, ElementType.Normal);
        var monster2 = new Goblin("2", "Troll", 30, ElementType.Normal);

        TestContext.WriteLine("\nTest: Equal Damage Battle");
        TestContext.WriteLine($"Goblin: {monster1.Name} (Damage: {monster1.Damage}, Element: {monster1.ElementType})");
        TestContext.WriteLine($"Troll: {monster2.Name} (Damage: {monster2.Damage}, Element: {monster2.ElementType})");

        try
        {
            var result = _battleLogic.DetermineRoundWinner(monster1, monster2);
            TestContext.WriteLine($"Ergebnis: Unentschieden (Spieler {result})");
            Assert.That(result, Is.EqualTo(0), "Equal damage should result in a draw");
        }
        catch (Exception ex)
        {
            TestContext.WriteLine($"Fehler: {ex.Message}");
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_ElementalEffectiveness_FireVsWater()
    {
        var fireSpell = new SpellCard("1", "FireSpell", 40, ElementType.Fire);
        var waterSpell = new SpellCard("2", "WaterSpell", 20, ElementType.Water);

        TestContext.WriteLine("\nTest: Fire vs Water Effectiveness");
        TestContext.WriteLine(
            $"FireSpell: {fireSpell.Name} (Damage: {fireSpell.Damage}, Element: {fireSpell.ElementType})");
        TestContext.WriteLine(
            $"WaterSpell: {waterSpell.Name} (Damage: {waterSpell.Damage}, Element: {waterSpell.ElementType})");

        var result = _battleLogic.DetermineRoundWinner(fireSpell, waterSpell);

        TestContext.WriteLine($"Ergebnis: Spieler {result} gewinnt");
        Assert.That(result, Is.EqualTo(2), "Water should win against Fire due to elemental effectiveness");
    }

    [Test]
    public void DetermineRoundWinner_EqualDamageWithElementalBonus_WaterWinsAgainstFire()
    {
        var fireSpell = new SpellCard("1", "FireSpell", 20, ElementType.Fire);
        var waterSpell = new SpellCard("2", "WaterSpell", 20, ElementType.Water);

        TestContext.WriteLine("\nTest: Equal Damage with Elemental Bonus");
        TestContext.WriteLine(
            $"FireSpell: {fireSpell.Name} (Damage: {fireSpell.Damage}, Element: {fireSpell.ElementType})");
        TestContext.WriteLine(
            $"WaterSpell: {waterSpell.Name} (Damage: {waterSpell.Damage}, Element: {waterSpell.ElementType})");

        var result = _battleLogic.DetermineRoundWinner(waterSpell, fireSpell);

        TestContext.WriteLine($"Ergebnis: Spieler {result} gewinnt");
        Assert.That(result, Is.EqualTo(1), "Water spell should win with equal base damage due to effectiveness");
    }

    [Test]
    public void DetermineRoundWinner_DragonVsGoblin_DragonWinsAutomatically()
    {
        var dragon = new Dragon("1", "Dragon", 10, ElementType.Normal);
        var goblin = new Goblin("2", "Goblin", 20, ElementType.Normal);

        TestContext.WriteLine("\nTest: Dragon vs Goblin");
        TestContext.WriteLine($"Dragon: {dragon.Name} (Damage: {dragon.Damage}, Element: {dragon.ElementType})");
        TestContext.WriteLine($"Goblin: {goblin.Name} (Damage: {goblin.Damage}, Element: {goblin.ElementType})");

        var result = _battleLogic.DetermineRoundWinner(dragon, goblin);

        TestContext.WriteLine($"Ergebnis: Spieler {result} gewinnt");
        Assert.That(result, Is.EqualTo(1));
    }

    [Test]
    public void CalculateDamage_CriticalHitTest()
    {
        var card1 = new Dragon("1", "TestDragon", 50, ElementType.Fire);
        var card2 = new Goblin("2", "TestGoblin", 50, ElementType.Normal);

        var results = new Dictionary<int, int>(); // Damage -> Häufigkeit
        const int iterations = 10000; // Mehr Iterationen für bessere Statistik

        // Sammle Ergebnisse
        for (int i = 0; i < iterations; i++)
        {
            var damage = _battleLogic.CalculateDamage(card1, card2);
            results.TryAdd(damage, 0);
            results[damage]++;
        }

        // Analyse der Ergebnisse
        TestContext.WriteLine("\nSchadensverteilung:");
        foreach (var kvp in results.OrderBy(x => x.Key))
        {
            double percentage = (double)kvp.Value / iterations * 100;
            TestContext.WriteLine($"Schaden {kvp.Key}: {kvp.Value} mal ({percentage:F1}%)");
        }

        int criticalDamage = results.GetValueOrDefault(100, 0);
        double criticalPercentage = (double)criticalDamage / iterations * 100;

        TestContext.WriteLine($"\nKritische Treffer: {criticalPercentage:F1}%");

        // Überprüfen, ob die Verteilung ungefähr stimmt (10 % ±2 %)
        Assert.That(criticalPercentage, Is.InRange(8, 12),
            "Kritische Treffer sollten bei etwa 10% liegen");

        // Prüfen, ob es nur die erwarteten Schadenswerte gibt
        Assert.That(results.Keys, Is.EquivalentTo(new[] { 50, 100 }),
            "Es sollte nur normale (50) und kritische (100) Treffer geben");
    }
}