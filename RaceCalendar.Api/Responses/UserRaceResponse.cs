namespace RaceCalendar.Api.Responses
{
    public record UserRaceResponse(
        int Id,
        int RaceId,
        int? RaceDistanceId);
}