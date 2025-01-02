using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.API.Server.Handlers;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data.Repositories.Interfaces;

namespace MonsterTradingCardGame.API.Server
{
    public class Router(
        IUserService userService,
        ICardService cardService,
        IBattleService battleService,
        IPackageService packageService,
        IPackageRepository packageRepository,
        IUserRepository userRepository,
        IStatsRepository statsRepository,
        BattleQueue battleQueue,
        ITradingService tradingService)
    {
        private readonly UserHandler _userHandler = new(userService);
        private readonly PackageHandler _packageHandler = new(packageService, userRepository, packageRepository);
        private readonly TradingHandler _tradingHandler = new(tradingService);
        private readonly CardHandler _cardHandler = new(cardService);
        private readonly StatsHandler _statsHandler = new(userService, statsRepository, userRepository);
        private readonly BattleHandler _battleHandler = new(battleService, battleQueue);

        public Response RouteRequest(string? requestLine, Dictionary<string, string> headers, string body)
        {
            if (string.IsNullOrEmpty(requestLine))
                return new Response(400, "Bad Request", "text/plain");

            var parts = requestLine.Split(' ');
            if (parts.Length < 2)
                return new Response(400, "Bad Request", "text/plain");

            var method = parts[0];
            var pathAndQuery = parts[1].Split('?');
            var path = pathAndQuery[0];

            // Query-Parameter extrahieren
            var queryParams = new Dictionary<string, string>();
            if (pathAndQuery.Length > 1)
            {
                var queryParts = pathAndQuery[1].Split('&');
                foreach (var param in queryParts)
                {
                    var keyValue = param.Split('=');
                    if (keyValue.Length == 2)
                    {
                        queryParams[keyValue[0]] = keyValue[1];
                    }
                }
            }

            // Nicht-geschützte Routen
            if (method == "POST" && path == "/users")
                return _userHandler.RegisterUser(new Request
                {
                    Body = body,
                    Path = "/users",
                    Method = "POST",
                    QueryParameters = queryParams
                });

            if (method == "POST" && path == "/sessions")
                return _userHandler.LoginUser(new Request { Body = body, Path = "/sessions", Method = "POST" });

            if (method == "GET" && path == "/")
            {
                // Bild in Base64 konvertieren
                string imagePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Assets", "0_0.png");
                string imageBase64 = Convert.ToBase64String(File.ReadAllBytes(imagePath));
                
                return new Response(200, $@"
                    <html>
                    <body style='text-align: center; font-family: Arial; background-color: #f0f0f0;'>
                        <h1 style='color: #333;'>Willkommen beim Monster Trading Card Game</h1>
                        <img src='data:image/png;base64,{imageBase64}' 
                             alt='MTCG Logo' 
                             style='max-width: 500px; border-radius: 10px; box-shadow: 0 4px 8px rgba(0,0,0,0.1);'>
                    </body>
                    </html>",
                    "text/html");
            }

            // Alle anderen Routen sind geschützt
            return HandleProtectedRoute(method, path, headers, body, queryParams);
        }

        private Response HandleProtectedRoute(string method, string path, Dictionary<string, string> headers,
            string body, Dictionary<string, string> queryParams)
        {
            // Token-Validierung
            if (!headers.TryGetValue("Authorization", out var authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new Response(401, "Unauthorized: No valid token provided", "application/json");
            }

            var token = authHeader.Substring("Bearer ".Length);
            if (!userService.ValidateToken(token))
            {
                return new Response(401, "Unauthorized: Invalid or expired token", "application/json");
            }

            var user = userService.GetUserFromToken(token);

            // Router für geschützte Endpunkte
            return (method, path) switch
            {
                // Package routes
                ("POST", "/transactions/packages") => _packageHandler.HandleBuyPackage(user),
                ("POST", "/packages") => _packageHandler.HandleCreatePackage(user.Username, body),

                // Card routes
                ("GET", "/cards") => _cardHandler.HandleGetUserCards(user),
                ("GET", "/deck") => _cardHandler.HandleGetDeck(user, queryParams.GetValueOrDefault("format")),
                ("PUT", "/deck") => _cardHandler.HandleConfigureDeck(user, body),

                // User routes
                ("GET", var p) when p.StartsWith("/users/") => _userHandler.HandleGetUserData(user, p[7..]),
                ("PUT", var p) when p.StartsWith("/users/") => _userHandler.HandleUpdateUserData(user, p[7..], body),

                // Stats routes
                ("GET", "/stats") => _statsHandler.HandleGetStats(user),
                ("GET", "/scoreboard") => _statsHandler.HandleScoreboard(),

                // Battle route
                ("POST", "/battles") => _battleHandler.HandleBattle(user),

                // Trading routes
                ("GET", "/tradings") => _tradingHandler.HandleGetTradings(),
                ("POST", "/tradings") => _tradingHandler.HandleCreateTrading(user, body),
                ("POST", var p) when p.StartsWith("/tradings/") => _tradingHandler.HandleExecuteTrading(user, p[10..],
                    body),
                ("DELETE", var p) when p.StartsWith("/tradings/") => _tradingHandler.HandleDeleteTrading(user, p[10..]),

                _ => new Response(404, "Not Found", "text/plain")
            };
        }
    }
}