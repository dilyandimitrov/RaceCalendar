using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Queries;

public class GetRaceRequestsQuery : IGetRaceRequestsQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetRaceRequestsQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IEnumerable<RaceRequest>> Get(bool includeProcessed = false)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var raceRequests = (await conn.QueryAsync<RaceRequestDto>(Sql, new
        {
            IncludeProcessed = includeProcessed
        }));

        return raceRequests.
            Select(r => new RaceRequest(
                r.Id,
                r.Type,
                r.Name,
                r.NameId,
                r.StartDate,
                r.Text,
                r.Source,
                r.ClientIP,
                r.CreatedOn,
                r.ContactInfo,
                r.IsProcessed))
            .ToList();
    }

    private sealed class RaceRequestDto
    {
        public int Id { get; set; } = default!;
        public RaceTypes Type { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string NameId { get; set; } = default!;
        public DateTime? StartDate { get; set; } = default!;
        public string Text { get; set; } = default!;
        public string Source { get; set; } = default!;
        public string ClientIP { get; set; } = default!;
        public DateTime CreatedOn { get; set; } = default!;
        public string ContactInfo { get; set; } = default!;
        public bool IsProcessed { get; set; } = default!;
    }

    private const string Sql = @$"
SELECT [Id] AS {nameof(RaceRequestDto.Id)}
      ,[Type] AS {nameof(RaceRequestDto.Type)}
      ,[Name] AS {nameof(RaceRequestDto.Name)}
      ,[NameId] AS {nameof(RaceRequestDto.NameId)}
      ,[StartDate] AS {nameof(RaceRequestDto.StartDate)}
      ,[Text] AS {nameof(RaceRequestDto.Text)}
      ,[Source] AS {nameof(RaceRequestDto.Source)}
      ,[ClientIP] AS {nameof(RaceRequestDto.ClientIP)}
      ,[CreatedOn] AS {nameof(RaceRequestDto.CreatedOn)}
      ,[ContactInfo] AS {nameof(RaceRequestDto.ContactInfo)}
      ,[IsProcessed] AS {nameof(RaceRequestDto.IsProcessed)}
FROM [dbo].[RaceRequests]
WHERE @IncludeProcessed = 1 OR IsProcessed = @IncludeProcessed";
}
