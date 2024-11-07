using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Data.Repositories;

public interface IUserRepository
{
    void AddUser(User user);
    User? GetUserByUsername(string username);
    User? GetUserById(int id);
}