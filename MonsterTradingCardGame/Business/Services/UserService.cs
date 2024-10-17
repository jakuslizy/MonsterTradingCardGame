using MonsterTradingCardGame.Data;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services;

public class UserService(UserRepository userRepository)
{
    public User RegisterUser(string username, string password)
    {
        if (userRepository.GetUser(username) != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        Console.WriteLine($"UserService - Original password: {password}");
        string hashedPassword = HashPassword(password);
        Console.WriteLine($"UserService - Hashed password: {hashedPassword}");
        var newUser = new User(username, hashedPassword);
        userRepository.AddUser(newUser);
        return newUser;
    }

    public string LoginUser(string username, string password)
    {
        var user = userRepository.GetUser(username);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid username or password");
        }

        var token = Guid.NewGuid().ToString();

        return token;
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    private bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public bool ValidateToken(string token)
    {
        return InMemoryDatabase.GetUsernameFromToken(token) != null;
    }

    public string GetUsernameFromToken(string token)
    {
        return InMemoryDatabase.GetUsernameFromToken(token) ??
               throw new UnauthorizedAccessException("Invalid token");
    }
}