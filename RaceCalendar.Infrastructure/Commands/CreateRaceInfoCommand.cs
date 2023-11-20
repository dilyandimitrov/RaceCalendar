using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class CreateRaceInfoCommand : ICreateRaceInfoCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public CreateRaceInfoCommand(IConnectionProvider connectionProvider)
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
            Value = raceInfo.Value,
        });
    }

    private const string Sql = @"
INSERT INTO [dbo].[RaceInfo]
        ([Id]
        ,[RaceId]
        ,[RaceDistanceId]
        ,[Name]
        ,[Value])
VALUES
        (@Id
        ,@RaceId
        ,@RaceDistanceId
        ,@Name
        ,@Value)";
}
