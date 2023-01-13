using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IExcelUpdaterService
{
    Task Update(Race race);
    Task Delete(Race race);
}
