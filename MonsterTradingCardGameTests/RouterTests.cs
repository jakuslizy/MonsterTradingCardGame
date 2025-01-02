using MonsterTradingCardGame.API.Server;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data.Repositories.Interfaces;
using NSubstitute;

namespace MonsterTradingCardGameTests;

[TestFixture]
public class RouterTests
{
    private Router _router;
    private IUserService _userService;
    private ICardService _cardService;
    private IBattleService _battleService;
    private IUserRepository _userRepository;
    private IPackageService _packageService;
    private ITradingService _tradingService;
    private BattleQueue _battleQueue;
    private IStatsRepository _statsRepository;
    private IPackageRepository _packageRepository;

    [SetUp]
    public void Setup()
    {
        _userRepository = Substitute.For<IUserRepository>();

        _userService = Substitute.For<IUserService>();
        _cardService = Substitute.For<ICardService>();
        _battleService = Substitute.For<IBattleService>();
        _packageService = Substitute.For<IPackageService>();

        _packageRepository = Substitute.For<IPackageRepository>();
        _statsRepository = Substitute.For<IStatsRepository>();
        _battleQueue = Substitute.For<BattleQueue>();
        _tradingService = Substitute.For<ITradingService>();

        _router = new Router(
            _userService,
            _cardService,
            _battleService,
            _packageService,
            _packageRepository,
            _userRepository,
            _statsRepository,
            _battleQueue,
            _tradingService
        );
    }

    [Test]
    public void TestHomeRoute()
    {
        var response = _router.RouteRequest("GET / HTTP/1.1", new Dictionary<string, string>(), "");
        TestContext.WriteLine($"Response Status Code: {response.StatusCode}");

        Assert.IsNotNull(response, "The response should not be null.");
        Assert.That(response.StatusCode, Is.EqualTo(200), "The Home route should return status code 200.");
        TestContext.WriteLine("Home-Route Test erfolgreich abgeschlossen");
    }

    [Test]
    public void TestUsersRoute()
    {
        var response = _router.RouteRequest("POST /users HTTP/1.1", new Dictionary<string, string>(), "");
        TestContext.WriteLine($"Response Status Code: {response.StatusCode}");

        Assert.IsNotNull(response, "The response should not be null");
        Assert.That(response.StatusCode, Is.EqualTo(400),
            "The Users route should return status code 400 for empty body");
        TestContext.WriteLine("Users-Route Test erfolgreich abgeschlossen");
    }

    [Test]
    public void TestInvalidRoute()
    {
        var response = _router.RouteRequest("GET /invalidpath HTTP/1.1", new Dictionary<string, string>(), "");
        TestContext.WriteLine($"Response Status Code: {response.StatusCode}");

        Assert.That(response.StatusCode, Is.EqualTo(401), "Invalid route should return status code 401");
        TestContext.WriteLine("Ungültige Route Test erfolgreich abgeschlossen");
    }

    [Test]
    public void TestNullRequest()
    {
        var response = _router.RouteRequest(null, new Dictionary<string, string>(), "");
        TestContext.WriteLine($"Response Status Code: {response.StatusCode}");

        Assert.That(response.StatusCode, Is.EqualTo(400), "Null request should return status code 400");
        TestContext.WriteLine("Null Request Test erfolgreich abgeschlossen");
    }

    [Test]
    public void TestCreatePackageUnauthorized()
    {
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer some-invalid-token" }
        };
        TestContext.WriteLine("Header mit ungültigem Token vorbereitet");

        _userService.ValidateToken(Arg.Any<string>()).Returns(false);
        TestContext.WriteLine("UserService wurde konfiguriert, Token als ungültig zu markieren");

        var response = _router.RouteRequest("POST /packages HTTP/1.1", headers, "[]");
        TestContext.WriteLine($"Response Status Code: {response.StatusCode}");

        Assert.That(response.StatusCode, Is.EqualTo(401), "Unauthorized access should return status code 401");
        TestContext.WriteLine("Unauthorized Package Creation Test erfolgreich abgeschlossen");
    }
}