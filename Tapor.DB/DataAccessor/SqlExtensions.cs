using Dodo.Tools.DB.MySql;
using MySqlConnector;

public static class SqlExtensions
{
    public static string ToSqlList(this IEnumerable<UUId> uuids)
    {
        return string.Join(",", uuids.Select(x => $"x'{x}'"));
    }
    
    public static string ToSqlList(this IEnumerable<Uuid> uuids)
    {
        return string.Join(",", uuids.Select(x => $"x'{x}'"));
    }

    public static string ToSqlList(this IEnumerable<int> values)
    {
        return string.Join(",", values);
    }
    
    public static string ToSqlList(this IEnumerable<DateTime> values)
    {
        return string.Join(",", values.Select(FormatMySqlDate));
    }
    
    public static string ToSqlList(this IEnumerable<MySqlParameter> parameters)
    {
        var parameterNames = parameters.Select(p => $"@{p.ParameterName}");
        return string.Join(",", parameterNames);
    }
    
    private static string FormatMySqlDate(DateTime date)
        => $"'{date.Year}-{date.Month}-{date.Day}'";
}