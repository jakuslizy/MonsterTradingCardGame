using System.Data;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data.Repositories.Interfaces;

namespace MonsterTradingCardGame.Data.Repositories
{
    public class PackageRepository(ICardService cardService) : IPackageRepository
    {
        private readonly DataLayer _dal = DataLayer.Instance;

        public Package? GetPackage(int userId)
        {
            using var connection = _dal.CreateConnection();
            using var transaction = connection.BeginTransaction();
            try
            {
                // Atomically select an available package
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"
                SELECT p.id 
                FROM packages p 
                WHERE p.purchased_by IS NULL 
                LIMIT 1 
                FOR UPDATE";

                var packageId = command.ExecuteScalar();
                if (packageId == null)
                {
                    transaction.Rollback();
                    return null;
                }

                // Get the cards for this package
                command.CommandText = @"
                SELECT c.id, c.name, c.damage, c.element_type
                FROM cards c
                WHERE c.package_id = @packageId";
                command.Parameters.Clear();
                DataLayer.AddParameterWithValue(command, "@packageId", DbType.Int32, Convert.ToInt32(packageId));

                var package = new Package { Id = Convert.ToInt32(packageId) };
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var card = cardService.CreateCard(
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.GetInt32(2),
                            Enum.Parse<ElementType>(reader.GetString(3))
                        );
                        if (card != null)
                        {
                            package.AddCard(card);
                        }
                    }
                }

                transaction.Commit();
                return package;
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
        }

        public void UpdatePackageOwner(int packageId, int userId)
        {
            using var connection = _dal.CreateConnection();
            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;

                // Update package ownership
                command.CommandText = @"
                UPDATE packages 
                SET purchased_by = @userId,
                    purchased_at = CURRENT_TIMESTAMP 
                WHERE id = @packageId";

                DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, userId);
                DataLayer.AddParameterWithValue(command, "@packageId", DbType.Int32, packageId);

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"Package with ID {packageId} not found");
                }

                // Update cards ownership in the same transaction
                command.CommandText = @"
                UPDATE cards 
                SET user_id = @userId,
                    in_stack = true 
                WHERE package_id = @packageId";

                command.ExecuteNonQuery();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void CreatePackage(Package package, List<Card> cards)
        {
            using var connection = _dal.CreateConnection();
            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;

                // Create package
                command.CommandText = @"
                INSERT INTO packages (price, created_at)
                VALUES (@price, @created_at)
                RETURNING id";

                DataLayer.AddParameterWithValue(command, "@price", DbType.Int32, Package.PackagePrice);
                DataLayer.AddParameterWithValue(command, "@created_at", DbType.DateTime, DateTime.UtcNow);

                var packageId = Convert.ToInt32(command.ExecuteScalar());

                // Insert cards
                foreach (var card in cards)
                {
                    command.CommandText = @"
                    INSERT INTO cards (id, name, damage, element_type, package_id, in_stack)
                    VALUES (@id, @name, @damage, @element_type, @package_id, false)";

                    command.Parameters.Clear();
                    DataLayer.AddParameterWithValue(command, "@id", DbType.String, card.Id);
                    DataLayer.AddParameterWithValue(command, "@name", DbType.String, card.Name);
                    DataLayer.AddParameterWithValue(command, "@damage", DbType.Int32, card.Damage);
                    DataLayer.AddParameterWithValue(command, "@element_type", DbType.String,
                        card.ElementType.ToString());
                    DataLayer.AddParameterWithValue(command, "@package_id", DbType.Int32, packageId);

                    command.ExecuteNonQuery();
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}