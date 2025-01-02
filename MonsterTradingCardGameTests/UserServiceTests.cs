using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Data.Repositories.Interfaces;
using MonsterTradingCardGame.Domain.Models;
using NSubstitute;

namespace MonsterTradingCardGameTests;

[TestFixture]
public class UserServiceTests
{
    private IUserRepository _userRepository;
    private IStatsRepository _statsRepository;
    private ISessionRepository _sessionRepository;
    private UserService _userService;

    [SetUp]
    public void Setup()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _statsRepository = Substitute.For<IStatsRepository>();
        _sessionRepository = Substitute.For<ISessionRepository>();
        _userService = new UserService(_userRepository, _sessionRepository, _statsRepository);
    }

    [Test]
    public void RegisterUser_UsernameTaken_ThrowsInvalidOperationException()
    {
        // Arrange
        var existingUser = new User("existingUser", "hashedPassword");
        TestContext.WriteLine($"Existierender User vorbereitet: {existingUser.Username}");

        _userRepository.GetUserByUsername("existingUser").Returns(existingUser);
        TestContext.WriteLine("Repository Mock konfiguriert für existierenden User");

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _userService.RegisterUser("existingUser", "password"));
        TestContext.WriteLine($"{ex.Message}");
    }

    [Test]
    public void LoginUser_InvalidCredentials_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new User("testUser", BCrypt.Net.BCrypt.HashPassword("correctPassword"));
        TestContext.WriteLine($"User vorbereitet: {user.Username}");

        _userRepository.GetUserByUsername("testUser").Returns(user);
        TestContext.WriteLine("Repository Mock konfiguriert");

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() =>
            _userService.LoginUser("testUser", "wrongPassword"));
        TestContext.WriteLine($"{ex.Message}");
    }

    [Test]
    public void LoginUser_ValidCredentials_ReturnsToken()
    {
        var password = "testPassword";
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new User("testUser", hashedPassword);
        TestContext.WriteLine($"User vorbereitet: {user.Username}");

        _userRepository.GetUserByUsername("testUser").Returns(user);
        TestContext.WriteLine("Repository Mock konfiguriert");

        var token = _userService.LoginUser("testUser", password);
        TestContext.WriteLine("Login durchgeführt");

        Assert.That(token, Is.Not.Null);
        Assert.That(token, Does.Contain("testUser"));
        TestContext.WriteLine($"Token erfolgreich erstellt und validiert: {token}");
    }
}