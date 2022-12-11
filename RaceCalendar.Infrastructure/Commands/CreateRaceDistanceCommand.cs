using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Commands;

public class CreateRaceDistanceCommand : ICreateRaceDistanceCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public CreateRaceDistanceCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(RaceDistance raceDistance)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Id = raceDistance.Id,
            RaceId = raceDistance.RaceId,
            Name = raceDistance.Name,
            Distance = raceDistance.Distance,
            StartDate = raceDistance.StartDate,
            StartTime = raceDistance.StartTime,
            UnconfirmedDate = raceDistance.UnconfirmedDate,
            ElevationGain = raceDistance.ElevationGain,
            Price = raceDistance.Price,
            Link = raceDistance.Link,
            ResultsLink = raceDistance.ResultsLink
        });
    }

    private const string Sql = @"
INSERT INTO [dbo].[RaceDistances]
        ([Id]
        ,[RaceId]
        ,[Name]
        ,[Distance]
        ,[StartDate]
        ,[StartTime]
        ,[UnconfirmedDate]
        ,[ElevationGain]
        ,[Price]
        ,[Link]
        ,[ResultsLink])
VALUES
        (@Id
        ,@RaceId
        ,@Name
        ,@Distance
        ,@StartDate
        ,@StartTime
        ,@UnconfirmedDate
        ,@ElevationGain
        ,@Price
        ,@Link
        ,@ResultsLink)";
}
