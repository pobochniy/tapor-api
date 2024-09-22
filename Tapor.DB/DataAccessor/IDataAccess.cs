using MySqlConnector;

namespace Dodo.Tools.DB.MySql;

public interface IDataAccess : IDisposable
{
	MySqlConnection Connection { get; }
	bool IsTransaction { get; }
	MySqlTransaction Transaction { get; }

	DataAccess AddArrayParameter(string key, IEnumerable<Uuid> value);
	DataAccess AddArrayParameter(string key, IEnumerable<UUId> value);
	DataAccess AddArrayParameter<T>(string key, IEnumerable<T> value);
	DataAccess AddJsonParameter(string key, object value);
	DataAccess AddOptionalArrayParameter(string arrayKey, string skipOptionKey, IEnumerable<Uuid> value);
	DataAccess AddOptionalArrayParameter(string arrayKey, string skipOptionKey, IEnumerable<UUId> value);
	DataAccess AddOptionalArrayParameter<T>(string arrayKey, string skipOptionKey, IEnumerable<T> value);
	DataAccess AddParameter(string key, Guid value);
	DataAccess AddParameter(string key, IEnumerable<byte> value);
	DataAccess AddParameter(string key, IEnumerable<DateTime> value);
	DataAccess AddParameter(string key, IEnumerable<UUId> value);
	DataAccess AddParameter(string key, object value);
	DataAccess AddParameter(string key, string value);
	DataAccess AddParameter(string key, UUId value);
	DataAccess AddParameter<T>(string key, IEnumerable<T> value);
	DataAccess ClearParameters();
	void Commit();
	Task CommitAsync(CancellationToken cancellationToken = default);
	void ExecuteSqlNonQuery(string sql, int? timeoutInSeconds = null);
	Task ExecuteSqlNonQueryAsync(string sql, int? timeoutInSeconds = null, CancellationToken cancellationToken = default);
	ExecuteReaderResult ExecuteSqlReader(string sql, int? timeoutInSeconds = null);
	Task<ExecuteReaderResult> ExecuteSqlReaderAsync(string sql, int? timeoutInSeconds = null, CancellationToken cancellationToken = default);
	T ExecuteSqlScalar<T>(string sql);
	Task<T> ExecuteSqlScalarAsync<T>(string sql, CancellationToken cancellationToken = default);
	T? ExecuteSqlScalarOld<T>(string sql) where T : struct;
	void Rollback();
	Task RollbackAsync();
}