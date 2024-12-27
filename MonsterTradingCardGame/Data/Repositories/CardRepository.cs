using System.Data;
using MonsterTradingCardGame.Domain.Models;
using MonsterTradingCardGame.Business.Factories;

namespace MonsterTradingCardGame.Data.Repositories
{
    public class CardRepository : ICardRepository
    {
        private readonly DataLayer _dal;

        public CardRepository()
        {
            _dal = DataLayer.Instance;
        }

        public void AddCard(Card card, int userId)
        {
            using var connection = _dal.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO cards (id, name, damage, element_type, user_id, in_deck)
                VALUES (@id, @name, @damage, @element_type, @userId, @inDeck)";
                
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, card.Id);
            DataLayer.AddParameterWithValue(command, "@name", DbType.String, card.Name);
            DataLayer.AddParameterWithValue(command, "@damage", DbType.Int32, card.Damage);
            DataLayer.AddParameterWithValue(command, "@element_type", DbType.String, card.ElementType.ToString());
            DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, userId);
            DataLayer.AddParameterWithValue(command, "@inDeck", DbType.Boolean, card.InDeck);
                
            command.ExecuteNonQuery();
        }

        public void UpdateCardDeckStatus(string cardId, bool inDeck)
        {
            using var connection = _dal.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE cards 
                SET in_deck = @inDeck
                WHERE id = @cardId";
                
            DataLayer.AddParameterWithValue(command, "@cardId", DbType.String, cardId);
            DataLayer.AddParameterWithValue(command, "@inDeck", DbType.Boolean, inDeck);
                
            command.ExecuteNonQuery();
        }

        public void TransferCard(string cardId, int fromUserId, int toUserId)
        {
            using var connection = _dal.CreateConnection();
            using var transaction = connection.BeginTransaction();
            try
            {
                using var command = connection.CreateCommand();
                command.Transaction = transaction;
                command.CommandText = @"
                    UPDATE cards 
                    SET user_id = @toUserId,
                        in_deck = false
                    WHERE id = @cardId 
                    AND user_id = @fromUserId";

                DataLayer.AddParameterWithValue(command, "@cardId", DbType.String, cardId);
                DataLayer.AddParameterWithValue(command, "@fromUserId", DbType.Int32, fromUserId);
                DataLayer.AddParameterWithValue(command, "@toUserId", DbType.Int32, toUserId);

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    throw new InvalidOperationException($"Karte {cardId} konnte nicht übertragen werden");
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public void UpdateCard(Card card)
        {
            using var connection = _dal.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = @"
                UPDATE cards 
                SET name = @name, damage = @damage, element_type = @element_type
                WHERE id = @id";
                
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, card.Id);
            DataLayer.AddParameterWithValue(command, "@name", DbType.String, card.Name);
            DataLayer.AddParameterWithValue(command, "@damage", DbType.Int32, card.Damage);
            DataLayer.AddParameterWithValue(command, "@element_type", DbType.String, card.ElementType.ToString());
            
            command.ExecuteNonQuery();
        }

        public Card? GetCardById(string id)
        {
            using var connection = _dal.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM cards WHERE id = @id";
            DataLayer.AddParameterWithValue(command, "@id", DbType.String, id);
            
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return CreateCardFromReader(reader);
            }
            return null;
        }

        public List<Card> GetCardsByUserId(int userId)
        {
            var cards = new List<Card>();
            using var connection = _dal.CreateConnection();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM cards WHERE user_id = @userId";
            DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, userId);
            
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var card = CreateCardFromReader(reader);
                if (card != null) cards.Add(card);
            }
            return cards;
        }

        private Card? CreateCardFromReader(IDataReader reader)
        {
            var name = reader["name"].ToString() ?? "";
            var id = reader["id"].ToString() ?? "";
            var damage = Convert.ToInt32(reader["damage"]);
            var elementType = Enum.Parse<ElementType>(reader["element_type"].ToString() ?? "Normal");
            var userId = Convert.ToInt32(reader["user_id"]);
            var inDeck = Convert.ToBoolean(reader["in_deck"]);
            
            var card = CardFactory.CreateCard(id, name, damage, elementType);
            if (card != null)
            {
                card.UserId = userId;
                card.InDeck = inDeck;
            }
            return card;
        }

        public Card? CreateCard(string id, string name, int damage, ElementType elementType)
        {
            return CardFactory.CreateCard(id, name, damage, elementType);
        }
    }
}
