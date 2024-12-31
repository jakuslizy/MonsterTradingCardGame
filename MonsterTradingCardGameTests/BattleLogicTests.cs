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
        // Arrange
        var goblin = new Goblin("1", "WaterGoblin", 50, ElementType.Water);
        var dragon = new Dragon("2", "Dragon", 30, ElementType.Normal);

        // Act & Assert
        try 
        {
            var result = _battleLogic.DetermineRoundWinner(goblin, dragon);
            Assert.That(result, Is.EqualTo(2), "Dragon should win against Goblin regardless of damage");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_WizzardVsOrk_WizzardWinsAutomatically()
    {
        // Arrange
        var wizzard = new Wizzard("1", "Wizzard", 20, ElementType.Normal);
        var ork = new Ork("2", "Ork", 50, ElementType.Normal);

        // Act & Assert
        try 
        {
            var result = _battleLogic.DetermineRoundWinner(wizzard, ork);
            Assert.That(result, Is.EqualTo(1), "Wizzard should win against Ork regardless of damage");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_WaterSpellVsKnight_SpellWinsAutomatically()
    {
        // Arrange
        var waterSpell = new SpellCard("1", "WaterSpell", 10, ElementType.Water);
        var knight = new Knight("2", "Knight", 50, ElementType.Normal);

        // Act & Assert
        try 
        {
            var result = _battleLogic.DetermineRoundWinner(waterSpell, knight);
            Assert.That(result, Is.EqualTo(1), "WaterSpell should instantly defeat Knight");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_SpellVsKraken_KrakenWinsAutomatically()
    {
        // Arrange
        var spell = new SpellCard("1", "FireSpell", 50, ElementType.Fire);
        var kraken = new Kraken("2", "Kraken", 30, ElementType.Water);

        // Act & Assert
        try 
        {
            var result = _battleLogic.DetermineRoundWinner(spell, kraken);
            Assert.That(result, Is.EqualTo(2), "Kraken should win against any spell");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_DragonVsFireElf_FireElfWins()
    {
        // Arrange
        var dragon = new Dragon("1", "Dragon", 50, ElementType.Fire);
        var fireElf = new FireElf("2", "FireElf", 25, ElementType.Fire);

        // Act & Assert
        try 
        {
            var result = _battleLogic.DetermineRoundWinner(fireElf, dragon);
            Assert.That(result, Is.EqualTo(1), "FireElf evades Dragon's attack and deals 25 damage to win");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_WaterSpellVsFireSpell_WaterSpellDealsDoubleDamage()
    {
        // Arrange
        var waterSpell = new SpellCard("1", "WaterSpell", 20, ElementType.Water);
        var fireSpell = new SpellCard("2", "FireSpell", 30, ElementType.Fire);

        // Act & Assert
        try 
        {
            var result = _battleLogic.DetermineRoundWinner(waterSpell, fireSpell);
            Assert.That(result, Is.EqualTo(1), "Water spell should win due to double damage against fire");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_MonsterVsMonster_HigherDamageWins()
    {
        // Arrange
        var strongMonster = new Goblin("1", "StrongGoblin", 50, ElementType.Normal);
        var weakMonster = new Goblin("2", "WeakGoblin", 30, ElementType.Normal);

        // Act & Assert
        try 
        {
            var result = _battleLogic.DetermineRoundWinner(strongMonster, weakMonster);
            Assert.That(result, Is.EqualTo(1), "Monster with higher damage should win in pure monster fights");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }

    [Test]
    public void DetermineRoundWinner_EqualDamage_ResultsInDraw()
    {
        // Arrange
        var monster1 = new Goblin("1", "Goblin", 30, ElementType.Normal);
        var monster2 = new Goblin("2", "Troll", 30, ElementType.Normal);

        // Act & Assert
        try 
        {
            var result = _battleLogic.DetermineRoundWinner(monster1, monster2);
            Assert.That(result, Is.EqualTo(0), "Equal damage should result in a draw");
        }
        catch (Exception ex)
        {
            Assert.Fail($"Test failed with exception: {ex.Message}\nStack trace: {ex.StackTrace}");
        }
    }
}