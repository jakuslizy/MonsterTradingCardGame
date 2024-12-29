using MonsterTradingCardGame.API.Server;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data.Repositories;
using NSubstitute;

namespace MonsterTradingCardGameTests;

public class Tests
{
    private Router _router;
    private IUserService _userService;
    private ICardService _cardService;
    private IBattleService _battleService;
    private IUserRepository _userRepository;
    private SessionRepository _sessionRepository;
    private IPackageService _packageService;
    private ITradingService _tradingService;
    private BattleQueue _battleQueue;
    private IStatsRepository _statsRepository;
    private IPackageRepository _packageRepository;

    [SetUp]
    public void Setup()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _sessionRepository = Substitute.For<SessionRepository>();
        
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
        Assert.IsNotNull(response, "The response should not be null.");
        Assert.That(response.StatusCode, Is.EqualTo(200), "The Home route should return status code 200.");
    }

    [Test]
    public void TestUsersRoute()
    {
        var response = _router.RouteRequest("POST /users HTTP/1.1", new Dictionary<string, string>(), "");
        Assert.IsNotNull(response, "The response should not be null");
        Assert.That(response.StatusCode, Is.EqualTo(400), "The Users route should return status code 400 (empty body)");
    }

    [Test]
    public void TestInvalidRoute()
    {
        var response = _router.RouteRequest("GET /invalidpath HTTP/1.1", new Dictionary<string, string>(), "");
        Assert.That(response.StatusCode, Is.EqualTo(401));
    }

    [Test]
    public void TestNullRequest()
    {
        var response = _router.RouteRequest(null, new Dictionary<string, string>(), "");
        Assert.That(response.StatusCode, Is.EqualTo(400));
    }
    [Test]
    public void TestCreatePackageUnauthorized()
    {
        var headers = new Dictionary<string, string>
        {
            { "Authorization", "Bearer some-invalid-token" }
        };
        
        _userService.ValidateToken(Arg.Any<string>()).Returns(false);
        
        var response = _router.RouteRequest("POST /packages HTTP/1.1", headers, "[]");
        Assert.That(response.StatusCode, Is.EqualTo(401));
    }
}
