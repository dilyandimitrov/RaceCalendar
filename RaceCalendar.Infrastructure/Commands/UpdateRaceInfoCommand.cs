using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Commands;

public class UpdateRaceInfoCommand : IUpdateRaceInfoCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public UpdateRaceInfoCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(RaceInfo raceInfo)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Id = raceInfo.Id,
            RaceId = raceInfo.RaceId,
            RaceDistanceId = raceInfo.RaceDistanceId,
            Name = raceInfo.Name,
            Value = raceInfo.Value
        });
    }

    private const string Sql = @"
UPDATE [dbo].[RaceInfo]
SET [RaceId] = @RaceId,
    [RaceDistanceId] = @RaceDistanceId,
    [Name] = @Name,
    [Value] = @Value
WHERE Id = @Id";
}
