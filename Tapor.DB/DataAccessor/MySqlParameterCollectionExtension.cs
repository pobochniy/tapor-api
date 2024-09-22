using System.Data;
using System.Text.Json;
using MySqlConnector;

namespace Dodo.Tools.DB.MySql
{
	public static class MySqlParameterCollectionExtension
	{
		public static void AddParameter(this MySqlParameterCollection mySqlParameterCollection, string key, Guid value)
		{
			mySqlParameterCollection.AddWithValue(key, value.ToByteArray());
		}

		/// <summary>
		/// Obsolete. This call casts uuid to string.
		/// </summary>
		public static void AddParameter(this MySqlParameterCollection mySqlParameterCollection, string key, UUId value)
		{
			mySqlParameterCollection.AddWithValue(key, value?.ToString());
		}

		public static void AddParameter(this MySqlParameterCollection mySqlParameterCollection, string key, object value)
		{
			value = (value != null && value.GetType().IsEnum ? Convert.ToInt32(value) : value);
			mySqlParameterCollection.AddWithValue(key, value);
		}

		public static void AddParameter(this MySqlParameterCollection mySqlParameterCollection, string key, string value)
		{
			mySqlParameterCollection.AddWithValue(key, value);
		}

		public static void AddParameter(this MySqlParameterCollection mySqlParameterCollection, string key, IEnumerable<UUId> value)
		{
			mySqlParameterCollection.AddWithValue(key, ToMySqlArray(value));
		}

		public static void AddParameter(this MySqlParameterCollection mySqlParameterCollection, string key, IEnumerable<DateTime> value)
		{
			mySqlParameterCollection.AddWithValue(key, ToMySqlArray(value));
		}

		public static void AddParameter(this MySqlParameterCollection mySqlParameterCollection, string key, IEnumerable<byte> value)
		{
			mySqlParameterCollection.AddWithValue(key, value);
		}

		public static void AddParameter<T>(this MySqlParameterCollection mySqlParameterCollection, string key, IEnumerable<T> value)
		{
			mySqlParameterCollection.AddWithValue(key, ToMySqlArray(value));
		}
		
		public static IEnumerable<MySqlParameter> AddArrayParameter<T>(
			this MySqlParameterCollection mySqlParameterCollection,
			string key,
			IEnumerable<T> values)
		{
			return values.Select((value, index) => mySqlParameterCollection.AddWithValue($"{key}{index}", value));
		}

		public static void AddJsonParameter(this MySqlParameterCollection mySqlParameterCollection, string key, object value)
		{
			mySqlParameterCollection.AddWithValue(key, JsonSerializer.Serialize(value));
		}
		
		public static MySqlParameter AddOutputParameter(this MySqlParameterCollection mySqlParameterCollection, string key, MySqlDbType type = MySqlDbType.Int64, int size = 8)
		{
			var parameter = mySqlParameterCollection.AddParameterWithoutValue(key, type, size);
			mySqlParameterCollection[key].Direction = ParameterDirection.Output;

			return parameter;
		}
		
		public static MySqlParameter AddParameterWithoutValue(this MySqlParameterCollection mySqlParameterCollection, string key, MySqlDbType type = MySqlDbType.Int64, int size = 8)
		{
			var parameter = new MySqlParameter(key, type, size);
			mySqlParameterCollection.Add(parameter);
			return parameter;
		}
		
		private static string ToMySqlArray(IEnumerable<DateTime> source)
		{
			if (source == null) return null;

			return $"'{string.Join("','", source.Select(x => x.ToString("yyyy.MM.dd")))}'";
		}

		private static string ToMySqlArray(IEnumerable<UUId> source)
		{
			if (source == null || !source.Any()) return null;

			return string.Join(",", source.Select(x => $"UNHEX('{x}')"));
		}

		private static string ToMySqlArray<T>(IEnumerable<T> source)
		{
			if (source == null) return null;

			return (typeof(T).IsEnum ? string.Join(",", source.Cast<int>()) : string.Join(",", source));
		}
	}
}