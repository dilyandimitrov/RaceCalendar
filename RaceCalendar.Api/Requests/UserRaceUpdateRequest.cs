using RaceCalendar.Domain.Models;

namespace RaceCalendar.Api.Requests
{
    public record UserRaceUpdateRequest(
        int Id,
        int RaceId,
        int RaceDistanceId,
        string? Position,
        string? Result,
        UserRaceTypes Type,
        ResultTypes? ResultType)
    {
        public UserRace ToDomainModel(string userId)
        {
            return new UserRace(Id, userId, Type, RaceId, RaceDistanceId, null, Result, ResultType, Position);
        }
    }
}
