namespace RaceCalendar.Api.Requests;

public record UpdateEventCommentRequest(
    long Id,
    long EventId,
    long? ParentCommentId,
    string Text,
    string CreatedBy);
