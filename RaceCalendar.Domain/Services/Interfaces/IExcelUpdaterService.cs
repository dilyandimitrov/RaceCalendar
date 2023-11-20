using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IExcelUpdaterService
{
    Task Update(Race race);
    Task Delete(Race race);
}
