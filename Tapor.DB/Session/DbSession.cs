using System.Data;
using Dapper;
using Dodo.Data.Factories;
using MySqlConnector;

namespace Tapor.DB.Session;

public class DbSession : IDbSession
{
    public const int COMMAND_TIMEOUT = 30;
    private readonly MySqlTransaction _transaction;

    public DbSession(IDbConnection connection, MySqlTransaction transaction)
    {
        Connection = connection;
        _transaction = transaction;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        Connection.Dispose();
    }

    public IDbConnection Connection { get; }
    public IDbTransaction Transaction => _transaction;

    public void Commit()
    {
        _transaction?.Commit();
    }

    public Task CommitAsync(CancellationToken ct)
    {
        return _transaction?.CommitAsync(ct);
    }

    public void Rollback()
    {
        _transaction?.Rollback();
    }

    public Task RollbackAsync(CancellationToken ct)
    {
        return _transaction?.RollbackAsync(ct);
    }

    public Task<T> QueryFirstAsync<T>(string sql, object param = null, int commandTimeout = COMMAND_TIMEOUT,
        CancellationToken ct = default)
    {
        return Connection.QueryFirstAsync<T>(new CommandDefinition(sql, param, _transaction, commandTimeout,
            cancellationToken: ct));
    }

    public Task<T> QuerySingleAsync<T>(string sql, object param = null, int commandTimeout = COMMAND_TIMEOUT,
        CancellationToken ct = default)
    {
        return Connection.QuerySingleAsync<T>(new CommandDefinition(sql, param, _transaction, commandTimeout,
            cancellationToken: ct));
    }

    public Task<dynamic> QuerySingleAsync(string sql, object param = null, int commandTimeout = COMMAND_TIMEOUT,
        CancellationToken ct = default)
    {
        return Connection.QuerySingleAsync(new CommandDefinition(sql, param, _transaction, commandTimeout,
            cancellationToken: ct));
    }

    public Task<dynamic> QuerySingleOrDefaultAsync(string sql, object param = null, int commandTimeout = 30,
        CancellationToken ct = default)
    {
        return Connection.QuerySingleOrDefaultAsync(new CommandDefinition(sql, param, _transaction, commandTimeout,
            cancellationToken: ct));
    }

    public Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null, int commandTimeout = 30,
        CancellationToken ct = default)
    {
        return Connection.QuerySingleOrDefaultAsync<T>(new CommandDefinition(sql, param, _transaction,
            commandTimeout, cancellationToken: ct));
    }

    public T QueryFirst<T>(string sql, object param = null, int commandTimeout = COMMAND_TIMEOUT)
    {
        return Connection.QueryFirst<T>(sql, param, _transaction, commandTimeout);
    }

    public Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int commandTimeout = COMMAND_TIMEOUT,
        CancellationToken ct = default)
    {
        return Connection.QueryAsync<T>(new CommandDefinition(sql, param, _transaction, commandTimeout,
            cancellationToken: ct));
    }

    public async Task<T[]> QueryArrayAsync<T>(string sql, object param = null, int commandTimeout = COMMAND_TIMEOUT,
        CancellationToken ct = default)
    {
        var bufferedResult = await Connection.QueryAsync<T>(new CommandDefinition(sql, param, _transaction,
            commandTimeout, cancellationToken: ct));
        return bufferedResult.ToArray();
    }

    public Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null,
        int commandTimeout = DbSession.COMMAND_TIMEOUT, CancellationToken ct = default)
    {
        return Connection.QueryAsync(new CommandDefinition(sql, param, _transaction, commandTimeout,
            cancellationToken: ct));
    }

    public IEnumerable<T> Query<T>(string sql, object param = null, int commandTimeout = COMMAND_TIMEOUT,
        bool buffered = true)
    {
        return Connection.Query<T>(sql, param, _transaction, commandTimeout: commandTimeout, buffered: buffered);
    }

    public Task<dynamic> QueryFirstOrDefaultAsync(string sql, object param = null,
        int commandTimeout = DbSession.COMMAND_TIMEOUT, CancellationToken ct = default)
    {
        return Connection.QueryFirstOrDefaultAsync(new CommandDefinition(sql, param, _transaction, commandTimeout,
            cancellationToken: ct));
    }

    public Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null,
        int commandTimeout = COMMAND_TIMEOUT, CancellationToken ct = default)
    {
        return Connection.QueryFirstOrDefaultAsync<T>(new CommandDefinition(sql, param, _transaction,
            commandTimeout, cancellationToken: ct));
    }

    public T QueryFirstOrDefault<T>(string sql, object param = null, int commandTimeout = COMMAND_TIMEOUT)
    {
        return Connection.QueryFirstOrDefault<T>(sql, param, _transaction, commandTimeout);
    }

    public Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = COMMAND_TIMEOUT,
        CancellationToken ct = default)
    {
        return Connection.ExecuteAsync(new CommandDefinition(sql, param, _transaction, commandTimeout,
            cancellationToken: ct));
    }

    public int Execute(string sql, object param = null, int? commandTimeout = COMMAND_TIMEOUT)
    {
        return Connection.Execute(sql, param, _transaction, commandTimeout);
    }

    public Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = COMMAND_TIMEOUT,
        CancellationToken ct = default)
    {
        return Connection.ExecuteScalarAsync<T>(new CommandDefinition(sql, param, _transaction, commandTimeout,
            cancellationToken: ct));
    }

    public (T1[], T2[]) QueryMultiple<T1, T2>(string sql, object param = null,
        int? commandTimeout = COMMAND_TIMEOUT)
    {
        using var query = Connection.QueryMultiple(sql, param, _transaction, commandTimeout);
        var firstResultSet = query.Read<T1>();
        var secondResultSet = query.Read<T2>();

        return (firstResultSet.ToArray(), secondResultSet.ToArray());
    }

    public (T1[], T2[], T3[]) QueryMultiple<T1, T2, T3>(string sql, object param = null,
        int? commandTimeout = COMMAND_TIMEOUT)
    {
        using var query = Connection.QueryMultiple(sql, param, _transaction, commandTimeout);
        var firstResultSet = query.Read<T1>();
        var secondResultSet = query.Read<T2>();
        var thirdResultSet = query.Read<T3>();

        return (firstResultSet.ToArray(), secondResultSet.ToArray(), thirdResultSet.ToArray());
    }

    public async Task<(T1[], T2[])> QueryMultipleAsync<T1, T2>(string sql, object param = null,
        int? commandTimeout = COMMAND_TIMEOUT, CancellationToken ct = default)
    {
        await using var query =
            await Connection.QueryMultipleAsync(new CommandDefinition(sql, param, _transaction, commandTimeout,
                cancellationToken: ct));
        var firstResultSet = await query.ReadAsync<T1>();
        var secondResultSet = await query.ReadAsync<T2>();

        return (firstResultSet.ToArray(), secondResultSet.ToArray());
    }
}