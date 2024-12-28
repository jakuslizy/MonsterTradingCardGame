using System.Text.Json;
using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.Business.Services;

namespace MonsterTradingCardGame.API.Server;

public class UserHandler(IUserService userService)
{
    public Response RegisterUser(Request request)
    {
        if (string.IsNullOrEmpty(request.Body))
        {
            return new Response(400, "Invalid request: Empty body", "application/json");
        }

        try
        {
            var registrationData = JsonSerializer.Deserialize<LoginData>(request.Body);
            if (registrationData == null)
            {
                return new Response(400, "Invalid registration data", "application/json");
            }

            if (string.IsNullOrEmpty(request.Body))
            {
                return new Response(400, "Invalid request: Empty body", "application/json");
            }

            if (string.IsNullOrWhiteSpace(registrationData.Username) || registrationData.Username.Length < 3)
            {
                return new Response(400, "Username must be at least 3 characters long", "application/json");
            }

            if (string.IsNullOrWhiteSpace(registrationData.Password) || registrationData.Password.Length < 6)
            {
                return new Response(400, "Password must be at least 6 characters long", "application/json");
            }

            var newUser = userService.RegisterUser(registrationData.Username, registrationData.Password);
            return new Response(201,
                JsonSerializer.Serialize(new { Message = "User successfully created", UserId = newUser.Id }),
                "application/json");
        }
        catch (InvalidOperationException ex)
        {
            return new Response(409, JsonSerializer.Serialize(new { ex.Message }), "application/json");
        }
        catch (Exception)
        {
            return new Response(500, "An error occurred while processing your request", "application/json");
        }
    }

    public Response LoginUser(Request request)
    {
        //Console.WriteLine($"Received login request with body: {request.Body}");
        if (string.IsNullOrEmpty(request.Body))
        {
            return new Response(400, "Invalid request: Empty body", "application/json");
        }

        try
        {
            var loginData = JsonSerializer.Deserialize<LoginData>(request.Body);
            if (loginData == null)
            {
                return new Response(400, "Invalid login data", "application/json");
            }

            var token = userService.LoginUser(loginData.Username, loginData.Password);
            return new Response(200, token, "text/plain");
        }
        catch (JsonException)
        {
            return new Response(400, "Invalid JSON format", "application/json");
        }
        catch (InvalidOperationException ex)
        {
            return new Response(401, ex.Message, "text/plain");
        }
    }
}