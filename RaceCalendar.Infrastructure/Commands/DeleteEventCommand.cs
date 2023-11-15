using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Commands;

public class DeleteEventCommand : IDeleteEventCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public DeleteEventCommand(IConnectionProvider connectionProvider)
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
DELETE dbo.[Events]
WHERE Id = @Id";
}
