using System.Data;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Business.Services;
using Npgsql;
using NpgsqlTypes;

namespace MonsterTradingCardGame.Data.Repositories
{
    public class PackageRepository : IPackageRepository
    {
        private readonly DataLayer _dal = DataLayer.Instance;
        private readonly ICardService _cardService;

        public PackageRepository(ICardService cardService)
        {
            _cardService = cardService;
        }

        public void CreatePackage(Package package, List<Card> cards)
        {
            if (package == null) throw new ArgumentNullException(nameof(package));
            if (cards == null) throw new ArgumentNullException(nameof(cards));
            if (!cards.Any()) throw new ArgumentException("Cards list cannot be empty", nameof(cards));

            using var command = _dal.CreateCommand(@"
                INSERT INTO packages (price, created_at)
                VALUES (@price, @created_at)
                RETURNING id");

            try
            {
                // Create package
                DataLayer.AddParameterWithValue(command, "@price", DbType.Int32, Package.PackagePrice);
                DataLayer.AddParameterWithValue(command, "@created_at", DbType.DateTime, DateTime.UtcNow);
                    
                var packageId = Convert.ToInt32(command.ExecuteScalar());

                // Insert cards
                foreach (var card in cards)
                {
                    using var cardCmd = _dal.CreateCommand(@"
                        INSERT INTO cards (id, name, damage, element_type, package_id)
                        VALUES (@id, @name, @damage, @element_type, @package_id)");
                        
                    DataLayer.AddParameterWithValue(cardCmd, "@id", DbType.String, card.Id);
                    DataLayer.AddParameterWithValue(cardCmd, "@name", DbType.String, card.Name);
                    DataLayer.AddParameterWithValue(cardCmd, "@damage", DbType.Int32, card.Damage);
                    DataLayer.AddParameterWithValue(cardCmd, "@element_type", DbType.String, card.ElementType.ToString());
                    DataLayer.AddParameterWithValue(cardCmd, "@package_id", DbType.Int32, packageId);
                        
                    cardCmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating package: {ex.Message}");
                throw;
            }
        }

        // In PackageRepository.cs
public Package? GetPackage(int userId)
{
    using var connection = _dal.CreateConnection();
    using var command = connection.CreateCommand();
    using var transaction = connection.BeginTransaction(IsolationLevel.Serializable);
    
    try
    {
        var package = new Package();
        var hasCards = false;

        command.Connection = connection;
        command.Transaction = transaction;
        command.CommandText = @"
            WITH first_available_package AS (
                SELECT id 
                FROM packages 
                WHERE id IN (
                    SELECT DISTINCT package_id 
                    FROM cards 
                    WHERE user_id IS NULL
                )
                ORDER BY id
                LIMIT 1
                FOR UPDATE
            )
            SELECT c.id, c.name, c.damage, c.element_type
            FROM cards c
            JOIN first_available_package p ON c.package_id = p.id
            WHERE c.user_id IS NULL
            ORDER BY c.id";
        
        using (var reader = command.ExecuteReader())
        {
            while (reader.Read())
            {
                hasCards = true;
                var cardId = reader.GetString(reader.GetOrdinal("id"));
                var name = reader.GetString(reader.GetOrdinal("name"));
                var damage = reader.GetInt32(reader.GetOrdinal("damage"));
                var elementType = Enum.Parse<ElementType>(
                    reader.GetString(reader.GetOrdinal("element_type")));

                var card = _cardService.CreateCard(cardId, name, damage, elementType);
                if (card != null)
                {
                    package.AddCard(card);
                }
            }
        }

        if (hasCards)
        {
            command.CommandText = @"
                UPDATE cards 
                SET user_id = @userId 
                WHERE id = ANY(@cardIds)";
                
            command.Parameters.Clear();
            DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, userId);
            var parameter = command.CreateParameter();
            parameter.ParameterName = "@cardIds";
            parameter.Value = package.GetCards().Select(c => c.Id).ToArray();
            ((NpgsqlParameter)parameter).NpgsqlDbType = NpgsqlDbType.Array | NpgsqlDbType.Text;
            command.Parameters.Add(parameter);
            
            command.ExecuteNonQuery();
            transaction.Commit();
            return package;
        }

        transaction.Commit();
        return null;
    }
    catch
    {
        transaction.Rollback();
        throw;
    }
}
    }
}