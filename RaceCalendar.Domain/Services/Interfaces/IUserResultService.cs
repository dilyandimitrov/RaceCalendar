using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IUserResultService
{
    Task<(string? position, string? result, ResultTypes? type)> GetResult(string resultUrl, string userNames);
    Task FetchAndSaveAllResults(string userEmail);
}
