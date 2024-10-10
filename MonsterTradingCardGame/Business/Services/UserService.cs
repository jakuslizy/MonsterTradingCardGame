using MonsterTradingCardGame.Data;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Business.Services;

public class UserService
{
    public void RegisterUser(User user)
    {
        if (InMemoryDatabase.GetUser(user.Username) != null)
        {
            throw new InvalidOperationException("Username already exists");
        }

        InMemoryDatabase.AddUser(user);
    }

    public string LoginUser(string username, string password)
    {
        var user = InMemoryDatabase.GetUser(username);
        if (user == null || user.Password != password)
        {
            throw new InvalidOperationException("Invalid username or password");
        }

        var token = Guid.NewGuid().ToString();
        InMemoryDatabase.AddToken(token, username);
        return token;
    }
}