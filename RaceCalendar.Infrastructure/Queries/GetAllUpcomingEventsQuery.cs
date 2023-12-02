using Dapper;
using Microsoft.Data.SqlClient;
using NodaTime;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Queries;

public class GetAllUpcomingEventsQuery : IGetAllUpcomingEventsQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetAllUpcomingEventsQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IEnumerable<Event>> QueryAsync(bool showPublicOnly = false)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var result = await conn.QueryAsync<EventDto>(Sql,
            new
            {
                ShowPublicOnly = showPublicOnly
            });

        return result
            .Select(e => new Event(
                e.Id,
                e.Name,
                e.Description,
                e.City,
                e.Latitude,
                e.Longitude,
                e.StartDate,
                e.StartTime,
                e.Distance,
                e.ElevationGain,
                e.Link,
                e.Terrain,
                e.Cancelled,
                e.IsPublic,
                e.MaxParticipants,
                e.Contact,
                e.CreatedBy,
                e.CreatedOn,
                e.ModifiedOn,
                e.CommentsCount))
            .ToList();
    }

    private const string Sql = $@"
DECLARE @Now DATETIME = SYSDATETIMEOFFSET() AT TIME ZONE 'FLE Standard Time'

SELECT *
FROM dbo.[Events] e
OUTER APPLY 
( 
   SELECT COUNT(*) AS {nameof(EventDto.CommentsCount)}
   FROM dbo.[EventComments] ec
   WHERE ec.EventId = e.Id
) c
WHERE CAST(e.StartDate AS DATETIME) + CAST(e.StartTime AS DATETIME) >= @Now AND
(@ShowPublicOnly = 0 OR e.IsPublic = @ShowPublicOnly)
ORDER BY e.StartDate, e.StartTime";

    private sealed class EventDto
    {
        public long Id { get; init; }
        public string Name { get; init; } = default!;
        public string? Description { get; init; }
        public string City { get; init; } = default!;
        public decimal Latitude { get; init; }
        public decimal Longitude { get; init; }
        public LocalDate StartDate { get; init; }
        public TimeSpan StartTime { get; init; }
        public double? Distance { get; init; }
        public int? ElevationGain { get; set; }
        public string? Link { get; init; }
        public Terrains? Terrain { get; init; }
        public Cancelled? Cancelled { get; init; }
        public bool IsPublic { get; init; }
        public int MaxParticipants { get; init; }
        public string? Contact { get; init; }
        public string CreatedBy { get; init; } = default!;
        public DateTime CreatedOn { get; init; }
        public DateTime? ModifiedOn { get; init; }
        public long? CommentsCount { get; set; }
    }
}
