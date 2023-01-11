using Dapper;
using Microsoft.Data.SqlClient;
using NodaTime;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Queries;

public class GetRaceDistancesQuery : IGetRaceDistancesQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetRaceDistancesQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IEnumerable<RaceDistance>> Get(int raceId, ISet<int>? raceDistanceIds = null)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var distances = (await conn.QueryAsync<RaceDistanceDto>(Sql,
            new
            {
                RaceId = raceId,
                RaceDistanceIds = raceDistanceIds,
                RaceDistanceIdsCount = raceDistanceIds?.Count
            }));

        return distances
            .Select(d => new RaceDistance(
                d.Id,
                d.RaceId,
                d.Name,
                d.Distance,
                d.StartDate,
                d.StartTime,
                d.UnconfirmedDate,
                d.ElevationGain,
                d.Price,
                d.Link,
                d.ResultsLink))
            .ToList();
    }

    private sealed class RaceDistanceDto
    {
        public int Id { get; set; } = default;
        public int RaceId { get; set; } = default;
        public string Name { get; init; } = default!;
        public double? Distance { get; init; } = default!;
        public LocalDate? StartDate { get; init; } = default!;
        public TimeSpan? StartTime { get; init; } = default!;
        public int? UnconfirmedDate { get; init; } = default!;
        public int? ElevationGain { get; init; } = default!;
        public string Price { get; init; } = default!;
        public string Link { get; init; } = default!;
        public string ResultsLink { get; init; } = default!;
    }

    private const string Sql = $@"
SELECT [Id] AS {nameof(RaceDistanceDto.Id)}
      ,[RaceId] AS {nameof(RaceDistanceDto.RaceId)}
      ,[Name] AS {nameof(RaceDistanceDto.Name)}
      ,[Distance] AS {nameof(RaceDistanceDto.Distance)}
      ,[StartDate] AS {nameof(RaceDistanceDto.StartDate)}
      ,[StartTime] AS {nameof(RaceDistanceDto.StartTime)}
      ,[UnconfirmedDate] AS {nameof(RaceDistanceDto.UnconfirmedDate)}
      ,[ElevationGain] AS {nameof(RaceDistanceDto.ElevationGain)}
      ,[Price] AS {nameof(RaceDistanceDto.Price)}
      ,[Link] AS {nameof(RaceDistanceDto.Link)}
      ,[ResultsLink] AS {nameof(RaceDistanceDto.ResultsLink)}
FROM [dbo].[RaceDistances]
WHERE RaceId = @RaceId AND 
(Id IN @RaceDistanceIds OR @RaceDistanceIdsCount IS NULL)
";
}
