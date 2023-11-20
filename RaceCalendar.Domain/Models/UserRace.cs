namespace RaceCalendar.Domain.Models
{
    public record UserRace
        (int? Id,
        string UserId,
        UserRaceTypes Type,
        int RaceId,
        int? RaceDistanceId,
        DateTime? CreatedOn,
        string? Result,
        ResultTypes? ResultType,
        string? Position)
    {
        public UserRace(
            int? id,
            string userId,
            UserRaceTypes type,
            int raceId,
            int? raceDistanceId,
            DateTime? createdOn,
            string? result,
            ResultTypes? resultType,
            string? position,
            string? description)
            : this(id, userId, type, raceId, raceDistanceId, createdOn, result, resultType, position)
        {
            Description = description;
        }

        public Race? Race { get; set; }
        public RaceDistance? RaceDistance { get; set; }
        public string? Description { get; set; }
    }
}
