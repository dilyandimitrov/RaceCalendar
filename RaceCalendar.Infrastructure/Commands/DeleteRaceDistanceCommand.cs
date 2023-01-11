using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Commands;

public class DeleteRaceDistanceCommand : IDeleteRaceDistanceCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public DeleteRaceDistanceCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(ISet<int> ids)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Ids = ids
        });
    }

    private const string Sql = @"
DELETE dbo.RaceDistances
WHERE Id IN @Ids";
}
