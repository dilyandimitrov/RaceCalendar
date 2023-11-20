using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class UpdateRaceCommand : IUpdateRaceCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public UpdateRaceCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(Race race)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Id = race.Id,
            Name = race.Name,
            NameId = race.NameId,
            Country = race.Country,
            City = race.City,
            StartDate = race.StartDate,
            EndDate = race.EndDate,
            Link = race.Link,
            Tags = race.Tags,
            Terrain = race.Terrain,
            Special = race.Special,
            Cancelled = race.Cancelled,
            Latitude = race.Latitude,
            Longitude = race.Longitude
        });
    }

    private const string Sql = @"
UPDATE [dbo].[Races]
SET [Name] = @Name,
    [NameId] = @NameId,
    [Country] = @Country,
    [City] = @City,
    [StartDate] = @StartDate,
    [EndDate] = @EndDate,
    [Link] = @Link,
    [Tags] = @Tags,
    [Terrain] = @Terrain,
    [Special] = @Special,
    [Cancelled] = @Cancelled,
    [Latitude] = @Latitude,
    [Longitude] = @Longitude
WHERE Id = @Id";
}
