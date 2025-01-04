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
        // Test 1: Goblin vs Dragon
        var goblin = new Goblin("1", "WaterGoblin", 50, ElementType.Water);
        var dragon = new Dragon("2", "Dragon", 30, ElementType.Normal);

        TestContext.WriteLine("\nTest 1: Goblin vs Dragon");
        TestContext.WriteLine($"Goblin: {goblin.Name} (Damage: {goblin.Damage}, Element: {goblin.ElementType})");
        TestContext.WriteLine($"Dragon: {dragon.Name} (Damage: {dragon.Damage}, Element: {dragon.ElementType})");

        var result1 = _battleLogic.DetermineRoundWinner(goblin, dragon);
        TestContext.WriteLine($"Ergebnis: Spieler {result1} gewinnt");
        Assert.That(result1, Is.EqualTo(2), "Dragon should win against Goblin when Goblin attacks");

        // Test 2: Dragon vs Goblin
        TestContext.WriteLine("\nTest 2: Dragon vs Goblin");
        var result2 = _battleLogic.DetermineRoundWinner(dragon, goblin);
        TestContext.WriteLine($"Ergebnis: Spieler {result2} gewinnt");
        Assert.That(result2, Is.EqualTo(1), "Dragon should win against Goblin when Dragon attacks");
    }

    [Test]
    public void DetermineRoundWinner_WizzardVsOrk_WizzardWinsAutomatically()
    {
        var wizzard = new Wizzard("1", "Wizzard", 20, ElementType.Normal);
        var ork = new Ork("2", "Ork", 50, ElementType.Normal);

        // Test 1: Wizzard greift Ork an
        TestContext.WriteLine("\nTest 1: Wizzard vs Ork");
        TestContext.WriteLine($"Wizzard: {wizzard.Name} (Damage: {wizzard.Damage}, Element: {wizzard.ElementType})");
        TestContext.WriteLine($"Ork: {ork.Name} (Damage: {ork.Damage}, Element: {ork.ElementType})");

        var result1 = _battleLogic.DetermineRoundWinner(wizzard, ork);
        TestContext.WriteLine($"Ergebnis: Spieler {result1} gewinnt");
        Assert.That(result1, Is.EqualTo(1), "Wizzard should win against Ork when Wizzard attacks");

        // Test 2: Ork greift Wizzard an
        TestContext.WriteLine("\nTest 2: Ork vs Wizzard");
        var result2 = _battleLogic.DetermineRoundWinner(ork, wizzard);
        TestContext.WriteLine($"Ergebnis: Spieler {result2} gewinnt");
        Assert.That(result2, Is.EqualTo(2), "Wizzard should win against Ork when Ork attacks");
    }

    [Test]
    public void DetermineRoundWinner_WaterSpellVsKnight_SpellWinsAutomatically()
    {
        var waterSpell = new SpellCard("1", "WaterSpell", 10, ElementType.Water);
        var knight = new Knight("2", "Knight", 50, ElementType.Normal);

        // Test 1: WaterSpell greift Knight an
        TestContext.WriteLine("\nTest 1: WaterSpell vs Knight");
        TestContext.WriteLine(
            $"WaterSpell: {waterSpell.Name} (Damage: {waterSpell.Damage}, Element: {waterSpell.ElementType})");
        TestContext.WriteLine($"Knight: {knight.Name} (Damage: {knight.Damage}, Element: {knight.ElementType})");

        var result1 = _battleLogic.DetermineRoundWinner(waterSpell, knight);
        TestContext.WriteLine($"Ergebnis: Spieler {result1} gewinnt");
        Assert.That(result1, Is.EqualTo(1), "WaterSpell should win when WaterSpell attacks Knight");

        // Test 2: Knight greift WaterSpell an
        TestContext.WriteLine("\nTest 2: Knight vs WaterSpell");
        var result2 = _battleLogic.DetermineRoundWinner(knight, waterSpell);
        TestContext.WriteLine($"Ergebnis: Spieler {result2} gewinnt");
        Assert.That(result2, Is.EqualTo(2), "WaterSpell should win when Knight attacks WaterSpell");
    }

    [Test]
    public void DetermineRoundWinner_SpellVsKraken_KrakenWinsAutomatically()
    {
        var spell = new SpellCard("1", "FireSpell", 50, ElementType.Fire);
        var kraken = new Kraken("2", "Kraken", 30, ElementType.Water);

        // Test 1: Spell greift Kraken an
        TestContext.WriteLine("\nTest 1: Spell vs Kraken");
        TestContext.WriteLine($"Spell: {spell.Name} (Damage: {spell.Damage}, Element: {spell.ElementType})");
        TestContext.WriteLine($"Kraken: {kraken.Name} (Damage: {kraken.Damage}, Element: {kraken.ElementType})");

        var result1 = _battleLogic.DetermineRoundWinner(spell, kraken);
        TestContext.WriteLine($"Ergebnis: Spieler {result1} gewinnt");
        Assert.That(result1, Is.EqualTo(2), "Kraken should win when spell attacks Kraken");

        // Test 2: Kraken greift Spell an
        TestContext.WriteLine("\nTest 2: Kraken vs Spell");
        var result2 = _battleLogic.DetermineRoundWinner(kraken, spell);
        TestContext.WriteLine($"Ergebnis: Spieler {result2} gewinnt");
        Assert.That(result2, Is.EqualTo(1), "Kraken should win when Kraken attacks spell");
    }

    [Test]
    public void DetermineRoundWinner_DragonVsFireElf_FireElfWins()
    {
        var dragon = new Dragon("1", "Dragon", 50, ElementType.Fire);
        var fireElf = new FireElf("2", "FireElf", 25, ElementType.Fire);

        // Test 1: FireElf greift Dragon an
        TestContext.WriteLine("\nTest 1: FireElf vs Dragon");
        TestContext.WriteLine($"FireElf: {fireElf.Name} (Damage: {fireElf.Damage}, Element: {fireElf.ElementType})");
        TestContext.WriteLine($"Dragon: {dragon.Name} (Damage: {dragon.Damage}, Element: {dragon.ElementType})");

        var result1 = _battleLogic.DetermineRoundWinner(fireElf, dragon);
        TestContext.WriteLine($"Ergebnis: Spieler {result1} gewinnt");
        Assert.That(result1, Is.EqualTo(1), "FireElf should win when FireElf attacks Dragon");

        // Test 2: Dragon greift FireElf an
        TestContext.WriteLine("\nTest 2: Dragon vs FireElf");
        var result2 = _battleLogic.DetermineRoundWinner(dragon, fireElf);
        TestContext.WriteLine($"Ergebnis: Spieler {result2} gewinnt");
        Assert.That(result2, Is.EqualTo(2), "FireElf should win when Dragon attacks FireElf");
    }

    [Test]
    public void DetermineRoundWinner_WaterSpellVsFireSpell_WaterSpellWins()
    {
        var waterSpell = new SpellCard("1", "WaterSpell", 20, ElementType.Water);
        var fireSpell = new SpellCard("2", "FireSpell", 30, ElementType.Fire);

        // Test 1: WaterSpell greift FireSpell an
        TestContext.WriteLine("\nTest 1: WaterSpell vs FireSpell");
        TestContext.WriteLine(
            $"WaterSpell: {waterSpell.Name} (Damage: {waterSpell.Damage}, Element: {waterSpell.ElementType})");
        TestContext.WriteLine(
            $"FireSpell: {fireSpell.Name} (Damage: {fireSpell.Damage}, Element: {fireSpell.ElementType})");

        var result1 = _battleLogic.DetermineRoundWinner(waterSpell, fireSpell);
        TestContext.WriteLine($"Ergebnis: Spieler {result1} gewinnt");
        Assert.That(result1, Is.EqualTo(1), "WaterSpell should win due to double damage against FireSpell");

        // Test 2: FireSpell greift WaterSpell an
        TestContext.WriteLine("\nTest 2: FireSpell vs WaterSpell");
        var result2 = _battleLogic.DetermineRoundWinner(fireSpell, waterSpell);
        TestContext.WriteLine($"Ergebnis: Spieler {result2} gewinnt");
        Assert.That(result2, Is.EqualTo(2), "WaterSpell should still win due to element advantage");
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
    public void DetermineRoundWinner_EqualDamage_WithCriticals()
    {
        var monster1 = new Goblin("1", "Goblin", 30, ElementType.Normal);
        var monster2 = new Goblin("2", "Troll", 30, ElementType.Normal);

        // Mehrere Durchläufe durchführen
        var results = new Dictionary<int, int>();
        const int iterations = 1000;

        for (int i = 0; i < iterations; i++)
        {
            var result = _battleLogic.DetermineRoundWinner(monster1, monster2);
            results.TryAdd(result, 0);
            results[result]++;
        }

        TestContext.WriteLine("\nErgebnisse nach " + iterations + " Durchläufen:");
        foreach (var kvp in results)
        {
            double percentage = kvp.Value * 100.0 / iterations;
            TestContext.WriteLine($"Ergebnis {kvp.Key}: {kvp.Value} mal ({percentage:F1}%)");
        }

        // Prüfen ob alle möglichen Ergebnisse vorkommen
        Assert.That(results.Keys, Is.SubsetOf(new[] { 0, 1, 2 }),
            "Ergebnisse sollten nur 0 (Draw), 1 (Spieler 1) oder 2 (Spieler 2) sein");
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

        // Mehrere Durchläufe für statistische Auswertung
        var results = new Dictionary<int, int>();
        const int iterations = 1000;

        for (int i = 0; i < iterations; i++)
        {
            var result = _battleLogic.DetermineRoundWinner(fireSpell, waterSpell);
            results.TryAdd(result, 0);
            results[result]++;
        }

        TestContext.WriteLine("\nErgebnisse nach " + iterations + " Durchläufen:");
        foreach (var kvp in results)
        {
            double percentage = kvp.Value * 100.0 / iterations;
            TestContext.WriteLine($"Spieler {kvp.Key}: {kvp.Value} mal ({percentage:F1}%)");
        }

        // WaterSpell sollte in der Mehrheit der Fälle gewinnen
        Assert.That(
            results.GetValueOrDefault(2, 0),
            Is.GreaterThan(results.GetValueOrDefault(1, 0)),
            "Water should win against Fire in majority of cases due to elemental effectiveness"
        );
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