using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Responses;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Queries;

public class GetAllUsersQuery : IGetAllUsersQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetAllUsersQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IEnumerable<GetAllUsersResponse>> Get()
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var multiQuery = await conn.QueryMultipleAsync(Sql);

        var users = multiQuery.Read<GetAllUsersResponse>().ToList();
        var userRaces = multiQuery.Read<GetAllUserRacesResponse>().ToList();

        users.ForEach(user => user.Races = userRaces.Where(ur => ur.UserId == user.Id).ToList());

        return users;
    }

    private const string Sql = @$"
SELECT 
    Id AS {nameof(GetAllUsersResponse.Id)},
    Email AS {nameof(GetAllUsersResponse.Email)},
    FirstName AS {nameof(GetAllUsersResponse.FirstName)},
    LastName AS {nameof(GetAllUsersResponse.LastName)},
    CreatedOn AS {nameof(GetAllUsersResponse.CreatedOn)},
    UpdatedOn AS {nameof(GetAllUsersResponse.UpdatedOn)}
FROM [RaceCalendar].[dbo].[Users.Users]
ORDER BY CreatedOn

SELECT 
    ur.UserId AS {nameof(GetAllUserRacesResponse.UserId)},
    r.Name AS {nameof(GetAllUserRacesResponse.Name)},
    r.NameId AS {nameof(GetAllUserRacesResponse.NameId)},
    r.StartDate AS {nameof(GetAllUserRacesResponse.StartDate)},
    rd.Distance AS {nameof(GetAllUserRacesResponse.Distance)}
FROM [RaceCalendar].[dbo].[UserRaces] ur
INNER JOIN [RaceCalendar].[dbo].[Races] r ON r.Id = ur.RaceId
INNER JOIN [RaceCalendar].[dbo].[RaceDistances] rd ON rd.Id = ur.RaceDistanceId
ORDER BY r.StartDate
";

    private sealed class RaceDistanceCountDto
    {
        public int RaceId { get; set; } = default!;
        public int Count { get; set; } = default!;
    }
}
