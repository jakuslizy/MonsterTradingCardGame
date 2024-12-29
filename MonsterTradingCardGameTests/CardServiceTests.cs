using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Data.Repositories.Interfaces;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Domain.Models.MonsterCards;
using NSubstitute;

namespace MonsterTradingCardGameTests;

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
        // Arrange
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

        // Act
        _cardService.ConfigureDeck(_testUser, selectedCardIds);

        // Assert
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
        // Arrange
        var allUserCards = new List<Card>
        {
            new Dragon("card1", "Dragon", 10, ElementType.Fire) { UserId = 1, InDeck = true },
            new Goblin("card2", "Goblin", 20, ElementType.Water) { UserId = 1, InDeck = true },
            new Knight("card3", "Knight", 30, ElementType.Normal) { UserId = 1, InDeck = true },
            new SpellCard("card4", "FireSpell", 40, ElementType.Fire) { UserId = 1, InDeck = true },
            new SpellCard("card5", "WaterSpell", 50, ElementType.Water) { UserId = 1, InDeck = false }
        };

        _userRepository.GetUserDeck(_testUser.Id).Returns(allUserCards.Where(c => c.InDeck).ToList());

        // Act
        var deckCards = _cardService.GetUserDeck(_testUser);

        // Assert
        Assert.That(deckCards.Count, Is.EqualTo(4));
        Assert.That(deckCards.All(c => c.InDeck), Is.True);
    }

    [Test]
    public void ConfigureDeck_WithCardsNotInStack_ShouldThrowException()
    {
        // Arrange
        var userCards = new List<Card>
        {
            new Dragon("card1", "Dragon", 10, ElementType.Fire) { UserId = 1, InDeck = false }
        };

        var invalidCardIds = new List<string> { "card1", "card2", "card3", "card4" };

        _userRepository.GetUserCards(1).Returns(userCards);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(
            () => _cardService.ConfigureDeck(_testUser, invalidCardIds)
        );
        Assert.That(exception.Message, Does.Contain("nicht im Stack des Users gefunden"));
    }
}