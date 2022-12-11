using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IImportRaceService
{
    Task<string> Import();
}
