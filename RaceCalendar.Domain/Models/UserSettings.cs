using System;

namespace RaceCalendar.Domain.Models;

public record UserSettings
    (int Id,
    string UserId,
    DateTime CreatedOn,
    DateTime? UpdatedOn,
    UserSettingsYears RacesFilterYear,
    bool? RacesShowPassed);
