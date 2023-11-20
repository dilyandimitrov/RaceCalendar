namespace RaceCalendar.Domain.Queries;

public interface IGetRaceDistancesCountQuery
{
    Task<IDictionary<int, int>> Get(ISet<int> raceIds);
}
