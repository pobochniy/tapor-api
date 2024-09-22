using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using MySqlConnector;

namespace Dodo.Tools.DB.MySql
{
	public class ExecuteReaderResult : IDisposable
	{
		private readonly DataAccess _dataAccess;
		private readonly MySqlDataReader _reader;
		private readonly MySqlCommand _command;
		private readonly MySqlTransaction _transaction;


		public int AffectedRows => _reader.RecordsAffected;

		internal ExecuteReaderResult(DataAccess dataAccess, MySqlDataReader reader, MySqlCommand command, MySqlTransaction transaction)
		{
			_dataAccess = dataAccess;
			_reader = reader;
			_command = command;
			_transaction = transaction;
		}

		private T Read<T>(Func<DataReader, T> action)
		{
			if (_reader.Read()) return action(new DataReader(_reader));
			if (!_reader.NextResult()) return default(T);

			return _reader.Read() ? action(new DataReader(_reader)) : default(T);
		}

		private async Task<T> ReadAsync<T>(Func<DataReader, T> action, CancellationToken cancellationToken)
		{
			if (await _reader.ReadAsync(cancellationToken))
			{
				return action(new DataReader(_reader));
			}
			if (!await _reader.NextResultAsync(cancellationToken)) return default(T);

			return await _reader.ReadAsync(cancellationToken) ? action(new DataReader(_reader)) : default(T);
		}

		public bool NextResult()
		{
			return _reader.NextResult();
		}

		public async Task<bool> NextResultAsync(CancellationToken cancellationToken)
		{
			return await _reader.NextResultAsync(cancellationToken);
		}
		
		private List<T> ReadAll<T>(Func<DataReader, T> action)
		{
			var result = new List<T>();
			while (_reader.Read())
			{
				result.Add(action(new DataReader(_reader)));
			}

			return result;
		}

		private async Task<List<T>> ReadAllAsync<T>(Func<DataReader, T> action, CancellationToken cancellationToken)
		{
			var result = new List<T>();
			while (await _reader.ReadAsync(cancellationToken))
			{
				result.Add(action(new DataReader(_reader)));
			}

			return result;
		}

		private void DisposeData()
		{
			_command.Parameters.Clear();

			_reader.Dispose();
			if (_transaction == null) Dispose();
		}

		public T GetObject<T>(Func<DataReader, T> action, bool closeConnection = true)
		{
			try
			{
				return Read(action);
			}
			finally
			{
				if (closeConnection) DisposeData();
			}
		}

		public async Task<T> GetObjectAsync<T>(Func<DataReader, T> action, bool closeConnection = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				return await ReadAsync(action, cancellationToken);
			}
			finally
			{
				if (closeConnection) DisposeData();
			}
		}

		public T[] GetArray<T>(Func<DataReader, T> action, bool closeConnection = true)
		{
			try
			{
				return ReadAll(action).ToArray();
			}
			finally
			{
				if (closeConnection) DisposeData();
			}
		}

		public async Task<T[]> GetArrayAsync<T>(Func<DataReader, T> action, bool closeConnection = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				return (await ReadAllAsync(action, cancellationToken)).ToArray();
			}
			finally
			{
				if (closeConnection) DisposeData();
			}
		}

		public List<T> GetList<T>(Func<DataReader, T> action, bool closeConnection = true)
		{
			try
			{
				return ReadAll(action);
			}
			finally
			{
				if (closeConnection) DisposeData();
			}
		}

		public async Task<List<T>> GetListAsync<T>(Func<DataReader, T> action,  bool closeConnection = true, CancellationToken cancellationToken = default(CancellationToken))
		{
			try
			{
				return await ReadAllAsync(action, cancellationToken);
			}
			finally
			{
				if (closeConnection) DisposeData();
			}
		}

		public IEnumerable<T> ToIEnumerable<T>(Func<DataReader, T> action, bool closeConnection = true)
		{
			try
			{
				return ReadAll(action);
			}
			finally
			{
				if (closeConnection) DisposeData();
			}
		}

		public async Task<IEnumerable<T>> ToIEnumerableAsync<T>(Func<DataReader, T> action, bool closeConnection = true, CancellationToken cancellationToken = default (CancellationToken))
		{
			try
			{
				return await ReadAllAsync(action, cancellationToken);
			}
			finally
			{
				if (closeConnection) DisposeData();
			}
		}

		public void Dispose()
		{
			_dataAccess?.Dispose();

			GC.SuppressFinalize(this);
		}
	}
}