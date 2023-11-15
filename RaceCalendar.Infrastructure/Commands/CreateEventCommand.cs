using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class CreateEventCommand : ICreateEventCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public CreateEventCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(Event @event)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Name = @event.Name,
            Description = @event.Description,
            City = @event.City,
            Latitude = @event.Latitude,
            Longitude = @event.Longitude,
            StartDate = @event.StartDate,
            StartTime = @event.StartTime,
            Distance = @event.Distance,
            ElevationGain = @event.ElevationGain,
            Link = @event.Link,
            Terrain = @event.Terrain,
            Cancelled = @event.Cancelled,
            IsPublic = @event.IsPublic,
            MaxParticipants = @event.MaxParticipants,
            Contact = @event.Contact,
            CreatedBy = @event.CreatedBy,
            CreatedOn = DateTime.UtcNow
        });
    }

    private const string Sql = @"
INSERT INTO [dbo].[Events]
        ([Name]
        ,[Description]
        ,[City]
        ,[Latitude]
        ,[Longitude]
        ,[StartDate]
        ,[StartTime]
        ,[Distance]
        ,[ElevationGain]
        ,[Link]
        ,[Terrain]
        ,[Cancelled]
        ,[IsPublic]
        ,[MaxParticipants]
        ,[Contact]
        ,[CreatedBy]
        ,[CreatedOn]
        )
VALUES
        (@Name
        ,@Description
        ,@City
        ,@Latitude
        ,@Longitude
        ,@StartDate
        ,@StartTime
        ,@Distance
        ,@ElevationGain
        ,@Link
        ,@Terrain
        ,@Cancelled
        ,@IsPublic
        ,@MaxParticipants
        ,@Contact
        ,@CreatedBy
        ,@CreatedOn)";
}
