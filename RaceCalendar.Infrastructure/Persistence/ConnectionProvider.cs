using Microsoft.Extensions.Options;

namespace RaceCalendar.Infrastructure.Persistence;

public class ConnectionProvider : IConnectionProvider
{
    private readonly ConnectionStringsOptions _options;

    public ConnectionProvider(IOptions<ConnectionStringsOptions> options)
    {
        _options = options.Value;
    }

    public string GetConnection()
    {
        return _options.DefaultConnection;
    }
}
