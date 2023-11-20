using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Commands;

public interface ICreateImportDataCommand
{
    Task Execute(ImportData importData);
}
