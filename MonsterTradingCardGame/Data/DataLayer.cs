using Npgsql;
using System.Data;

namespace MonsterTradingCardGame.Data;

public class DataLayer : IDisposable
{
    private static readonly object _lock = new object();
    private static DataLayer? _instance;
    private readonly NpgsqlDataSource _dataSource;

    public static DataLayer Instance 
    {
        get 
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    _instance ??= new DataLayer(
                        "Host=localhost;" +
                        "Database=mydb;" +
                        "Username=postgres;" +
                        "Password=postgres;" +
                        "Maximum Pool Size=200;" +
                        "Minimum Pool Size=1;" +
                        "Connection Lifetime=0;" +
                        "Connection Idle Lifetime=60;" +
                        "Timeout=30;" +
                        "Command Timeout=30");
                }
            }
            return _instance;
        }
    }

    private DataLayer(string connectionString)
    {
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
        _dataSource = dataSourceBuilder.Build();
    }

    public IDbConnection CreateConnection()
    {
        try
        {
            var connection = _dataSource.CreateConnection();
            connection.Open();
            return connection;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error creating connection: {ex.Message}");
            throw;
        }
    }

    public void Dispose()
    {
        _dataSource.Dispose();
    }

    public IDbCommand CreateCommand(string commandText)
    {
        var connection = CreateConnection();
        var command = connection.CreateCommand();
        command.CommandText = commandText;
        command.Connection = connection;
        return command;
    }

    public static void AddParameterWithValue(IDbCommand command, string parameterName, DbType dbType, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = parameterName;
        parameter.DbType = dbType;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }
}