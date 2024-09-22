using System;
using System.Linq;

namespace Dodo.Tools.DB.MySql
{
	public static class DataReaderExtensions
	{
		public static bool HasColumn(this System.Data.IDataReader reader, string column)
		{
			return Enumerable.Range(0, reader.FieldCount).Any(i => reader.GetName(i) == column);
		}

		public static bool HasValue(this System.Data.IDataReader reader, string column)
		{
			return HasColumn(reader, column) && reader[column] != DBNull.Value;
		}
	}
}