using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Commands;

public class UpdateRaceDistanceCommand : IUpdateRaceDistanceCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public UpdateRaceDistanceCommand(IConnectionProvider connectionProvider)
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
            ResultsLink = raceDistance.ResultsLink,
            Latitude = raceDistance.Latitude,
            Longitude = raceDistance.Longitude
        });
    }

    private const string Sql = @"
UPDATE [dbo].[RaceDistances]
SET [RaceId] = @RaceId,
    [Name] = @Name,
    [Distance] = @Distance,
    [StartDate] = @StartDate,
    [StartTime] = @StartTime,
    [UnconfirmedDate] = @UnconfirmedDate,
    [ElevationGain] = @ElevationGain,
    [Price] = @Price,
    [Link] = @Link,
    [ResultsLink] = @ResultsLink,
    [Latitude] = @Latitude,
    [Longitude] = @Longitude
WHERE Id = @Id";
}
