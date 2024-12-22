using System.Data;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Domain.Models.MonsterCards;

namespace MonsterTradingCardGame.Data.Repositories
{
    public class PackageRepository
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

        public Package? GetPackage()
        {
            var package = new Package();
            var hasCards = false;

            using var command = _dal.CreateCommand(@"
                WITH first_available_package AS (
                    SELECT id 
                    FROM packages 
                    WHERE user_id IS NULL 
                    LIMIT 1
                )
                SELECT c.id, c.name, c.damage, c.element_type
                FROM cards c
                JOIN first_available_package p ON c.package_id = p.id");

            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                hasCards = true;
                var cardId = reader.GetString(reader.GetOrdinal("id"));
                var name = reader.GetString(reader.GetOrdinal("name"));
                var damage = reader.GetInt32(reader.GetOrdinal("damage"));
                var elementType = Enum.Parse<ElementType>(
                    reader.GetString(reader.GetOrdinal("element_type")));

                var card = CreateCard(cardId, name, damage, elementType);
                package.AddCard(card);
            }

            return hasCards ? package : null;
        }

        private Card CreateCard(string id, string name, int damage, ElementType elementType)
        {
            // Spell-Karten
            if (name.Contains("Spell"))
            {
                return new SpellCard(id, name, damage, elementType);
            }

            // Monster-Karten
            return name switch
            {
                var n when n.Contains("Dragon") => new Dragon(id, name, damage, elementType),
                var n when n.Contains("FireElf") => new FireElf(id, name, damage, elementType),
                var n when n.Contains("Kraken") => new Kraken(id, name, damage, elementType),
                var n when n.Contains("Knight") => new Knight(id, name, damage, elementType),
                var n when n.Contains("Wizard") => new Wizzard(id, name, damage, elementType),
                var n when n.Contains("Ork") => new Ork(id, name, damage, elementType),
                var n when n.Contains("Goblin") => new Goblin(id, name, damage, elementType),
                _ => new Dragon(id, name, damage, elementType)
            };
        }
    }
}