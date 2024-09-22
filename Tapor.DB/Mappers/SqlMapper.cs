using System.Data;
using System.Data.Common;
using Tapor.Shared.Dtos;

namespace Tapor.DB.Mappers;

public static partial class SqlMapper
{
    public static async Task<IssueDto> IssueToDto(this DbDataReader reader, CancellationToken ct)
    {
        return new IssueDto
        {
            Id = reader.GetInt64("Id"),
            Assignee = !await reader.IsDBNullAsync("Assignee", ct) ? reader.GetGuid("Assignee") : null,
            Reporter = !await reader.IsDBNullAsync("Reporter", ct) ? reader.GetGuid("Reporter") : null,
            Summary = reader.GetString("Summary"),
            Description = reader.GetString("Description"),
            Type = reader.GetInt32("Type"),
            Status = reader.GetInt32("Status"),
            Priority = reader.GetInt32("Priority"),
            Size = reader.GetInt32("Size"),
            EstimatedTime = !await reader.IsDBNullAsync("EstimatedTime", ct)
                ? reader.GetDecimal("EstimatedTime")
                : null,
            DueDate = !await reader.IsDBNullAsync("DueDate", ct) ? reader.GetDateTime("DueDate") : null,
        };
    }
}