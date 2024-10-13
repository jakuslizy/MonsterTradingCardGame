using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Data;

public static class InMemoryDatabase
{
    private static Dictionary<string, User> _users = new Dictionary<string, User>();
    private static List<Package> _packages = new List<Package>();
    private static Dictionary<string, string> _tokens = new Dictionary<string, string>();

    public static void AddUser(User user)
    {
        _users[user.Username] = user;
    }

    public static User? GetUser(string username)
    {
        return _users.GetValueOrDefault(username);
    }

    public static void AddPackage(Package package)
    {
        _packages.Add(package);
    }

    public static Package GetPackage()
    {
        if (_packages.Count > 0)
        {
            var package = _packages[0];
            _packages.RemoveAt(0);
            return package;
        }

        return null!; // Null-Forgiving-Operator
    }

    public static void AddToken(string token, string username)
    {
        _tokens[token] = username;
    }

    public static string? GetUsernameFromToken(string token)
    {
        return _tokens.GetValueOrDefault(token);
    }
}