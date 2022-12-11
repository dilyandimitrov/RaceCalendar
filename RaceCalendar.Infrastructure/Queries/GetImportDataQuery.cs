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

public class GetImportDataQuery : IGetImportDataQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetImportDataQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<ImportData> GetLast(long? id = null)
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var results = await conn.QueryAsync<ImportDataDto>(Sql,
            new
            {
                Id = id
            });

        var result = results.First();

        return new ImportData(
            result.Id,
            result.UploadedOn,
            result.Name,
            result.SizeInBytes,
            result.ContentType,
            result.Data,
            result.Notes);
    }

    public async Task<IEnumerable<ImportData>> GetAll()
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var result = await conn.QueryAsync<ImportDataDto>(Sql,
            new
            {
                Id = default(int?)
            });

        return result.Select(x => new ImportData(
            x.Id,
            x.UploadedOn,
            x.Name,
            x.SizeInBytes,
            x.ContentType,
            x.Data,
            x.Notes));
    }

    private const string Sql = $@"
SELECT 
    Id AS {nameof(ImportDataDto.Id)},
    UploadedOn AS {nameof(ImportDataDto.UploadedOn)},
    Name AS {nameof(ImportDataDto.Name)},
    SizeInBytes AS {nameof(ImportDataDto.SizeInBytes)},
    ContentType AS {nameof(ImportDataDto.ContentType)},
    Data AS {nameof(ImportDataDto.Data)},
    Notes AS {nameof(ImportDataDto.Notes)}
FROM dbo.ImportData
WHERE @Id IS NULL OR Id = @Id
ORDER BY UploadedOn DESC
";

    private sealed class ImportDataDto
    {
        public int Id { get; set; } = default!;
        public DateTime UploadedOn { get; set; } = default!;
        public string Name { get; set; } = default!;
        public long SizeInBytes { get; set; } = default!;
        public string ContentType { get; set; } = default!;
        public byte[] Data { get; set; } = default!;
        public string Notes { get; set; } = default!;
    }
}
