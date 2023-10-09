using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Commands;

public class CreateRaceCommand : ICreateRaceCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public CreateRaceCommand(IConnectionProvider connectionProvider)
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
            Country = race.Country ?? "България",
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
INSERT INTO [dbo].[Races]
        ([Id]
        ,[Name]
        ,[NameId]
        ,[Country]
        ,[City]
        ,[StartDate]
        ,[EndDate]
        ,[Link]
        ,[Tags]
        ,[Terrain]
        ,[Special]
        ,[Cancelled]
        ,[Latitude]
        ,[Longitude])
VALUES
        (@Id
        ,@Name
        ,@NameId
        ,@Country
        ,@City
        ,@StartDate
        ,@EndDate
        ,@Link
        ,@Tags
        ,@Terrain
        ,@Special
        ,@Cancelled
        ,@Latitude
        ,@Longitude)";
}
