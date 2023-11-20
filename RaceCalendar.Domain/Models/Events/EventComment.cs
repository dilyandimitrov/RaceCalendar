namespace RaceCalendar.Domain.Models.Events;

public class EventComment
{
    public EventComment(
        long id,
        long eventId,
        long? parentCommentId,
        string text,
        string createdBy,
        DateTime createdOn,
        DateTime? modifiedOn)
    {
        Id = id;
        EventId = eventId;
        ParentCommentId = parentCommentId;
        Text = text;
        CreatedBy = createdBy;
        CreatedOn = createdOn;
        ModifiedOn = modifiedOn;
    }

    public long Id { get; set; }
    public long EventId { get; set; }
    public long? ParentCommentId { get; set; }
    public string Text { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    public UserForEvents? CreatedByUser { get; set; } = default!;
}
