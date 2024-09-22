using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
using Tapor.DB.Scripts.Issue;
using Tapor.Shared.Dtos;
using Tapor.Shared.Interfaces;
using Tapor.Shared.Options;

namespace Tapor.DB;

public class IssueRepository : IIssueRepository
{
    private readonly ILogger<IssueRepository> _logger;
    private readonly string _connectionString;
    private readonly int _commandTimeout = TimeSpan.FromSeconds(5).Seconds;

    public IssueRepository(IOptions<ConnectionStringOptions> config, ILogger<IssueRepository> logger)
    {
        _connectionString = config.Value.AppConnection;
        _logger = logger;
    }

    public async Task<long> Create(IssueDto dto, CancellationToken ct)
    {
        // создаем подключение к базе
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.CommandTimeout = _commandTimeout;

        // формируем запрос
        command.CommandText = Sql.IssueCreate;
        // command.CommandType = CommandType.Text;
        AddParameters(command, dto);
        // command.Transaction = await connection.BeginTransactionAsync(IsolationLevel.ReadUncommitted, ct);

        // сохраняем в базу, получаем айди созданной записи
        var issueObj = await command.ExecuteScalarAsync(ct);
        // await command.Transaction.CommitAsync(ct);
        var issueId = long.Parse(issueObj.ToString());

        // добавить в логгер запись
        _logger.LogInformation("Issue {IssueId} created", issueId);

        return issueId;
    }

    public async IAsyncEnumerable<IssueDto> GetList([EnumeratorCancellation] CancellationToken ct)
    {
        // создаем подключение к базе
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(ct);

        await using var command = connection.CreateCommand();
        command.CommandTimeout = _commandTimeout;

        // формируем запрос
        command.CommandText = "select * from Issue;";

        // получаем записи в ридер
        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            yield return new IssueDto
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

    private static void AddParameters(MySqlCommand command, IssueDto dto)
    {
        command.Parameters.AddWithValue("assignee", dto.Assignee);
        command.Parameters.AddWithValue("reporter", dto.Reporter);
        command.Parameters.AddWithValue("summary", dto.Summary);
        command.Parameters.AddWithValue("description", dto.Description);
        command.Parameters.AddWithValue("type", dto.Type);
        command.Parameters.AddWithValue("status", dto.Status);
        command.Parameters.AddWithValue("priority", dto.Priority);
        command.Parameters.AddWithValue("size", dto.Size);
        command.Parameters.AddWithValue("estimatedTime", dto.EstimatedTime);
        command.Parameters.AddWithValue("createDate", DateTime.UtcNow);
        command.Parameters.AddWithValue("dueDate", dto.DueDate);
    }
}
