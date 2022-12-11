using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Models.Authentication;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IUserResultService
{
    Task<(string? position, string? result, ResultTypes? type)> GetResult(string resultUrl, string userNames);
    Task FetchAndSaveAllResults(string userEmail);
}
