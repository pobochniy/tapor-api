using System.Data;
using System.Data.Common;
using System.Text.Json;
using MySqlConnector;

namespace Dodo.Tools.DB.MySql
{
	public class DataReader //: IDataReader
	{
		// Disposed in ExecuteReaderResult
		private readonly MySqlDataReader _reader;

		public DataReader(MySqlDataReader reader)
		{
			_reader = reader;
		}

		public MySqlDataReader GetMySqlReader()
		{
			return _reader;
		}

		public string Prefix { get; set; }

		public void ClearPrefix()
		{
			Prefix = string.Empty;
		}

		public void SetPrefix(string prefix)
		{
			Prefix = prefix;
		}

		public bool HasValue
		{
			get { return true; }
		}

		/// <summary>
		/// Содержит ли адаптер данных столбец key
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public bool Has(string key)
		{
			return _reader.HasColumn(Prefix + key);
		}

		public bool IsNull(string key)
		{
			return Convert.IsDBNull(_reader[Prefix + key]);
		}

		public bool IsNotNull(string key)
		{
			return !Convert.IsDBNull(_reader[Prefix + key]);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public T Get<T>(string key)
		{
			object value = _reader[Prefix + key];
			Type destinationType = typeof(T).IsEnum ? Enum.GetUnderlyingType(typeof(T)) : typeof(T);

			return (T)Convert.ChangeType(value, destinationType);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public T? GetNullable<T>(string key) where T : struct
		{
			if (Convert.IsDBNull(_reader[Prefix + key])) return null;

			return Get<T>(key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public T[] GetArray<T>(string key) where T : struct
		{
			string value = MySqlDataReaderExtension.GetString(_reader, Prefix + key);
			if (string.IsNullOrEmpty(value)) return Array.Empty<T>();

			const char separator = ',';
			string[] data = value.Split(separator);
			return Array.ConvertAll(data, x => (T)(object)Convert.ToInt32(x));
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// <exception cref="JsonException"></exception>
		/// </summary>
		public T GetJson<T>(string key)
		{
			return JsonSerializer.Deserialize<T>(MySqlDataReaderExtension.GetString(_reader, Prefix + key));
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public Guid GetGuid(string key)
		{
			return MySqlDataReaderExtension.GetGuid(_reader, Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public Guid? GetGuidNullable(string key)
		{
			return _reader.GetGuidNullable(Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public bool GetBoolean(string key)
		{
			return MySqlDataReaderExtension.GetBoolean(_reader, Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public bool? GetBooleanNullable(string key)
		{
			return _reader.GetBooleanNullable(Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public byte GetByte(string key)
		{
			return MySqlDataReaderExtension.GetByte(_reader, Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public byte? GetByteNullable(string key)
		{
			return _reader.GetByteNullable(Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public int GetInt32(string key)
		{
			return MySqlDataReaderExtension.GetInt32(_reader, Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public int? GetInt32Nullable(string key)
		{
			return _reader.GetInt32Nullable(Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public long GetInt64(string key)
		{
			return MySqlDataReaderExtension.GetInt64(_reader, Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public long? GetInt64Nullable(string key)
		{
			return _reader.GetInt64Nullable(Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public string GetString(string key)
		{
			return MySqlDataReaderExtension.GetString(_reader, Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public DateTime GetDateTime(string key, DateTimeKind kind = DateTimeKind.Unspecified)
		{
			return MySqlDataReaderExtension.GetDateTime(_reader, Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public DateTime? GetDateTimeNullable(string key, DateTimeKind kind = DateTimeKind.Unspecified)
		{
			return _reader.GetDateTimeNullable(Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public double GetDouble(string key)
		{
			return MySqlDataReaderExtension.GetDouble(_reader, Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public double? GetDoubleNullable(string key)
		{
			return _reader.GetDoubleNullable(Prefix + key);
		}

		/// <summary>
		/// <exception cref="InvalidCastException"></exception>
		/// <exception cref="IndexOutOfRangeException"></exception>
		/// <exception cref="MySqlException"></exception>
		/// </summary>
		public decimal GetDecimal(string key)
		{
			return MySqlDataReaderExtension.GetDecimal(_reader, Prefix + key);
		}

		public decimal? GetDecimalNullable(string key)
		{
			return _reader.GetDecimalNullable(Prefix + key);
		}

		public UUId GetUUId(string key)
		{
			return _reader.GetUUId(Prefix + key);
		}

		public Uuid GetPrimitivesUuid(string key)
		{
			return _reader.GetPrimitivesUuid(Prefix + key);
		}

		public Uuid? GetPrimitivesUuidNullable(string key)
		{
			return _reader.GetPrimitivesUuidNullable(Prefix + key);
		}

		public static explicit operator DbDataReader(DataReader dataReader)
		{
			return dataReader._reader;
		}

		public void Dispose()
		{
			_reader.Dispose();
		}

		public async ValueTask DisposeAsync()
		{
			await _reader.DisposeAsync();
		}
	}
}