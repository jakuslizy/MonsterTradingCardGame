using System.Text.Json;
using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.Business.Services;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.API.Server;

public class UserHandler(IUserService userService)
{
    private readonly IUserService _userService = userService;

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

            var newUser = _userService.RegisterUser(registrationData.Username, registrationData.Password);
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

            var token = _userService.LoginUser(loginData.Username, loginData.Password);
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

    public Response HandleGetUserData(User requestingUser, string username)
    {
        try
        {
            if (requestingUser.Username != username)
            {
                return new Response(403, "Access denied", "application/json");
            }

            var user = _userService.GetUserData(username);
            var userData = new
            {
                user.Name,
                user.Bio,
                user.Image
            };
            
            return new Response(200, 
                JsonSerializer.Serialize(userData), 
                "application/json");
        }
        catch (Exception ex)
        {
            return new Response(500, $"Error retrieving user data: {ex.Message}", "application/json");
        }
    }
    public Response HandleUpdateUserData(User requestingUser, string username, string body)
        {
            try
            {
                if (requestingUser.Username != username)
                {
                    return new Response(403, "Access denied", "application/json");
                }

                var updateData = JsonSerializer.Deserialize<Dictionary<string, string>>(body);
                if (updateData == null)
                {
                    return new Response(400, "Invalid request body", "application/json");
                }

                _userService.UpdateUserData(
                    username,
                    updateData.GetValueOrDefault("Name"),
                    updateData.GetValueOrDefault("Bio"),
                    updateData.GetValueOrDefault("Image")
                );

                return new Response(200, "User data updated successfully", "application/json");
            }
            catch (JsonException)
            {
                return new Response(400, "Invalid JSON format", "application/json");
            }
            catch (Exception ex)
            {
                return new Response(500, $"Error updating user data: {ex.Message}", "application/json");
            }
        }
}