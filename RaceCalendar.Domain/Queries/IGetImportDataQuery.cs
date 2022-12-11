using RaceCalendar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface IGetImportDataQuery
{
    Task<ImportData> GetLast(long? id = null);
    Task<IEnumerable<ImportData>> GetAll();
}
