using System.Data;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Business.Services;

namespace MonsterTradingCardGame.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataLayer _dal = DataLayer.Instance;
    private readonly ICardService _cardService;

    public UserRepository(ICardService cardService)
    {
        _cardService = cardService;
    }

    public void AddUser(User user)
    {
        using var command = _dal.CreateCommand(@"
            INSERT INTO users (username, password_hash, coins, elo, created_at)
            VALUES (@username, @password_hash, @coins, @elo, @created_at)
            RETURNING id");
        DataLayer.AddParameterWithValue(command, "@username", DbType.String, user.Username);
        DataLayer.AddParameterWithValue(command, "@password_hash", DbType.String, user.PasswordHash);
        DataLayer.AddParameterWithValue(command, "@coins", DbType.Int32, user.Coins);
        DataLayer.AddParameterWithValue(command, "@elo", DbType.Int32, user.Elo);
        DataLayer.AddParameterWithValue(command, "@created_at", DbType.DateTime, user.CreatedAt);
        var id = (int)(command.ExecuteScalar() ?? 0);
        user.Id = id;
    }

    public User? GetUserByUsername(string username)
    {
        using var command = _dal.CreateCommand("SELECT * FROM users WHERE username = @username");
        DataLayer.AddParameterWithValue(command, "@username", DbType.String, username);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User(
                id: reader.GetInt32(reader.GetOrdinal("id")),
                username: reader.GetString(reader.GetOrdinal("username")),
                passwordHash: reader.GetString(reader.GetOrdinal("password_hash")),
                createdAt: reader.GetDateTime(reader.GetOrdinal("created_at")),
                coins: reader.GetInt32(reader.GetOrdinal("coins")),
                elo: reader.GetInt32(reader.GetOrdinal("elo"))
            );
        }

        return null;
    }

    public User? GetUserById(int id)
    {
        using var command = _dal.CreateCommand("SELECT * FROM users WHERE id = @id");
        DataLayer.AddParameterWithValue(command, "@id", DbType.Int32, id);
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return new User(
                id: reader.GetInt32(reader.GetOrdinal("id")),
                username: reader.GetString(reader.GetOrdinal("username")),
                passwordHash: reader.GetString(reader.GetOrdinal("password_hash")),
                createdAt: reader.GetDateTime(reader.GetOrdinal("created_at")),
                coins: reader.GetInt32(reader.GetOrdinal("coins")),
                elo: reader.GetInt32(reader.GetOrdinal("elo"))
            );
        }

        return null;
    }
    
public void UpdateUser(User user)
{
    using var command = _dal.CreateCommand(@"
        UPDATE users 
        SET coins = @coins
        WHERE id = @id");
        
    DataLayer.AddParameterWithValue(command, "@coins", DbType.Int32, user.Coins);
    DataLayer.AddParameterWithValue(command, "@id", DbType.Int32, user.Id);
    
    command.ExecuteNonQuery();
}
public void UpdateUserCoins(int userId, int coins)
{
    using var command = _dal.CreateCommand(@"
        UPDATE users 
        SET coins = @coins
        WHERE id = @id");
        
    DataLayer.AddParameterWithValue(command, "@coins", DbType.Int32, coins);
    DataLayer.AddParameterWithValue(command, "@id", DbType.Int32, userId);
    
    command.ExecuteNonQuery();
}
public List<Card> GetUserCards(int userId)
{
    var cards = new List<Card>();
    using var command = _dal.CreateCommand(
        @"SELECT c.id, c.name, c.damage, c.element_type 
          FROM cards c
          WHERE c.user_id = @userId");
    
    DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, userId);
    
    using var reader = command.ExecuteReader();
    while (reader.Read())
    {
        var id = reader.GetString(reader.GetOrdinal("id"));
        var name = reader.GetString(reader.GetOrdinal("name"));
        var damage = reader.GetInt32(reader.GetOrdinal("damage"));
        var elementTypeString = reader.GetString(reader.GetOrdinal("element_type"));
        
        ElementType elementType;
        if (Enum.TryParse<ElementType>(elementTypeString, true, out elementType))
        {
            var card = _cardService.CreateCard(id, name, damage, elementType);
            cards.Add(card);
        }
        else
        {
            throw new InvalidOperationException($"Invalid element type: {elementTypeString}");
        }
    }
    
    return cards;
}

public void SaveUserCards(int userId, List<Card> cards)
{
    using var command = _dal.CreateCommand(
        @"UPDATE cards 
          SET user_id = @userId 
          WHERE id = @cardId");
    
    foreach (var card in cards)
    {
        command.Parameters.Clear();
        DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, userId);
        DataLayer.AddParameterWithValue(command, "@cardId", DbType.String, card.Id);
        command.ExecuteNonQuery();
    }
}

public void UpdateUserDeck(int userId, List<string> cardIds)
{
    using var command = _dal.CreateCommand(@"
        UPDATE users 
        SET deck_cards = @deckCards
        WHERE id = @id");
        
    var deckJson = System.Text.Json.JsonSerializer.Serialize(cardIds);
    DataLayer.AddParameterWithValue(command, "@deckCards", DbType.String, deckJson);
    DataLayer.AddParameterWithValue(command, "@id", DbType.Int32, userId);
    
    command.ExecuteNonQuery();
}

    // Todo: weitere Methoden wie UpdateUser, DeleteUser usw.
}