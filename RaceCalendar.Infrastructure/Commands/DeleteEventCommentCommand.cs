using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class DeleteEventCommentCommand : IDeleteEventCommentCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public DeleteEventCommentCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(long id)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Id = id
        });
    }

    private const string Sql = @"
-- delete replies if any
DELETE dbo.[EventComments]
WHERE ParentCommentId = @Id;

-- delete the parent comment
DELETE dbo.[EventComments]
WHERE Id = @Id";
}
