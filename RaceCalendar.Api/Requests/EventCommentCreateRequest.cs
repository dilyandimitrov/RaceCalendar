namespace RaceCalendar.Api.Requests;

public record EventCommentCreateRequest(
    long Id,
    long EventId,
    long? ParentCommentId,
    string Text);
