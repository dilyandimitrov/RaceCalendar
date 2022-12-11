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

public class GetUserRacesByUserQuery : IGetUserRacesByUserQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetUserRacesByUserQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IEnumerable<UserRace>> Get(string userId)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var userRaces = await conn.QueryAsync<UserRaceDto>(Sql, new
        {
            UserId = userId
        });

        return userRaces
            .Select(r => new UserRace(
                r.Id,
                r.UserId,
                r.Type,
                r.RaceId,
                r.RaceDistanceId,
                r.CreatedOn,
                r.Result,
                r.ResultType,
                r.Position,
                r.Description))
            .ToList();
    }

    private sealed class UserRaceDto
    {
        public int Id { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public UserRaceTypes Type { get; set; } = default!;
        public int RaceId { get; set; } = default!;
        public int? RaceDistanceId { get; set; } = default!;
        public DateTime CreatedOn { get; set; } = default!;
        public string Result { get; set; } = default!;
        public ResultTypes? ResultType { get; set; } = default!;
        public string Position { get; set; } = default!;
        public string Description { get; set; } = default!;
    }

    private const string Sql = $@"
SELECT [Id] AS {nameof(UserRaceDto.Id)}
      ,[UserId] AS {nameof(UserRaceDto.UserId)}
      ,[Type] AS {nameof(UserRaceDto.Type)}
      ,[RaceId] AS {nameof(UserRaceDto.RaceId)}
      ,[RaceDistanceId] AS {nameof(UserRaceDto.RaceDistanceId)}
      ,[CreatedOn] AS {nameof(UserRaceDto.CreatedOn)}
      ,[Position] AS {nameof(UserRaceDto.Position)}
      ,[Result] AS {nameof(UserRaceDto.Result)}
      ,[ResultType] AS {nameof(UserRaceDto.ResultType)}
      ,[Description] AS {nameof(UserRaceDto.Description)}
FROM [dbo].[UserRaces]
WHERE UserId = @UserId";
}
