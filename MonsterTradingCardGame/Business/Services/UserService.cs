using MonsterTradingCardGame.API.Server.DTOs;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data.Repositories.Interfaces;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services;

public class UserService(
    IUserRepository userRepository,
    ISessionRepository sessionRepository,
    IStatsRepository statsRepository)
    : IUserService
{
    public User RegisterUser(string username, string password)
    {
        if (userRepository.GetUserByUsername(username) != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        Console.WriteLine($"UserService - Original password: {password}");
        string hashedPassword = HashPassword(password);
        Console.WriteLine($"UserService - Hashed password: {hashedPassword}");
        var newUser = new User(username, hashedPassword);
        userRepository.AddUser(newUser);
        var initialStats = new Stats(newUser.Id);
        statsRepository.CreateStats(initialStats);
        return newUser;
    }

    public string LoginUser(string username, string password)
    {
        var user = userRepository.GetUserByUsername(username);
        if (user == null || !VerifyPassword(password, user.PasswordHash))
        {
            throw new InvalidOperationException("Invalid username or password");
        }

        var token = $"{username}-mtcgToken";

        var session = new Session(
            token: token,
            userId: user.Id,
            createdAt: DateTime.UtcNow,
            expiresAt: DateTime.UtcNow.AddHours(1) // Session läuft nach 1 Stunde ab
        );
        sessionRepository.CreateSession(session);

        return token;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
    }

    public bool ValidateToken(string token)
    {
        var session = sessionRepository.GetSessionByToken(token);
        return session != null && session.ExpiresAt > DateTime.UtcNow;
    }

    public User GetUserFromToken(string token)
    {
        var session = sessionRepository.GetSessionByToken(token);
        if (session == null || session.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired token");
        }

        var user = userRepository.GetUserById(session.UserId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        return user;
    }

    public User GetUserData(string username)
    {
        var user = userRepository.GetUserByUsername(username);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        return user;
    }

    public void UpdateUserData(string username, string? name, string? bio, string? image)
    {
        var user = userRepository.GetUserByUsername(username);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        user.Name = name;
        user.Bio = bio;
        user.Image = image;

        userRepository.UpdateUserData(user);
    }

    public Stats GetUserStats(int userId)
    {
        var stats = statsRepository.GetStatsByUserId(userId);
        if (stats == null)
        {
            throw new InvalidOperationException("Stats not found");
        }

        return stats;
    }
}