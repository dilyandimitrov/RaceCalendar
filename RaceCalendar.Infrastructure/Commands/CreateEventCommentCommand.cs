using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class CreateEventCommentCommand : ICreateEventCommentCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public CreateEventCommentCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(EventComment eventComment)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            EventId = eventComment.EventId,
            ParentCommentId = eventComment.ParentCommentId,
            Text = eventComment.Text,
            CreatedBy = eventComment.CreatedBy,
            CreatedOn = eventComment.CreatedOn
        });
    }

    private const string Sql = @"
INSERT INTO [dbo].[EventComments]
        ([EventId]
        ,[ParentCommentId]
        ,[Text]
        ,[CreatedBy]
        ,[CreatedOn])
VALUES
        (@EventId
        ,@ParentCommentId
        ,@Text
        ,@CreatedBy
        ,@CreatedOn)";
}
