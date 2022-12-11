namespace RaceCalendar.Domain.Models;

public record UserSettingsForUpdate(
    int Id,
    UserSettingsYears RacesFilterYear,
    bool RacesShowPassed);