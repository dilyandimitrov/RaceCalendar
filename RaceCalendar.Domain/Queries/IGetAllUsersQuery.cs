using RaceCalendar.Domain.Responses;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface IGetAllUsersQuery
{
    Task<IEnumerable<GetAllUsersResponse>> Get();
}
