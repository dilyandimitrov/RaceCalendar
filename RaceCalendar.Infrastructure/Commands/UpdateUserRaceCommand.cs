using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class UpdateUserRaceCommand : IUpdateUserRaceCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public UpdateUserRaceCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(UserRace userRace)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Id = userRace.Id,
            UserId = userRace.UserId,
            Type = userRace.Type,
            RaceId = userRace.RaceId,
            RaceDistanceId = userRace.RaceDistanceId,
            Position = userRace.Position,
            Result = userRace.Result,
            ResultType = userRace.ResultType,
            Description = userRace.Description
        });
    }

    private const string Sql = $@"
UPDATE dbo.UserRaces
SET [UserId] = @UserId,
    [Type] = @Type,
    [RaceId] = @RaceId,
    [RaceDistanceId] = @RaceDistanceId,
    [Position] = @Position,
    [Result] = @Result,
    [ResultType] = @ResultType,
    [Description] = @Description
WHERE Id = @Id
";
}
