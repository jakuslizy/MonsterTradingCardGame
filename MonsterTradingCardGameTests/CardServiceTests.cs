using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Data.Repositories.Interfaces;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Domain.Models.MonsterCards;
using NSubstitute;

namespace MonsterTradingCardGameTests;

[TestFixture]
public class CardServiceTests
{
    private IUserRepository _userRepository;
    private CardService _cardService;
    private User _testUser;

    [SetUp]
    public void Setup()
    {
        Substitute.For<ICardRepository>();
        _userRepository = Substitute.For<IUserRepository>();
        _cardService = new CardService(_userRepository);

        _testUser = new User("testUser", "hashedPassword", 1, DateTime.UtcNow, 10);
    }

    [Test]
    public void ConfigureDeck_WithValidCards_ShouldUpdateDeck()
    {
        var userCards = new List<Card>
        {
            new Dragon("card1", "Dragon", 10, ElementType.Fire) { UserId = 1, InDeck = false },
            new Goblin("card2", "Goblin", 20, ElementType.Water) { UserId = 1, InDeck = false },
            new Knight("card3", "Knight", 30, ElementType.Normal) { UserId = 1, InDeck = false },
            new SpellCard("card4", "FireSpell", 40, ElementType.Fire) { UserId = 1, InDeck = false },
            new SpellCard("card5", "WaterSpell", 50, ElementType.Water) { UserId = 1, InDeck = false }
        };

        var selectedCardIds = new List<string> { "card1", "card2", "card3", "card4" };

        _userRepository.GetUserCards(1).Returns(userCards);

        _cardService.ConfigureDeck(_testUser, selectedCardIds);

        _userRepository.Received(1).UpdateUserDeck(
            Arg.Is<int>(id => id == 1),
            Arg.Is<List<string>>(cards =>
                cards.Count == 4 &&
                cards.SequenceEqual(selectedCardIds)
            )
        );
    }

    [Test]
    public void GetUserDeck_ShouldReturnOnlyDeckCards()
    {
        var allUserCards = new List<Card>
        {
            new Dragon("card1", "Dragon", 10, ElementType.Fire) { UserId = 1, InDeck = true },
            new Goblin("card2", "Goblin", 20, ElementType.Water) { UserId = 1, InDeck = true },
            new Knight("card3", "Knight", 30, ElementType.Normal) { UserId = 1, InDeck = true },
            new SpellCard("card4", "FireSpell", 40, ElementType.Fire) { UserId = 1, InDeck = true },
            new SpellCard("card5", "WaterSpell", 50, ElementType.Water) { UserId = 1, InDeck = false }
        };
        TestContext.WriteLine(
            $"User Cards vorbereitet: {allUserCards.Count} Karten gesamt, davon {allUserCards.Count(c => c.InDeck)} im Deck");

        _userRepository.GetUserDeck(_testUser.Id).Returns(allUserCards.Where(c => c.InDeck).ToList());
        TestContext.WriteLine("Repository Mock konfiguriert");

        var deckCards = _cardService.GetUserDeck(_testUser);
        TestContext.WriteLine($"Erhaltene Deck-Karten: {deckCards.Count}");

        Assert.That(deckCards.Count, Is.EqualTo(4));
        Assert.That(deckCards.All(c => c.InDeck), Is.True);

        TestContext.WriteLine("\nKarten im Deck:");
        foreach (var card in deckCards)
        {
            TestContext.WriteLine($"- {card.Name} ({card.ElementType}, {card.Damage} Damage)");
        }
    }

    [Test]
    public void ConfigureDeck_WithCardsNotInStack_ShouldThrowException()
    {
        var userCards = new List<Card>
        {
            new Dragon("card1", "Dragon", 10, ElementType.Fire) { UserId = 1, InDeck = false }
        };

        var invalidCardIds = new List<string> { "card1", "card2", "card3", "card4" };

        _userRepository.GetUserCards(1).Returns(userCards);

        var exception = Assert.Throws<InvalidOperationException>(
            () => _cardService.ConfigureDeck(_testUser, invalidCardIds)
        );
        Assert.That(exception.Message, Does.Contain("nicht im Stack des Users gefunden"));
    }

    [Test]
    public void ConfigureDeck_WithMoreThan4Cards_ThrowsException()
    {
        var userCards = new List<Card>
        {
            new Dragon("1", "Dragon", 10, ElementType.Fire),
            new Goblin("2", "Goblin", 20, ElementType.Water),
            new Knight("3", "Knight", 30, ElementType.Normal),
            new SpellCard("4", "FireSpell", 40, ElementType.Fire),
            new SpellCard("5", "WaterSpell", 50, ElementType.Water)
        };
        TestContext.WriteLine($"User Stack vorbereitet mit {userCards.Count} Karten");

        _userRepository.GetUserCards(1).Returns(userCards);
        TestContext.WriteLine("Repository Mock konfiguriert");

        var invalidDeckSelection = new List<string> { "1", "2", "3", "4", "5" };
        TestContext.WriteLine($"Versuche Deck mit {invalidDeckSelection.Count} Karten zu konfigurieren");

        var ex = Assert.Throws<InvalidOperationException>(() =>
            _cardService.ConfigureDeck(_testUser, invalidDeckSelection));
        TestContext.WriteLine($"{ex.Message}");
    }

    [Test]
    public void GetUserDeck_WithEmptyDeck_ReturnsEmptyList()
    {
        _userRepository.GetUserDeck(_testUser.Id).Returns(new List<Card>());
        TestContext.WriteLine("Repository wurde mit leerem Deck konfiguriert");

        var result = _cardService.GetUserDeck(_testUser);
        TestContext.WriteLine($"Erhaltenes Deck hat {result.Count} Karten");

        Assert.That(result, Is.Empty);
    }
}