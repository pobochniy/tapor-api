using System.Data;
using Dodo.Data.Factories;
using Microsoft.Extensions.Options;
using MySqlConnector;
using Tapor.Shared.Options;

namespace Tapor.DB.Session;

public class DbSessionFactory : IDbSessionFactory
{
    private readonly string _connectionString;

    public DbSessionFactory(IOptions<ConnectionStringOptions> config)
    {
        _connectionString = config.Value.AppConnection;
    }

    public IDbSession OpenTransaction(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted)
    {
        var connection = new MySqlConnection(_connectionString);
        connection.Open();

        var transaction = connection.BeginTransaction(isolationLevel);

        return new DbSession(connection, transaction);
    }

    public async Task<IDbSession> OpenTransactionAsync(IsolationLevel isolationLevel = IsolationLevel.ReadCommitted,
        CancellationToken ct = default)
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(ct).ConfigureAwait(false);

        var transaction = await connection.BeginTransactionAsync(isolationLevel, ct).ConfigureAwait(false);

        return new DbSession(connection, transaction);
    }

    public IDbSession Open()
    {
        var connection = new MySqlConnection(_connectionString);
        connection.Open();

        return new DbSession(connection, null);
    }

    public async Task<IDbSession> OpenAsync(CancellationToken ct = default)
    {
        var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(ct).ConfigureAwait(false);

        return new DbSession(connection, null);
    }
}