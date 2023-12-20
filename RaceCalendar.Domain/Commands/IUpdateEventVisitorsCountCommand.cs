namespace RaceCalendar.Domain.Commands;

public interface IUpdateEventVisitorsCountCommand
{
    Task Execute(long eventId, long visitorsCount);
}
