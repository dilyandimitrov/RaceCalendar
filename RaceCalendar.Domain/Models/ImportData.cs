using System;

namespace RaceCalendar.Domain.Models;

public record ImportData(
    int? Id,
    DateTime UploadedOn,
    string Name,
    long SizeInBytes,
    string ContentType,
    byte[] Data,
    string? Notes);