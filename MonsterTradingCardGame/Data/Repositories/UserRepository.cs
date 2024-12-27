using System.Data;
using MonsterTradingCardGame.Domain.Models;
using NpgsqlTypes;
using Npgsql;

namespace MonsterTradingCardGame.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly DataLayer _dal = DataLayer.Instance;
    private readonly ICardRepository _cardRepository;

    public UserRepository(ICardRepository cardRepository)
    {
        _cardRepository = cardRepository;
    }

    public void AddUser(User user)
    {
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO users (username, password_hash, coins, created_at, display_name, bio, image)
            VALUES (@username, @password_hash, 20, @created_at, @display_name, @bio, @image)
            RETURNING id";
            
        DataLayer.AddParameterWithValue(command, "@username", DbType.String, user.Username);
        DataLayer.AddParameterWithValue(command, "@password_hash", DbType.String, user.PasswordHash);
        DataLayer.AddParameterWithValue(command, "@created_at", DbType.DateTime, user.CreatedAt);
        DataLayer.AddParameterWithValue(command, "@display_name", DbType.String, user.Name ?? (object)DBNull.Value);
        DataLayer.AddParameterWithValue(command, "@bio", DbType.String, user.Bio ?? (object)DBNull.Value);
        DataLayer.AddParameterWithValue(command, "@image", DbType.String, user.Image ?? (object)DBNull.Value);
        
        var id = (int)(command.ExecuteScalar() ?? 0);
        user.Id = id;
    }

    public User? GetUserByUsername(string username)
    {
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM users WHERE username = @username";
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
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM users WHERE id = @id";
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
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE users 
            SET display_name = @display_name,
                bio = @bio,
                image = @image
            WHERE id = @id";
        
        DataLayer.AddParameterWithValue(command, "@display_name", DbType.String, user.Name ?? (object)DBNull.Value);
        DataLayer.AddParameterWithValue(command, "@bio", DbType.String, user.Bio ?? (object)DBNull.Value);
        DataLayer.AddParameterWithValue(command, "@image", DbType.String, user.Image ?? (object)DBNull.Value);
        DataLayer.AddParameterWithValue(command, "@id", DbType.Int32, user.Id);
        
        command.ExecuteNonQuery();
    }

    public void UpdateUserCoins(int userId, int coins)
    {
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE users 
            SET coins = @coins
            WHERE id = @id";
        
        DataLayer.AddParameterWithValue(command, "@coins", DbType.Int32, coins);
        DataLayer.AddParameterWithValue(command, "@id", DbType.Int32, userId);
        
        command.ExecuteNonQuery();
    }

    public List<Card> GetUserCards(int userId)
    {
        return _cardRepository.GetCardsByUserId(userId);
    }

    public void SaveUserCards(int userId, List<Card> cards)
    {
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE cards 
            SET user_id = @userId 
            WHERE id = @cardId";
        
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
        using var connection = _dal.CreateConnection();
        using var resetCommand = connection.CreateCommand();
        resetCommand.CommandText = @"
            UPDATE cards 
            SET in_deck = false
            WHERE user_id = @userId";
        DataLayer.AddParameterWithValue(resetCommand, "@userId", DbType.Int32, userId);
        resetCommand.ExecuteNonQuery();
        
        if (cardIds.Count > 0)
        {
            // Dann die ausgewählten Karten auf in_deck = true setzen
            using var updateCommand = connection.CreateCommand();
            updateCommand.CommandText = @"
                UPDATE cards 
                SET in_deck = true
                WHERE user_id = @userId AND id = ANY(@cardIds)";
            
            updateCommand.Parameters.Clear();
            DataLayer.AddParameterWithValue(updateCommand, "@userId", DbType.Int32, userId);
            var parameter = updateCommand.CreateParameter();
            parameter.ParameterName = "@cardIds";
            parameter.Value = cardIds.ToArray();
            ((NpgsqlParameter)parameter).NpgsqlDbType = NpgsqlDbType.Array;
            updateCommand.Parameters.Add(parameter);
            
            updateCommand.ExecuteNonQuery();
        }
    }

    public List<Card> GetUserDeck(int userId)
    {
        var cards = new List<Card>();
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT c.id, c.name, c.damage, c.element_type 
            FROM cards c
            WHERE c.user_id = @userId AND c.in_deck = true";
        
        DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, userId);
        
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var card = _cardRepository.CreateCard(
                reader.GetString(reader.GetOrdinal("id")),
                reader.GetString(reader.GetOrdinal("name")),
                reader.GetInt32(reader.GetOrdinal("damage")),
                Enum.Parse<ElementType>(reader.GetString(reader.GetOrdinal("element_type")))
            );
            
            if (card != null)
            {
                card.InDeck = true;
                cards.Add(card);
            }
        }
        return cards;
    }

    public void UpdateUserData(User user)
    {
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE users 
            SET display_name = @display_name, bio = @bio, image = @image
            WHERE username = @username";
        
        DataLayer.AddParameterWithValue(command, "@display_name", DbType.String, user.Name ?? (object)DBNull.Value);
        DataLayer.AddParameterWithValue(command, "@bio", DbType.String, user.Bio ?? (object)DBNull.Value);
        DataLayer.AddParameterWithValue(command, "@image", DbType.String, user.Image ?? (object)DBNull.Value);
        DataLayer.AddParameterWithValue(command, "@username", DbType.String, user.Username);
        
        command.ExecuteNonQuery();
    }

    public User? GetRandomOpponent(int userId)
    {
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT * FROM users 
            WHERE id != @userId 
            AND id IN (SELECT user_id FROM cards WHERE in_deck = true GROUP BY user_id HAVING COUNT(*) = 4)
            ORDER BY RANDOM() 
            LIMIT 1";
        
        DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, userId);
        
        using var reader = command.ExecuteReader();
        return reader.Read() ? MapUserFromReader(reader) : null;
    }

    private User MapUserFromReader(IDataReader reader)
    {
        return new User(
            id: reader.GetInt32(reader.GetOrdinal("id")),
            username: reader.GetString(reader.GetOrdinal("username")),
            passwordHash: reader.GetString(reader.GetOrdinal("password_hash")),
            createdAt: reader.GetDateTime(reader.GetOrdinal("created_at"))
        )
        {
            Name = reader.IsDBNull(reader.GetOrdinal("display_name")) ? null : reader.GetString(reader.GetOrdinal("display_name")),
            Bio = reader.IsDBNull(reader.GetOrdinal("bio")) ? null : reader.GetString(reader.GetOrdinal("bio")),
            Image = reader.IsDBNull(reader.GetOrdinal("image")) ? null : reader.GetString(reader.GetOrdinal("image"))
        };
    }

    public int GetUserCoins(int userId)
    {
        using var connection = _dal.CreateConnection();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT coins FROM users WHERE id = @userId";
        DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, userId);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return reader.GetInt32(0);
        }
        throw new InvalidOperationException($"User with ID {userId} not found");
    }

    // Todo: weitere Methoden wie UpdateUser, DeleteUser usw.
}