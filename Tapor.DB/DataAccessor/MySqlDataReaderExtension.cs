using System;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace Dodo.Tools.DB.MySql
{
	public static class MySqlDataReaderExtension
	{
		private static readonly ILogger _logger;

		public static object GetValue(this MySqlDataReader reader, string name)
		{
			if (reader.IsDbNull(name))
			{
				return null;
			}

			return reader.GetValue(reader.GetOrdinal(name));
		}

		public static Guid GetGuid(this MySqlDataReader reader, string name)
		{
			try
			{
				return reader.GetGuid(reader.GetOrdinal(name));
			}
			catch (InvalidCastException ex)
			{
				_logger.LogError(ex, $"Invalid type conversion to '{typeof(Guid)}' in field '{name}'");
				throw;
			}
		}
		
		public static Guid? GetGuidNullable(this MySqlDataReader reader, string name)
		{
			if (reader.IsDbNull(name))
			{
				return null;
			}

			return GetGuid(reader, name);
		}

		public static UUId GetUUId(this MySqlDataReader reader, string name)
		{
			var ordinal = reader.GetOrdinal(name);
			if (reader.IsDBNull(ordinal))
			{
				return null;
			}

			var value = reader.GetValue(ordinal);
			try
			{
				return new UUId((byte[]) value);
			}
			catch (InvalidCastException ex)
			{
				_logger.LogError(ex, $"Invalid type conversion to '{typeof(UUId)}' in field '{name}'");
				throw;
			}
			
		}

		public static Uuid GetPrimitivesUuid(this MySqlDataReader reader, string name)
		{
			var value = reader.GetValue(reader.GetOrdinal(name));
			try
			{
				return new Uuid((byte[]) value);
			}
			catch (InvalidCastException ex)
			{
				_logger.LogError(ex, $"Invalid type conversion to '{typeof(Uuid)}' in field '{name}'");
				throw;
			}
		}
		
		public static Uuid? GetPrimitivesUuidNullable(this MySqlDataReader reader, string name)
		{
			var ordinal = reader.GetOrdinal(name);
			if (reader.IsDBNull(ordinal))
			{
				return null;
			}

			return GetPrimitivesUuid(reader, name);
		}

		public static string GetString(this MySqlDataReader reader, string name)
		{
			if (reader.IsDbNull(name))
			{
				return null;
			}

			var value = reader.GetValue(name);

			switch (value)
			{
				case string str:
					return str;
				
				case byte[] bytes:
					// Это фиаско, но да, мы храним string в полях с типом varbinary, например devices.Description. Старый коннектор это обрабатывал корректно.
					return System.Text.Encoding.UTF8.GetString(bytes);
					
				case Guid guid:
					// Поле char(36) читается как GUID, придется сконвертировать обратно в строку :(
					return guid.ToString();
				
				default:
					return Convert.ToString(value, CultureInfo.InvariantCulture);
			}
		}

		public static byte GetByte(this MySqlDataReader reader, string name)
		{
			return TryGet(reader, name, value => Convert.ToByte(value, CultureInfo.InvariantCulture));
		}
		
		public static byte? GetByteNullable(this MySqlDataReader reader, string name)
		{
			return TryGetNullable(reader, name, value => Convert.ToByte(value, CultureInfo.InvariantCulture));
		}

		public static bool GetBoolean(this MySqlDataReader reader, string name)
		{
			return TryGet(reader, name, value => Convert.ToBoolean(value, CultureInfo.InvariantCulture));
		}

		public static bool? GetBooleanNullable(this MySqlDataReader reader, string name)
		{
			return TryGetNullable(reader, name, value => Convert.ToBoolean(value, CultureInfo.InvariantCulture));
		}

		public static int GetInt32(this MySqlDataReader reader, string name)
		{
			return TryGet(reader, name, value => Convert.ToInt32(value, CultureInfo.InvariantCulture));
		}

		public static int? GetInt32Nullable(this MySqlDataReader reader, string name)
		{
			return TryGetNullable(reader, name, value => Convert.ToInt32(value, CultureInfo.InvariantCulture));
		}

		public static long GetInt64(this MySqlDataReader reader, string name)
		{
			return TryGet(reader, name, value => Convert.ToInt64(value, CultureInfo.InvariantCulture));
		}

		public static long? GetInt64Nullable(this MySqlDataReader reader, string name)
		{
			return TryGetNullable(reader, name, value => Convert.ToInt64(value, CultureInfo.InvariantCulture));
		}

		public static double GetDouble(this MySqlDataReader reader, string name)
		{
			return TryGet(reader, name, value => Convert.ToDouble(value, CultureInfo.InvariantCulture));
		}

		public static double? GetDoubleNullable(this MySqlDataReader reader, string name)
		{
			return TryGetNullable(reader, name, value => Convert.ToDouble(value, CultureInfo.InvariantCulture));
		}

		public static decimal GetDecimal(this MySqlDataReader reader, string name)
		{
			return TryGet(reader, name, value => Convert.ToDecimal(value, CultureInfo.InvariantCulture));
		}

		public static decimal? GetDecimalNullable(this MySqlDataReader reader, string name)
		{
			return TryGetNullable(reader, name, value => Convert.ToDecimal(value, CultureInfo.InvariantCulture));
		}

		public static DateTime GetDateTime(this MySqlDataReader reader, string name)
		{
			return TryGet(reader, name, value => Convert.ToDateTime(value, CultureInfo.InvariantCulture));
		}

		public static DateTime? GetDateTimeNullable(this MySqlDataReader reader, string name)
		{
			return TryGetNullable(reader, name, value => Convert.ToDateTime(value, CultureInfo.InvariantCulture));
		}

		/// <summary>
		/// Использовать только для Nullable
		/// </summary>
		private static T? TryGetNullable<T>(MySqlDataReader reader, string name, Func<object, T> convert) where T : struct
		{
			if (reader.IsDbNull(name))
			{
				return null;
			}

			return TryGet(reader, name, convert);
		}

		private static T TryGet<T>(MySqlDataReader reader, string name, Func<object, T> convert)
		{
			var value = reader.GetValue(name);
			try
			{
				if (value is T result)
				{
					return result;
				}

				result = convert(value);

				_logger.LogTrace($"Unexpected type conversion '{value?.GetType().ToString() ?? "null"}' -> '{typeof(T)}' in field '{name}'\nCommandText: {GetCommand(reader)?.CommandText}");
				return result;
			}
			catch (InvalidCastException)
			{
				_logger.LogError($"Invalid type conversion '{value?.GetType().ToString() ?? "null"}' -> '{typeof(T)}' in field '{name}'");
				throw;
			}
			catch (FormatException)
			{
				_logger.LogError($"Invalid format on type conversion {value?.GetType().ToString() ?? "null"} -> {typeof(T)} in field '{name}'");
				throw;
			}
			catch (OverflowException)
			{
				_logger.LogError($"Overflow on type conversion {value?.GetType().ToString() ?? "null"} -> {typeof(T)} in field '{name}'");
				throw;
			}
		}

		private static MySqlCommand GetCommand(MySqlDataReader reader)
		{
			return typeof(MySqlDataReader).GetProperty("Command", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(reader) as MySqlCommand;
		}

		public static bool IsDbNull(this MySqlDataReader reader, string name)
		{
			return reader.IsDBNull(reader.GetOrdinal(name));
		}

		public static bool HasColumn(this MySqlDataReader reader, string name)
		{
			return Enumerable.Range(0, reader.FieldCount).Any(i => reader.GetName(i) == name);
		}
	}
}
