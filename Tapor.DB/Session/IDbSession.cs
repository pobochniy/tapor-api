using System.Data;

namespace Tapor.DB.Session;

public interface IDbSession : IDisposable
{
    IDbConnection Connection { get; }
    IDbTransaction Transaction { get; }

    void Commit();
    Task CommitAsync(CancellationToken ct);
    void Rollback();
    Task RollbackAsync(CancellationToken ct);

    Task<T> QueryFirstAsync<T>(string sql, object param = null, int commandTimeout = DbSession.COMMAND_TIMEOUT,
        CancellationToken ct = default(CancellationToken));

    Task<T> QuerySingleAsync<T>(string sql, object param = null, int commandTimeout = DbSession.COMMAND_TIMEOUT,
        CancellationToken ct = default(CancellationToken));

    Task<dynamic> QuerySingleAsync(string sql, object param = null, int commandTimeout = DbSession.COMMAND_TIMEOUT,
        CancellationToken ct = default(CancellationToken));

    Task<dynamic> QuerySingleOrDefaultAsync(string sql, object param = null,
        int commandTimeout = DbSession.COMMAND_TIMEOUT, CancellationToken ct = default(CancellationToken));

    Task<T> QuerySingleOrDefaultAsync<T>(string sql, object param = null,
        int commandTimeout = DbSession.COMMAND_TIMEOUT, CancellationToken ct = default(CancellationToken));

    T QueryFirst<T>(string sql, object param = null, int commandTimeout = DbSession.COMMAND_TIMEOUT);

    Task<IEnumerable<T>> QueryAsync<T>(string sql, object param = null, int commandTimeout = DbSession.COMMAND_TIMEOUT,
        CancellationToken ct = default(CancellationToken));

    Task<T[]> QueryArrayAsync<T>(string sql, object param = null, int commandTimeout = DbSession.COMMAND_TIMEOUT,
        CancellationToken ct = default(CancellationToken));

    Task<IEnumerable<dynamic>> QueryAsync(string sql, object param = null,
        int commandTimeout = DbSession.COMMAND_TIMEOUT, CancellationToken ct = default(CancellationToken));

    IEnumerable<T> Query<T>(string sql, object param = null, int commandTimeout = DbSession.COMMAND_TIMEOUT,
        bool buffered = true);

    Task<dynamic> QueryFirstOrDefaultAsync(string sql, object param = null,
        int commandTimeout = DbSession.COMMAND_TIMEOUT, CancellationToken ct = default(CancellationToken));

    Task<T> QueryFirstOrDefaultAsync<T>(string sql, object param = null, int commandTimeout = DbSession.COMMAND_TIMEOUT,
        CancellationToken ct = default(CancellationToken));

    T? QueryFirstOrDefault<T>(string sql, object param = null, int commandTimeout = DbSession.COMMAND_TIMEOUT);

    Task<int> ExecuteAsync(string sql, object param = null, int? commandTimeout = DbSession.COMMAND_TIMEOUT,
        CancellationToken ct = default(CancellationToken));

    int Execute(string sql, object param = null, int? commandTimeout = DbSession.COMMAND_TIMEOUT);

    Task<T> ExecuteScalarAsync<T>(string sql, object param = null, int? commandTimeout = DbSession.COMMAND_TIMEOUT,
        CancellationToken ct = default(CancellationToken));

    (T1[], T2[]) QueryMultiple<T1, T2>(string sql, object param = null,
        int? commandTimeout = DbSession.COMMAND_TIMEOUT);

    (T1[], T2[], T3[]) QueryMultiple<T1, T2, T3>(string sql, object param = null,
        int? commandTimeout = DbSession.COMMAND_TIMEOUT);

    Task<(T1[], T2[])> QueryMultipleAsync<T1, T2>(string sql, object param = null,
        int? commandTimeout = DbSession.COMMAND_TIMEOUT, CancellationToken ct = default(CancellationToken));
}