using MonsterTradingCardGame.Data;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services;

public class UserService
{
    public User RegisterUser(string username, string password)
    {
        if (InMemoryDatabase.GetUser(username) != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        string hashedPassword = HashPassword(password);
        var newUser = new User(username, hashedPassword)
        {
            Id = Guid.NewGuid().ToString(),
            CreatedAt = DateTime.UtcNow
        };

        InMemoryDatabase.AddUser(newUser);
        return newUser;
    }

    public string LoginUser(string username, string password)
    {
        var user = InMemoryDatabase.GetUser(username);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid username or password");
        }

        var token = Guid.NewGuid().ToString();
        InMemoryDatabase.AddToken(token, username);
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