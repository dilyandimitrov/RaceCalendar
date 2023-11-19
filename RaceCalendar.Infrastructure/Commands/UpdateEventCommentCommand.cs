using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class UpdateEventCommentCommand : IUpdateEventCommentCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public UpdateEventCommentCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(EventComment eventComment)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Id = eventComment.Id,
            Text = eventComment.Text,
            ModifiedOn = DateTime.UtcNow
        });
    }

    private const string Sql = @"
UPDATE [dbo].[EventComments]
SET [Text] = @Text,
    [ModifiedOn] = @ModifiedOn
WHERE Id = @Id";
}
