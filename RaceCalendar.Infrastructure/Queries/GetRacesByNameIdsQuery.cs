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

public class GetRacesByNameIdsQuery : IGetRacesByNameIdsQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetRacesByNameIdsQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IReadOnlyDictionary<string, Race>> Get(ISet<string> nameIds)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var races = (await conn.QueryAsync<RaceDto>(Sql, new
        {
            NameIds = nameIds
        }));

        return races
            .ToDictionary(r => r.NameId, r => new Race(
                r.Id, 
                r.Name, 
                r.NameId, 
                r.Country, 
                r.City, 
                r.StartDate, 
                r.EndDate, 
                r.Link, 
                r.Tags, 
                r.Cancelled, 
                r.Terrain, 
                r.Special));
    }

    private sealed class RaceDto
    {
        public int Id { get; set; } = default;
        public string Name { get; init; } = default!;
        public string NameId { get; init; } = default!;
        public string Country { get; init; } = default!;
        public string City { get; init; } = default!;
        public LocalDate? StartDate { get; init; } = default!;
        public LocalDate? EndDate { get; init; } = default!;
        public string Link { get; init; } = default!;
        public string Tags { get; init; } = default!;
        public Terrains Terrain { get; init; } = default!;
        public Specials Special { get; init; } = default!;
        public Cancelled Cancelled { get; init; } = default!;
    }

    private const string Sql = @$"
SELECT [Id] AS {nameof(RaceDto.Id)}
    ,[Name] AS {nameof(RaceDto.Name)}
    ,[NameId] AS {nameof(RaceDto.NameId)}
    ,[Country] AS {nameof(RaceDto.Country)}
    ,[City] AS {nameof(RaceDto.City)}
    ,[StartDate] AS {nameof(RaceDto.StartDate)}
    ,[EndDate] AS {nameof(RaceDto.EndDate)}
    ,[Link] AS {nameof(RaceDto.Link)}
    ,[Tags] AS {nameof(RaceDto.Tags)}
    ,[Terrain] AS {nameof(RaceDto.Terrain)}
    ,[Special] AS {nameof(RaceDto.Special)}
    ,[Cancelled] AS {nameof(RaceDto.Cancelled)}
FROM [dbo].[Races]
WHERE NameId IN @NameIds";
}
