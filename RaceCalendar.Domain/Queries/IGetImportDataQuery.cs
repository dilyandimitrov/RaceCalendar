using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Queries;

public interface IGetImportDataQuery
{
    Task<ImportData> GetLast(long? id = null);
    Task<IEnumerable<ImportData>> GetAll();
}
