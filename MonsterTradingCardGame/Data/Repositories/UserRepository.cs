using System.Data;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Business.Services;
using NpgsqlTypes;
using Npgsql;

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
            INSERT INTO users (username, password_hash, coins, created_at, display_name, bio, image)
            VALUES (@username, @password_hash, @coins, @created_at, @display_name, @bio, @image)
            RETURNING id");
        DataLayer.AddParameterWithValue(command, "@username", DbType.String, user.Username);
        DataLayer.AddParameterWithValue(command, "@password_hash", DbType.String, user.PasswordHash);
        DataLayer.AddParameterWithValue(command, "@coins", DbType.Int32, user.Coins);
        DataLayer.AddParameterWithValue(command, "@created_at", DbType.DateTime, user.CreatedAt);
        DataLayer.AddParameterWithValue(command, "@display_name", DbType.String, user.Name);
        DataLayer.AddParameterWithValue(command, "@bio", DbType.String, user.Bio);
        DataLayer.AddParameterWithValue(command, "@image", DbType.String, user.Image);
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
                coins: reader.GetInt32(reader.GetOrdinal("coins"))
            )
            {
                Name = reader.IsDBNull(reader.GetOrdinal("display_name")) ? null : reader.GetString(reader.GetOrdinal("display_name")),
                Bio = reader.IsDBNull(reader.GetOrdinal("bio")) ? null : reader.GetString(reader.GetOrdinal("bio")),
                Image = reader.IsDBNull(reader.GetOrdinal("image")) ? null : reader.GetString(reader.GetOrdinal("image"))
            };
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
                coins: reader.GetInt32(reader.GetOrdinal("coins"))
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
        @"SELECT c.id, c.name, c.damage, c.element_type, c.in_deck 
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
        var inDeck = reader.GetBoolean(reader.GetOrdinal("in_deck"));
        
        ElementType elementType;
        if (Enum.TryParse<ElementType>(elementTypeString, true, out elementType))
        {
            var card = _cardService.CreateCard(id, name, damage, elementType);
            card.InDeck = inDeck;
            cards.Add(card);
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
    // Zuerst alle Karten des Users auf in_deck = false setzen
    using var resetCommand = _dal.CreateCommand(@"
        UPDATE cards 
        SET in_deck = false
        WHERE user_id = @userId");
    DataLayer.AddParameterWithValue(resetCommand, "@userId", DbType.Int32, userId);
    resetCommand.ExecuteNonQuery();
    
    if (cardIds.Count > 0)
    {
        // Dann die ausgewählten Karten auf in_deck = true setzen
        using var updateCommand = _dal.CreateCommand(@"
            UPDATE cards 
            SET in_deck = true
            WHERE user_id = @userId AND id = ANY(@cardIds)");
        
        updateCommand.Parameters.Clear();
        DataLayer.AddParameterWithValue(updateCommand, "@userId", DbType.Int32, userId);
        var parameter = updateCommand.CreateParameter();
        parameter.ParameterName = "@cardIds";
        parameter.Value = cardIds.ToArray();
        ((NpgsqlParameter)parameter).NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Text;
        updateCommand.Parameters.Add(parameter);
        
        updateCommand.ExecuteNonQuery();
    }
}

public List<Card> GetUserDeck(int userId)
{
    var cards = new List<Card>();
    using var command = _dal.CreateCommand(@"
        SELECT c.id, c.name, c.damage, c.element_type 
        FROM cards c
        WHERE c.user_id = @userId AND c.in_deck = true");
    
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
            card.InDeck = true;
            cards.Add(card);
        }
    }
    return cards;
}

public void UpdateUserData(User user)
{
    using var command = _dal.CreateCommand(@"
        UPDATE users 
        SET display_name = @display_name, bio = @bio, image = @image
        WHERE username = @username");
        
    DataLayer.AddParameterWithValue(command, "@display_name", DbType.String, user.Name);
    DataLayer.AddParameterWithValue(command, "@bio", DbType.String, user.Bio);
    DataLayer.AddParameterWithValue(command, "@image", DbType.String, user.Image);
    DataLayer.AddParameterWithValue(command, "@username", DbType.String, user.Username);
    
    command.ExecuteNonQuery();
}

    // Todo: weitere Methoden wie UpdateUser, DeleteUser usw.
}