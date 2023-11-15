namespace RaceCalendar.Domain.Models.Authentication;

public record UserResponse(string Id, string Email, string FirstName, string LastName, bool IsAdmin);