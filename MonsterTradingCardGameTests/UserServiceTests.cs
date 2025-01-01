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
        _userRepository.GetUserByUsername("existingUser").Returns(existingUser);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _userService.RegisterUser("existingUser", "password"));
    }

    [Test]
    public void LoginUser_InvalidCredentials_ThrowsInvalidOperationException()
    {
        // Arrange
        var user = new User("testUser", BCrypt.Net.BCrypt.HashPassword("correctPassword"));
        _userRepository.GetUserByUsername("testUser").Returns(user);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() =>
            _userService.LoginUser("testUser", "wrongPassword"));
    }
}