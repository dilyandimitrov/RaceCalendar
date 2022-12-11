using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Infrastructure.Persistence;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Commands;

public class CreateImportDataCommand : ICreateImportDataCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public CreateImportDataCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(ImportData importData)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            UploadedOn = DateTime.UtcNow,
            Name = importData.Name,
            SizeInBytes = importData.SizeInBytes,
            ContentType = importData.ContentType,
            Data = importData.Data,
            Notes = importData.Notes
        });
    }

    private const string Sql = @"
INSERT INTO [dbo].[ImportData]
        ([UploadedOn]
        ,[Name]
        ,[SizeInBytes]
        ,[ContentType]
        ,[Data]
        ,[Notes])
VALUES
        (@UploadedOn
        ,@Name
        ,@SizeInBytes
        ,@ContentType
        ,@Data
        ,@Notes)";
}
