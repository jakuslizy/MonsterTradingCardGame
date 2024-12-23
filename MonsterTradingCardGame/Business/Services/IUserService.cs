using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services;

public interface IUserService
{
    User RegisterUser(string username, string password);
    string LoginUser(string username, string password);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hashedPassword);
    bool ValidateToken(string token);
    User GetUserFromToken(string token);
    User GetUserData(string username);
    void UpdateUserData(string username, string? name, string? bio, string? image);
}