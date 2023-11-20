using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class CreateUserRaceCommand : ICreateUserRaceCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public CreateUserRaceCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(UserRace userRace)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            UserId = userRace.UserId,
            Type = userRace.Type,
            RaceId = userRace.RaceId,
            RaceDistanceId = userRace.RaceDistanceId,
            CreatedOn = DateTime.UtcNow,
            Position = userRace.Position,
            Result = userRace.Result,
            ResultType = userRace.ResultType,
            Description = userRace.Description
        });
    }

    private const string Sql = @$"
INSERT INTO [dbo].[UserRaces]
    ([UserId]
    ,[Type]
    ,[RaceId]
    ,[RaceDistanceId]
    ,[CreatedOn]
    ,[Position]
    ,[Result]
    ,[ResultType]
    ,[Description])
VALUES
    (@UserId
    ,@Type
    ,@RaceId
    ,@RaceDistanceId
    ,@CreatedOn
    ,@Position
    ,@Result
    ,@ResultType
    ,@Description)
";
}
