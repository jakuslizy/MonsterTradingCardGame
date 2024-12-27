using System.Data;
using MonsterTradingCardGame.Domain.Models;

namespace MonsterTradingCardGame.Data.Repositories
{
    public class TradingRepository : ITradingRepository
    {
        private readonly DataLayer _dal = DataLayer.Instance;

        public IEnumerable<Trading> GetAllTrades()
        {
            using var command = _dal.CreateCommand(@"
                SELECT id, card_to_trade, type, minimum_damage, user_id 
                FROM tradings");

            var trades = new List<Trading>();
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                trades.Add(new Trading(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    reader.GetInt32(4)
                ));
            }

            return trades;
        }

        public Trading GetTrade(string id)
        {
            using var command = _dal.CreateCommand(@"
                SELECT id, card_to_trade, type, minimum_damage, user_id 
                FROM tradings 
                WHERE id = @id");

            DataLayer.AddParameterWithValue(command, "@id", DbType.String, id);
            
            using var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Trading(
                    reader.GetString(0),
                    reader.GetString(1),
                    reader.GetString(2),
                    reader.IsDBNull(3) ? null : reader.GetInt32(3),
                    reader.GetInt32(4)
                );
            }

            return null;
        }

        public void CreateTrade(Trading trade)
        {
            using var command = _dal.CreateCommand(@"
                INSERT INTO tradings (id, card_to_trade, type, minimum_damage, user_id)
                VALUES (@id, @cardToTrade, @type, @minimumDamage, @userId)");

            DataLayer.AddParameterWithValue(command, "@id", DbType.String, trade.Id);
            DataLayer.AddParameterWithValue(command, "@cardToTrade", DbType.String, trade.CardToTrade);
            DataLayer.AddParameterWithValue(command, "@type", DbType.String, trade.Type);
            DataLayer.AddParameterWithValue(command, "@minimumDamage", DbType.Int32, 
                (object)trade.MinimumDamage ?? DBNull.Value);
            DataLayer.AddParameterWithValue(command, "@userId", DbType.Int32, trade.UserId);

            command.ExecuteNonQuery();
        }

        public void DeleteTrade(string id)
        {
            using var command = _dal.CreateCommand(@"
                DELETE FROM tradings
                WHERE id = @id");

            DataLayer.AddParameterWithValue(command, "@id", DbType.String, id);
            command.ExecuteNonQuery();
        }
    }
}