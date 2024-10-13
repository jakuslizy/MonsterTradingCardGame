using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server
{
    public class Router
    {
        private readonly UserHandler _userHandler;
        private readonly UserService _userService;

        public Router(UserService userService, CardService cardService, BattleService battleService)
        {
            _userHandler = new UserHandler(userService);
            _userService = userService;
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
            if (!headers.TryGetValue("Authorization", out var token))
            {
                return new Response(401, "Unauthorized: No token provided", "application/json");
            }

            if (!_userService.ValidateToken(token))
            {
                return new Response(401, "Unauthorized: Invalid token", "application/json");
            }

            var username = _userService.GetUsernameFromToken(token);
            return new Response(404, "Not Found", "text/plain");
        }
    }
}