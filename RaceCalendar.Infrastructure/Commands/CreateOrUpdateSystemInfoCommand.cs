using Dapper;
using Microsoft.Data.SqlClient;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Infrastructure.Persistence;

namespace RaceCalendar.Infrastructure.Commands;

public class CreateOrUpdateSystemInfoCommand : ICreateOrUpdateSystemInfoCommand
{
    private readonly IConnectionProvider _connectionProvider;

    public CreateOrUpdateSystemInfoCommand(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task Execute(string name, string value)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        await conn.ExecuteAsync(Sql, new
        {
            Name = name,
            Value = value
        });
    }

    private const string Sql = @"
            MERGE [dbo].[SystemInfo] AS target
            USING 
            (
	            SELECT
		            @Name,
                    @Value
            ) AS source 
            (
	            Name,
                Value
            )
            ON (target.Name = source.Name)
            WHEN MATCHED THEN 
            UPDATE SET 
	            target.Value = source.Value
            WHEN NOT MATCHED THEN 
            INSERT 
            (
	            Name,
                Value
            )
            VALUES 
            (
	            source.Name,
                source.Value
            );
        ";
}
