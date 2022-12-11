namespace RaceCalendar.Api.Responses
{
    public record ImportDataResponse(
        int? Id,
        DateTime UploadedOn,
        string Name,
        long SizeInBytes,
        string ContentType,
        string? Notes);
}
