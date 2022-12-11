using RaceCalendar.Domain.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Queries;

public interface IGetUserRacesByUserQuery
{
    Task<IEnumerable<UserRace>> Get(string userId);
}
