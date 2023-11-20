using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class CreateRaceRequestCommand : ICreateRaceRequestCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public CreateRaceRequestCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(RaceRequest raceRequest)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Type = raceRequest.Type,
            Name = raceRequest.Name,
            NameId = raceRequest.NameId,
            StartDate = raceRequest.StartDate,
            Text = raceRequest.Text,
            Source = raceRequest.Source,
            ClientIP = raceRequest.ClientIP,
            CreatedOn = DateTime.UtcNow,
            ContactInfo = raceRequest.ContactInfo,
            IsProcessed = raceRequest.IsProcessed,
        });
    }

    private const string Sql = @"
INSERT INTO [dbo].[RaceRequests]
        ([Type]
        ,[Name]
        ,[NameId]
        ,[StartDate]
        ,[Text]
        ,[Source]
        ,[ClientIP]
        ,[CreatedOn]
        ,[ContactInfo]
        ,[IsProcessed])
VALUES
        (@Type
        ,@Name
        ,@NameId
        ,@StartDate
        ,@Text
        ,@Source
        ,@ClientIP
        ,@CreatedOn
        ,@ContactInfo
        ,@IsProcessed)";
}
