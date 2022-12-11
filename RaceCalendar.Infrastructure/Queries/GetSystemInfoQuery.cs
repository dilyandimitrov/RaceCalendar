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

public class GetSystemInfoQuery : IGetSystemInfoQuery
{
    private readonly IConnectionProvider _connectionProvider;

    public GetSystemInfoQuery(IConnectionProvider connectionProvider)
    {
        _connectionProvider = connectionProvider ?? throw new ArgumentNullException(nameof(connectionProvider));
    }

    public async Task<IEnumerable<SystemInfo>> Get()
    {
        using var conn = new SqlConnection(_connectionProvider.GetConnection());

        var systemInfos = await conn.QueryAsync<SystemInfoDto>(Sql);

        return systemInfos
            .Select(s => new SystemInfo(s.Id, s.Name, s.Value))
            .ToList();
    }

    private sealed class SystemInfoDto
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Value { get; set; } = default!;
    }

    private const string Sql = $@"
SELECT 
    Id AS {nameof(SystemInfoDto.Id)},
    Name AS {nameof(SystemInfoDto.Name)},
    Value AS {nameof(SystemInfoDto.Value)}
FROM dbo.SystemInfo";
}
