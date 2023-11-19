namespace RaceCalendar.Api.Responses;

public record EventCommentResponse(
    long Id,
    long EventId,
    string Text,
    IEnumerable<EventCommentResponse>? Replies,
    DateTime CreatedOn,
    DateTime? ModifiedOn,
    string CreatedBy,
    string CreatedByFirstName,
    string CreatedByLastName);