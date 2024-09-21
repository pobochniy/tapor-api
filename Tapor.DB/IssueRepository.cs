using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MySqlConnector;
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
        command.CommandText =
            @"insert into Issue (`Assignee`, Reporter, Summary, Description, Type, Status, Priority, Size, EstimatedTime, CreateDate, DueDate)
                    values (@assignee, @reporter, @summary, @description, @type, @status, @priority, @size, @estimatedTime, @createDate, @dueDate);
                select LAST_INSERT_ID();
            ";
        command.Parameters.AddWithValue("assignee", dto.Assignee);
        command.Parameters.Add(new MySqlParameter("reporter", MySqlDbType.String) {Value = dto.Reporter});
        command.Parameters.Add(new MySqlParameter("summary", MySqlDbType.String) {Value = dto.Summary});
        command.Parameters.Add(new MySqlParameter("description", MySqlDbType.String) {Value = dto.Description});
        command.Parameters.AddWithValue("type", dto.Type);
        command.Parameters.Add(new MySqlParameter("status", MySqlDbType.Int32) {Value = dto.Status});
        command.Parameters.Add(new MySqlParameter("priority", MySqlDbType.Int32) {Value = dto.Priority});
        command.Parameters.Add(new MySqlParameter("size", MySqlDbType.Int32) {Value = dto.Size});
        command.Parameters.Add(new MySqlParameter("estimatedTime", MySqlDbType.Decimal) {Value = dto.EstimatedTime});
        command.Parameters.AddWithValue("createDate", DateTime.UtcNow);
        command.Parameters.Add(new MySqlParameter("dueDate", MySqlDbType.DateTime) {Value = dto.DueDate});

        // сохраняем в базу, получаем айди созданной записи
        var issueObj = await command.ExecuteScalarAsync(ct);
        var issueId = long.Parse(issueObj.ToString());

        // добавить в логгер запись
        _logger.LogInformation("Issue {IssueId} created", issueId);

        return issueId;
    }

    public async IAsyncEnumerable<IssueDto> GetList([EnumeratorCancellation] CancellationToken ct)
    {
        // создаем подключение к базе
        await using var connection = new MySqlConnection(_connectionString);
        await connection.OpenAsync(ct).ConfigureAwait(false);

        await using var command = connection.CreateCommand();
        command.CommandTimeout = _commandTimeout;

        // формируем запрос
        command.CommandText = "select * from Issue;";

        // получаем записи в ридер
        await using var reader = await command.ExecuteReaderAsync(ct).ConfigureAwait(false);
        while (await reader.ReadAsync(ct).ConfigureAwait(false))
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

    private string ToDbDate(DateTime? dateTime)
    {
        return dateTime?.ToString("yyyy-MM-dd HH:mm:ss");
    }
}
