using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace Dodo.Tools.DB.MySql
{
	public sealed class DataAccess : IDataAccess
	{
		private readonly bool _errorLogging;
		private static readonly ILogger _logger;
		internal bool _createdFromAsync = false;

		private const int DefaultTimeOut = 120;

		public static DataAccess Command(string connection)
		{
			return new DataAccess(connection);
		}

		public static void ExecuteInTransaction(string connection, Action<DataAccess> method, IsolationLevel isolationLevel = IsolationLevel.Unspecified)
		{
			using (var dataAccess = new DataAccess(connection, true, isolationLevel: isolationLevel))
			{
				try
				{
					method(dataAccess);

					dataAccess.Commit();
				}
				catch
				{
					dataAccess.Rollback();
					throw;
				}
			}
		}

		public static async Task ExecuteInTransactionAsync(string connection, Func<DataAccess, CancellationToken, Task> method, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var dataAccess = await CreateAsync(connection, true, cancellationToken: cancellationToken))
			{
				try
				{
					await method(dataAccess, cancellationToken);

					await dataAccess.CommitAsync(cancellationToken);
				}
				catch
				{
					await dataAccess.RollbackAsync();
					throw;
				}
			}
		}

		public static T ExecuteInTransaction<T>(string connection, Func<DataAccess, T> method)
		{
			using (var dataAccess = new DataAccess(connection, true))
			{
				try
				{
					var result = method(dataAccess);
					dataAccess.Commit();
					return result;
				}
				catch
				{
					dataAccess.Rollback();
					throw;
				}
			}
		}

		public static async Task<T> ExecuteInTransactionAsync<T>(string connection, Func<DataAccess, CancellationToken, Task<T>> method, CancellationToken cancellationToken = default(CancellationToken))
		{
			using (var dataAccess = await CreateAsync(connection, true, cancellationToken: cancellationToken))
			{
				try
				{
					var result = await method(dataAccess, cancellationToken);
					await dataAccess.CommitAsync(cancellationToken);
					return result;
				}
				catch
				{
					await dataAccess.RollbackAsync();
					throw;
				}
			}
		}

		// Disposed in Execute* methods
		internal readonly MySqlConnection _connection;
		public MySqlConnection Connection => _connection;
		private readonly MySqlCommand _command;
		private readonly IDictionary<string, int> _arrayParameters = new Dictionary<string, int>();
		private MySqlDataReader _reader;

		public static async Task<DataAccess> CreateAsync(string connectionString, bool beginTransaction = false,
			int? timeOut = null, bool errorLogging = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			MySqlConnection connection = null;
			MySqlCommand command = null;
			MySqlTransaction transaction = null;

			try
			{
				connection = new MySqlConnection(connectionString);
				await connection.OpenAsync(cancellationToken);
				command = connection.CreateCommand();
				command.CommandTimeout = timeOut ?? DefaultTimeOut;
				command.Parameters.Clear();
				if (!beginTransaction) return new DataAccess(errorLogging, connection, command, null, null)
				{
					_createdFromAsync = true
				};

				transaction = await connection.BeginTransactionAsync(cancellationToken);
				command.Transaction = transaction;
				var dataAccess = new DataAccess(errorLogging, connection, command, null, transaction)
				{
					_createdFromAsync = true
				};
				return dataAccess;
			}
			catch (Exception)
			{
				transaction?.Dispose();
				command?.Dispose();
				connection?.Dispose();
				throw;
			}
		}

		private DataAccess(bool errorLogging, MySqlConnection connection, MySqlCommand command, MySqlDataReader reader, MySqlTransaction transaction)
		{
			_errorLogging = errorLogging;
			_connection = connection;
			_command = command;
			_reader = reader;
			_transaction = transaction;
		}

		public DataAccess(string connection, bool beginTransaction = false, int? timeOut = null, bool errorLogging = true,
			IsolationLevel isolationLevel = IsolationLevel.Unspecified)
		{
			_errorLogging = errorLogging;
			_connection = new MySqlConnection(connection);
			_connection.Open();

			_command = _connection.CreateCommand();
			_command.CommandTimeout = timeOut ?? DefaultTimeOut;
			_command.Parameters.Clear();

			if (beginTransaction)
			{
				_transaction = _connection.BeginTransaction(isolationLevel);
				_command.Transaction = _transaction;
			}
		}

		/// <summary>
		/// Obsolete. Use DataAccess.Command(String connection) or DataAccess.ExecuteInTransaction(String connection, …)
		/// </summary>
		public DataAccess(MySqlConnection connection, bool beginTransaction = false, int? timeOut = null, bool errorLogging = true)
		{
			if (connection.State != ConnectionState.Open) throw new ArgumentException("The connection is not opened.", "connection");

			_connection = connection;
			_errorLogging = errorLogging;

			_command = _connection.CreateCommand();
			_command.CommandTimeout = timeOut ?? DefaultTimeOut;
			_command.Parameters.Clear();

			if (beginTransaction)
			{
				_transaction = _connection.BeginTransaction();
				_command.Transaction = _transaction;
			}
		}

		public DataAccess AddParameter(string key, Guid value)
		{
			_command.Parameters.AddParameter(key, value);

			return this;
		}

		/// <summary>
		/// Obsolete. This call casts UUId to string.
		/// </summary>
		public DataAccess AddParameter(string key, UUId value)
		{
			_command.Parameters.AddParameter(key, value);

			return this;
		}

		public DataAccess AddParameter(string key, object value)
		{
			_command.Parameters.AddParameter(key, value);

			return this;
		}

		public DataAccess AddParameter(string key, string value)
		{
			_command.Parameters.AddParameter(key, value);

			return this;
		}

		public DataAccess AddParameter(string key, IEnumerable<UUId> value)
		{
			_command.Parameters.AddParameter(key, value);

			return this;
		}

		public DataAccess AddParameter(string key, IEnumerable<DateTime> value)
		{
			_command.Parameters.AddParameter(key, value);

			return this;
		}

		public DataAccess AddParameter(string key, IEnumerable<byte> value)
		{
			_command.Parameters.AddParameter(key, value);

			return this;
		}

		public DataAccess AddParameter<T>(string key, IEnumerable<T> value)
		{
			_command.Parameters.AddParameter(key, value);

			return this;
		}

		public DataAccess AddArrayParameter<T>(string key, IEnumerable<T> value)
		{
			var array = value as T[] ?? value.ToArray();
			for (var i = 0; i < array.Length; i++)
			{
				_command.Parameters.AddParameter($"{key}_{i}", array[i]);
			}
			_arrayParameters.Add(key, array.Length);

			return this;
		}

		public DataAccess AddArrayParameter(string key, IEnumerable<UUId> value)
		{
			var i = 0;
			foreach (var item in value)
			{
				_command.Parameters.AddParameter($"{key}_{i}", item.ToByteArray());
				i++;
			}
			_arrayParameters.Add(key, i); // i equals length of enumerable

			return this;
		}

		public DataAccess AddOptionalArrayParameter<T>(string arrayKey, string skipOptionKey, IEnumerable<T> value)
		{
			if (value == null || !value.Any())
			{
				AddParameter(skipOptionKey, true);
				return this;
			}

			AddParameter(skipOptionKey, false);

			return AddArrayParameter(arrayKey, value);
		}

		public DataAccess AddArrayParameter(string key, IEnumerable<Uuid> value)
		{
			var i = 0;
			foreach (var item in value)
			{
				_command.Parameters.AddParameter($"{key}_{i}", item.ToByteArray());
				i++;
			}
			_arrayParameters.Add(key, i); // i equals length of enumerable

			return this;
		}

		public DataAccess AddOptionalArrayParameter(string arrayKey, string skipOptionKey, IEnumerable<Uuid> value)
		{
			if (value == null || !value.Any())
			{
				AddParameter(skipOptionKey, true);
				return this;
			}

			AddParameter(skipOptionKey, false);

			return AddArrayParameter(arrayKey, value);
		}

		public DataAccess AddOptionalArrayParameter(string arrayKey, string skipOptionKey, IEnumerable<UUId> value)
		{
			if (value == null || !value.Any())
			{
				AddParameter(skipOptionKey, true);
				return this;
			}

			AddParameter(skipOptionKey, false);

			return AddArrayParameter(arrayKey, value);
		}

		public DataAccess AddJsonParameter(string key, object value)
		{
			_command.Parameters.AddJsonParameter(key, value);

			return this;
		}

		public DataAccess ClearParameters()
		{
			_command.Parameters.Clear();

			return this;
		}

		private readonly MySqlTransaction _transaction;
		public MySqlTransaction Transaction => _transaction;

		public bool IsTransaction => _transaction != null;

		public void Commit()
		{
			_transaction.Commit();
		}

		public async Task CommitAsync(CancellationToken cancellationToken = default(CancellationToken))
		{
			if (!_createdFromAsync)
			{
				_logger.LogDebug("Data Access was created from constructor, but uses async methods");
			}
			await _transaction.CommitAsync(cancellationToken);
		}

		public void Rollback()
		{
			_transaction.Rollback();
		}

		public async Task RollbackAsync()
		{
			await _transaction.RollbackAsync();
		}

		public void ExecuteSqlNonQuery(string sql, int? timeoutInSeconds = null)
		{
			if (_arrayParameters.Count != 0) InsertArrayParameters(ref sql);

			ExecuteNonQueryInternal(CommandType.Text, sql, timeoutInSeconds);
		}

		public Task ExecuteSqlNonQueryAsync(string sql, int? timeoutInSeconds = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_arrayParameters.Count != 0) InsertArrayParameters(ref sql);

			return ExecuteNonQueryInternalAsync(CommandType.Text, sql, timeoutInSeconds, cancellationToken);
		}

		public T ExecuteSqlScalar<T>(string sql)
		{
			if (_arrayParameters.Count != 0) InsertArrayParameters(ref sql);

			return ExecuteScalarInternal<T>(CommandType.Text, sql);
		}

		public Task<T> ExecuteSqlScalarAsync<T>(string sql, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_arrayParameters.Count != 0) InsertArrayParameters(ref sql);

			return ExecuteScalarInternalAsync<T>(CommandType.Text, sql, cancellationToken);
		}
		public ExecuteReaderResult ExecuteSqlReader(string sql, int? timeoutInSeconds = null)
		{
			if (_arrayParameters.Count != 0) InsertArrayParameters(ref sql);

			return ExecuteReaderInternal(CommandType.Text, sql, timeoutInSeconds);
		}

		public Task<ExecuteReaderResult> ExecuteSqlReaderAsync(string sql, int? timeoutInSeconds = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (_arrayParameters.Count != 0) InsertArrayParameters(ref sql);

			return ExecuteReaderInternalAsync(CommandType.Text, sql, timeoutInSeconds, cancellationToken);
		}

		/// <summary>
		/// Obsolete. Don't use.
		/// </summary>
		public T? ExecuteSqlScalarOld<T>(string sql) where T : struct
		{
			try
			{
				_command.CommandType = CommandType.Text;
				_command.CommandText = sql;

				var scalar = _command.ExecuteScalar();

				if (scalar == null || scalar == DBNull.Value)
				{
					return null;
				}
				return (T)Convert.ChangeType(scalar, conversionType: typeof(T));
			}
			catch (MySqlException mySqlException)
			{
				if (_errorLogging)
				{
					LogException(sql, _command, mySqlException);
				}
				throw;
			}
			catch (Exception ex)
			{
				LogException(sql, _command, ex);
				throw;
			}
			finally
			{
				_command.Parameters.Clear();
				if (_transaction == null) Dispose();
			}
		}

		private void ExecuteNonQueryInternal(CommandType commandType, string commandText, int? timeoutInSeconds)
		{
			try
			{
				_command.CommandType = commandType;
				_command.CommandText = commandText;

				if (timeoutInSeconds.HasValue)
				{
					_command.CommandTimeout = timeoutInSeconds.Value;
				}

				_command.ExecuteNonQuery();
			}
			catch (MySqlException mySqlException)
			{
				if (_errorLogging)
				{
					LogException(commandText, _command, mySqlException);
				}
				throw;
			}
			catch (Exception ex)
			{
				LogException(commandText, _command, ex);
				throw;
			}
			finally
			{
				_command.Parameters.Clear();
				if (_transaction == null) Dispose();
			}
		}

		private async Task ExecuteNonQueryInternalAsync(CommandType commandType, string commandText, int? timeoutInSeconds, CancellationToken cancellationToken)
		{
			try
			{
				if (!_createdFromAsync)
				{
					_logger.LogDebug("Data Access was created from constructor, but uses async methods");
				}
				_command.CommandType = commandType;
				_command.CommandText = commandText;

				if (timeoutInSeconds.HasValue)
				{
					_command.CommandTimeout = timeoutInSeconds.Value;
				}

				await _command.ExecuteNonQueryAsync(cancellationToken);
			}
			catch (MySqlException mySqlException)
			{
				if (_errorLogging)
				{
					LogException(commandText, _command, mySqlException);
				}
				throw;
			}
			catch (Exception ex)
			{
				LogException(commandText, _command, ex);
				throw;
			}
			finally
			{
				_command.Parameters.Clear();
				if (_transaction == null) Dispose();
			}
		}

		private T ExecuteScalarInternal<T>(CommandType commandType, string commandText)
		{
			try
			{
				_command.CommandType = commandType;
				_command.CommandText = commandText;

				var result = _command.ExecuteScalar();


				var type = typeof(T);

				if (type.IsArray)
					return ConvertToArray<T>(Convert.ToString(result));

				var underlyingType = Nullable.GetUnderlyingType(type);

				if (underlyingType != null)
					return ConvertToNullable<T>(result, underlyingType);

				return (T)Convert.ChangeType(result, type);
			}
			catch (MySqlException mySqlException)
			{
				if (_errorLogging)
				{
					LogException(commandText, _command, mySqlException);
				}
				throw;
			}
			catch (Exception ex)
			{
				LogException(commandText, _command, ex);
				throw;
			}
			finally
			{
				_command.Parameters.Clear();
				if (_transaction == null) Dispose();
			}
		}

		private async Task<T> ExecuteScalarInternalAsync<T>(CommandType commandType, string commandText, CancellationToken cancellationToken)
		{
			try
			{
				if (!_createdFromAsync)
				{
					_logger.LogDebug("Data Access was created from constructor, but uses async methods");
				}
				_command.CommandType = commandType;
				_command.CommandText = commandText;

				var result = await _command.ExecuteScalarAsync(cancellationToken);

				var type = typeof(T);

				if (type.IsArray)
					return ConvertToArray<T>(Convert.ToString(result));

				var underlyingType = Nullable.GetUnderlyingType(type);

				if (underlyingType != null)
					return ConvertToNullable<T>(result, underlyingType);

				return (T)Convert.ChangeType(result, type);
			}
			catch (MySqlException mySqlException)
			{
				if (_errorLogging)
				{
					LogException(commandText, _command, mySqlException);
				}
				throw;
			}
			catch (Exception ex)
			{
				LogException(commandText, _command, ex);
				throw;
			}
			finally
			{
				_command.Parameters.Clear();
				if (_transaction == null) Dispose();
			}
		}

		private static T ConvertToArray<T>(string value)
		{
			var type = typeof(T);
			var elementType = type.GetElementType();

			if (string.IsNullOrEmpty(value))
			{
				var emptyArray = Array.CreateInstance(elementType, 0);

				return (T)Convert.ChangeType(emptyArray, type);
			}

			var data = value.Split(',');
			var array = Array.CreateInstance(elementType, data.Length);

			for (var i = 0; i < data.Length; i++)
				array.SetValue(Convert.ChangeType(data[i], elementType), i);

			return (T)Convert.ChangeType(array, type);
		}

		private static T ConvertToNullable<T>(object value, Type underlyingType)
		{
			if (Convert.IsDBNull(value) || value == null) return default(T);

			return (T)Convert.ChangeType(value, underlyingType);
		}
		private ExecuteReaderResult ExecuteReaderInternal(CommandType commandType, string commandText, int? timeoutInSeconds = null)
		{
			try
			{
				_command.CommandType = commandType;
				_command.CommandText = commandText;

				if (timeoutInSeconds.HasValue)
				{
					_command.CommandTimeout = timeoutInSeconds.Value * 1000;
				}

				_reader = _command.ExecuteReader();

				return new ExecuteReaderResult(this, _reader, _command, _transaction);
			}
			catch (MySqlException mySqlException)
			{
				if (_errorLogging)
				{
					LogException(commandText, _command, mySqlException);
				}

				throw;
			}
			catch (Exception ex)
			{
				LogException(commandText, _command, ex);
				if (_transaction == null)
				{
					Dispose();
				}

				throw;
			}
			finally
			{
				_command.Parameters.Clear();
			}
		}

		private async Task<ExecuteReaderResult> ExecuteReaderInternalAsync(CommandType commandType, string commandText, int? timeoutInSeconds, CancellationToken cancellationToken)
		{
			if (!_createdFromAsync)
			{
				_logger.LogDebug("Data Access was created from constructor, but uses async methods");
			}

			try
			{
				_command.CommandType = commandType;
				_command.CommandText = commandText;

				if (timeoutInSeconds.HasValue)
				{
					_command.CommandTimeout = timeoutInSeconds.Value * 1000;
				}

				_reader = (MySqlDataReader)await _command.ExecuteReaderAsync(cancellationToken);

				return new ExecuteReaderResult(this, _reader, _command, _transaction);
			}
			catch (MySqlException mySqlException)
			{
				if (_errorLogging)
				{
					LogException(commandText, _command, mySqlException);
				}

				throw;
			}
			catch (Exception ex)
			{
				LogException(commandText, _command, ex);
				if (_transaction == null)
				{
					Dispose();
				}

				throw;
			}
			finally
			{
				_command.Parameters.Clear();
			}
		}

		private void LogException(string sql, MySqlCommand command, Exception exception)
		{
			if (!_errorLogging)
			{
				return;
			}

			try
			{
				var sb = new StringBuilder();
				sb.AppendLine($"Execution of \"{sql}\" failed");

				sb.AppendLine();
				sb.AppendLine("Parameters");
				var parameters = new List<string[]>();
				foreach (MySqlParameter parameter in command.Parameters)
				{
					var value = GetString(parameter.Value);
					sb.AppendLine($"\t{parameter.ParameterName} : {value}");
					parameters.Add(new[] { parameter.ParameterName, value });
				}
				sb.AppendLine();

				sb.AppendLine("Exception");
				sb.AppendLine("\t" + exception);

				sb.AppendLine();
				sb.AppendLine("Connection");
				sb.AppendLine(command.Connection.ConnectionString);

				// _logger.ForErrorEvent()
				// 	.Exception(exception)
				// 	.Message(sb.ToString())
				// 	.Property("Connection", command.Connection.ConnectionString)
				// 	.Property("Sql", sql)
				// 	.Property("SqlParameters", parameters)
				// 	.Log();
			}
			catch
			{
				// Логирование не должно бросать эксепшены.
			}
		}

		private static string GetString(object id)
		{
			if (id == null)
			{
				return "NULL";
			}

			if (id is byte[] bytes)
			{
				return BitConverter.ToString(bytes).Replace("-", "");
			}

			return id.ToString();
		}

		private void InsertArrayParameters(ref string sql)
		{
			foreach (var (paramName, arrayLength) in _arrayParameters)
			{
				var pattern = $@"@\b{paramName}\b";
				var replacement = string.Join(",", Enumerable.Range(0, arrayLength).Select(i => $"@{paramName}_{i}"));
				sql = Regex.Replace(sql, pattern, replacement);
			}
		}

		public void Dispose()
		{
			try
			{
				_reader?.Dispose();
				_command?.Dispose();
				_transaction?.Dispose();
				_connection?.Dispose();

				GC.SuppressFinalize(this);
			}
			catch { }
		}
	}
}