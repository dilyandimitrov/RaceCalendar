using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Services.Interfaces;
using System;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services;

public class SystemInfoService : ISystemInfoService
{
    private readonly ICreateOrUpdateSystemInfoCommand _createOrUpdateSystemInfoCommand;

    private const string DbLastUpdate = "DbLastUpdate";

    public SystemInfoService(ICreateOrUpdateSystemInfoCommand createOrUpdateSystemInfoCommand)
    {
        _createOrUpdateSystemInfoCommand = createOrUpdateSystemInfoCommand ?? throw new ArgumentNullException(nameof(createOrUpdateSystemInfoCommand));
    }

    public async Task CreateOrUpdateDbLastUpdated()
    {
        await _createOrUpdateSystemInfoCommand.Execute(DbLastUpdate, DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"));
    }
}
