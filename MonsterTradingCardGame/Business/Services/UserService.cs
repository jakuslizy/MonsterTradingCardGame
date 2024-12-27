using MonsterTradingCardGame.Data.Repositories;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services;

public class UserService : IUserService
{
    private readonly UserRepository _userRepository;
    private readonly SessionRepository _sessionRepository;
    private readonly StatsRepository _statsRepository;

    public UserService(UserRepository userRepository, SessionRepository sessionRepository, StatsRepository statsRepository)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _statsRepository = statsRepository;
    }

    public User RegisterUser(string username, string password)
    {
        if (_userRepository.GetUserByUsername(username) != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        Console.WriteLine($"UserService - Original password: {password}");
        string hashedPassword = HashPassword(password);
        Console.WriteLine($"UserService - Hashed password: {hashedPassword}");
        var newUser = new User(username, hashedPassword);
        _userRepository.AddUser(newUser);
        var initialStats = new Stats(newUser.Id);
        _statsRepository.CreateStats(initialStats);
        return newUser;
    }

    public string LoginUser(string username, string password)
    {
        var user = _userRepository.GetUserByUsername(username);
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
        _sessionRepository.CreateSession(session);

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
        var session = _sessionRepository.GetSessionByToken(token);
        return session != null && session.ExpiresAt > DateTime.UtcNow;
    }

    public User GetUserFromToken(string token)
    {
        var session = _sessionRepository.GetSessionByToken(token);
        if (session == null || session.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException("Invalid or expired token");
        }

        var user = _userRepository.GetUserById(session.UserId);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        return user;
    }

    public User GetUserData(string username)
    {
        var user = _userRepository.GetUserByUsername(username);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }
        return user;
    }

    public void UpdateUserData(string username, string? name, string? bio, string? image)
    {
        var user = _userRepository.GetUserByUsername(username);
        if (user == null)
        {
            throw new InvalidOperationException("User not found");
        }

        user.Name = name;
        user.Bio = bio;
        user.Image = image;

        _userRepository.UpdateUserData(user);
    }
    
    public Stats GetUserStats(int userId)
    {
        var stats = _statsRepository.GetStatsByUserId(userId);
        if (stats == null)
        {
            throw new InvalidOperationException("Stats not found");
        }
        return stats;
    }
}
