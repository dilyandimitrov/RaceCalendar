using RaceCalendar.Domain.Responses;

namespace RaceCalendar.Domain.Queries;

public interface IGetAllUsersQuery
{
    Task<IEnumerable<GetAllUsersResponse>> Get();
}
