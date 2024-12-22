using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server
{
    public class Router
    {
        private readonly UserHandler _userHandler;
        private readonly IUserService _userService;
        private readonly IPackageService _packageService;  // Neue Zeile

        public Router(IUserService userService, ICardService cardService, IBattleService battleService, IPackageService packageService)  // Geändert
        {
            _userHandler = new UserHandler(userService);
            _userService = userService;
            _packageService = packageService;  // Neue Zeile
        }

        public Response RouteRequest(string? requestLine, Dictionary<string, string> headers, string body)
        {
            if (string.IsNullOrEmpty(requestLine))
                return new Response(400, "Bad Request", "text/plain");

            var parts = requestLine.Split(' ');
            if (parts.Length < 2)
                return new Response(400, "Bad Request", "text/plain");

            var method = parts[0];
            var path = parts[1];

            return (method, path) switch
            {
                ("POST", "/users") => _userHandler.RegisterUser(new Request
                    { Body = body, Path = "/users", Method = "POST" }),
                ("POST", "/sessions") => _userHandler.LoginUser(new Request
                    { Body = body, Path = "/sessions", Method = "POST" }),
                ("GET", "/") => new Response(200,
                    "<html><body><h1>Willkommen beim Monster Trading Card Game</h1></body></html>", "text/html"),
                _ => HandleProtectedRoute(method, path, headers, body)
            };
        }

        private Response HandleProtectedRoute(string method, string path, Dictionary<string, string> headers,
            string body)
        {
            if (!headers.TryGetValue("Authorization", out var authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return new Response(401, "Unauthorized: No valid token provided", "application/json");
            }

            var token = authHeader.Substring("Bearer ".Length);

            if (!_userService.ValidateToken(token))
            {
                return new Response(401, "Unauthorized: Invalid or expired token", "application/json");
            }

            var user = _userService.GetUserFromToken(token);

            // Hier die neue Package-Route hinzufügen
            return (method, path) switch
            {
                ("POST", "/packages") => HandleCreatePackage(user.Username, body),
                _ => new Response(404, "Not Found", "text/plain")
            };
        }

        // Neue Methode für Package-Handling
        private Response HandleCreatePackage(string username, string body)
        {
            try
            {
                _packageService.CreatePackage(body, username);
                return new Response(201, "Package created successfully", "application/json");
            }
            catch (UnauthorizedAccessException)
            {
                return new Response(403, "Forbidden - only admin can create packages", "application/json");
            }
            catch (ArgumentException ex)
            {
                return new Response(400, ex.Message, "application/json");
            }
            catch (Exception)
            {
                return new Response(500, "Internal server error", "application/json");
            }
        }
    }
}