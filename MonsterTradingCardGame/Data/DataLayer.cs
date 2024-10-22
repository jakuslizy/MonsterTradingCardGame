using Npgsql;
using System.Data;

namespace MonsterTradingCardGame.Data;

public class DataLayer : IDisposable
{
    #region Singleton-Pattern

    private static DataLayer? _instance;
    public static DataLayer Instance 
    {
        get 
        {
            if (_instance == null)
            {
                _instance = new DataLayer("Host=localhost;Database=mydb;Username=postgres;Password=postgres;Persist Security Info=True");
            }
            return _instance;
        }
    }
    #endregion

    private readonly IDbConnection _connection;

    private DataLayer(string connectionString)
    {
        _connection = new NpgsqlConnection(connectionString);
        _connection.Open();
    }

    public void Dispose()
    {
        _connection.Close();
        _connection.Dispose();
    }

    public IDbCommand CreateCommand(string commandText)
    {
        IDbCommand command = _connection.CreateCommand();
        command.CommandText = commandText;
        return command;
    }

    public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType type, object? value)
    {
        IDbDataParameter parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.DbType = type;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}