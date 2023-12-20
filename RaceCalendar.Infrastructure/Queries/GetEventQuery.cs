using Dapper;
using Microsoft.Data.SqlClient;
using NodaTime;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Queries;

public class GetEventQuery : IGetEventQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetEventQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<Event?> QueryAsync(long id)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var result = await conn.QuerySingleOrDefaultAsync<EventDto>(Sql,
            new
            {
                Id = id
            });

        if (result is null)
        {
            return null;
        }

        return new Event(
                result.Id,
                result.Name,
                result.Description,
                result.City,
                result.Latitude,
                result.Longitude,
                result.StartDate,
                result.StartTime,
                result.Distance,
                result.ElevationGain,
                result.Link,
                result.Terrain,
                result.Cancelled,
                result.IsPublic,
                result.MaxParticipants,
                result.Contact,
                result.CreatedBy,
                result.CreatedOn,
                result.ModifiedOn,
                null,
                result.VisitorsCount);
    }

    private const string Sql = $@"
SELECT *
FROM dbo.[Events]
WHERE Id = @Id
";

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
        public long? VisitorsCount { get; init; } = default!;
        public DateTime CreatedOn { get; init; }
        public DateTime? ModifiedOn { get; init; }
    }
}
