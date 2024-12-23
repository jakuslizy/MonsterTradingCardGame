using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Data.Repositories;

// In IUserRepository.cs
public interface IUserRepository
{
    void AddUser(User user);
    User? GetUserByUsername(string username);
    User? GetUserById(int id);
    void UpdateUser(User user);
    void UpdateUserCoins(int userId, int coins); 
    List<Card> GetUserCards(int userId);
    void SaveUserCards(int userId, List<Card> cards);
    void UpdateUserDeck(int userId, List<string> cardIds);
}