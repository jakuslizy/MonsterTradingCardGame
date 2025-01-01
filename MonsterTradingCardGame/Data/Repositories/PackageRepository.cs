using System.Data;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Business.Services.Interfaces;
using MonsterTradingCardGame.Data.Repositories.Interfaces;

namespace MonsterTradingCardGame.Data.Repositories
{
    public class PackageRepository(ICardService cardService) : IPackageRepository
    {
        private readonly DataLayer _dal = DataLayer.Instance;

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
                    DataLayer.AddParameterWithValue(cardCmd, "@element_type", DbType.String,
                        card.ElementType.ToString());
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
            command.CommandText = @"
        SELECT p.id, c.id as card_id, c.name, c.damage, c.element_type
        FROM packages p
        JOIN cards c ON c.package_id = p.id
        WHERE p.purchased_by IS NULL
        LIMIT 5";

            var package = new Package();
            using var reader = command.ExecuteReader();
            bool hasRows = false;

            while (reader.Read())
            {
                hasRows = true;
                if (package.Id == 0)
                {
                    package.Id = reader.GetInt32(0);
                }

                var card = cardService.CreateCard(
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.GetInt32(3),
                    Enum.Parse<ElementType>(reader.GetString(4))
                );
                if (card != null)
                {
                    package.AddCard(card);
                }
            }

            return hasRows ? package : null;
        }

        public void UpdatePackageOwner(int packageId, int userId)
        {
            try
            {
                using var command = _dal.CreateCommand(@"
                    UPDATE packages 
                    SET purchased_by = @userId,
                        purchased_at = CURRENT_TIMESTAMP 
                    WHERE id = @packageId");

                DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, userId);
                DataLayer.AddParameterWithValue(command, "@packageId", DbType.Int32, packageId);

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"Package with ID {packageId} not found");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating package owner: {ex.Message}");
                throw;
            }
        }
    }
}