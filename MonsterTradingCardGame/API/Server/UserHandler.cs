using System.Text.Json;
using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server;

public class UserHandler(UserService userService)
{
    public Response RegisterUser(Request request)
    {
        if (string.IsNullOrEmpty(request.Body))
        {
            return new Response(400, "Invalid request: Empty body", "application/json");
        }

        var userData = JsonSerializer.Deserialize<User>(request.Body);
        if (userData != null)
        {
            userService.RegisterUser(userData);
            return new Response(201, "User successfully created", "application/json");
        }

        return new Response(400, "Invalid user data", "application/json");
    }

    public Response LoginUser(Request request)
    {
        if (string.IsNullOrEmpty(request.Body))
        {
            return new Response(400, "Bad request: No data in request body", "text/plain");
        }

        var loginData = JsonSerializer.Deserialize<LoginData>(request.Body);
        if (loginData == null)
        {
            return new Response(400, "Invalid login credentials", "text/plain");
        }

        var token = userService.LoginUser(loginData.Username, loginData.Password);
        return new Response(200, JsonSerializer.Serialize(new { Token = token }), "application/json");
    }
}