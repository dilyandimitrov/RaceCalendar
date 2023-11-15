using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class UpdateEventCommand : IUpdateEventCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public UpdateEventCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(Event @event)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Id = @event.Id,
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
            ModifiedOn = DateTime.UtcNow
        });
    }

    private const string Sql = @"
UPDATE [dbo].[Events]
SET [Name] = @Name,
    [Description] = @Description,
    [City] = @City,
    [Latitude] = @Latitude,
    [Longitude] = @Longitude,
    [StartDate] = @StartDate,
    [StartTime] = @StartTime,
    [Distance] = @Distance,
    [ElevationGain] = @ElevationGain,
    [Link] = @Link,
    [Terrain] = @Terrain,
    [Cancelled] = @Cancelled,
    [IsPublic] = @IsPublic,
    [MaxParticipants] = @MaxParticipants,
    [Contact] = @Contact,
    [ModifiedOn] = @ModifiedOn
WHERE Id = @Id";
}
