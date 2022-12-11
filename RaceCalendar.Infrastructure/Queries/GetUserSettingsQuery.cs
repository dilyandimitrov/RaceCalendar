using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Queries;

public class GetUserSettingsQuery : IGetUserSettingsQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetUserSettingsQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<UserSettings> Get(int id)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var userSettings = await conn.QuerySingleOrDefaultAsync<UserSettings>(SqlGetById, new
        {
            Id = id
        });

        return userSettings;
    }

    public async Task<UserSettings?> Get(string userId)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var userSettings = await conn.QuerySingleOrDefaultAsync<UserSettings>(SqlGetByUserId, new
        {
            UserId = userId
        });

        return userSettings;
    }

    private sealed class UserSettingsDto
    {
        public int Id { get; set; } = default!;
        public string UserId { get; set; } = default!;
        public DateTime CreatedOn { get; set; } = default!;
        public DateTime? UpdatedOn { get; set; } = default!;
        public int RacesFilterYear { get; set; } = default!;
        public bool? RacesShowPassed { get; set; } = default!;
    }

    private const string SqlGetById = $@"
SELECT [Id] AS {nameof(UserSettingsDto.Id)}
      ,[UserId] AS {nameof(UserSettingsDto.UserId)}
      ,[CreatedOn] AS {nameof(UserSettingsDto.CreatedOn)}
      ,[UpdatedOn] AS {nameof(UserSettingsDto.UpdatedOn)}
      ,[RacesFilterYear] AS {nameof(UserSettingsDto.RacesFilterYear)}
      ,[RacesShowPassed] AS {nameof(UserSettingsDto.RacesShowPassed)}
FROM [dbo].[UserSettings]
WHERE Id = @Id
";

    private const string SqlGetByUserId = $@"
SELECT [Id] AS {nameof(UserSettingsDto.Id)}
      ,[UserId] AS {nameof(UserSettingsDto.UserId)}
      ,[CreatedOn] AS {nameof(UserSettingsDto.CreatedOn)}
      ,[UpdatedOn] AS {nameof(UserSettingsDto.UpdatedOn)}
      ,[RacesFilterYear] AS {nameof(UserSettingsDto.RacesFilterYear)}
      ,[RacesShowPassed] AS {nameof(UserSettingsDto.RacesShowPassed)}
FROM [dbo].[UserSettings]
WHERE UserId = @UserId
";
}
