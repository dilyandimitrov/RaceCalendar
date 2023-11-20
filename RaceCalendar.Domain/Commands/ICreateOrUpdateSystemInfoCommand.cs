namespace RaceCalendar.Domain.Commands;

public interface ICreateOrUpdateSystemInfoCommand
{
    Task Execute(string name, string value);
}
