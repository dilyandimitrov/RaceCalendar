using RaceCalendar.Domain.Models;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface ICreateImportDataCommand
{
    Task Execute(ImportData importData);
}
