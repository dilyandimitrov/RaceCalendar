using RaceCalendar.Domain.Models;

namespace RaceCalendar.Api.Requests
{
    public record UserRaceCreateRequest(
        UserRaceTypes Type,
        int RaceId,
        int RaceDistanceId)
    {
        public UserRace ToDomainModel(string userId)
        {
            return new UserRace(null, userId, Type, RaceId, RaceDistanceId, null, null, null, null, null);
        }
    }
}
