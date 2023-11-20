namespace RaceCalendar.Domain.Models;

public record GeoPoint(string Type, Geometry Geometry, Properties Properties);

public record Geometry(string Type, IEnumerable<decimal> Coordinates);

public record Properties(string Name, string NameId, DateTime startDate, IEnumerable<RaceDistanceSimple> Distances);

public record RaceDistanceSimple(int Id, double? Distance, int? ElevationGain, Specials? Special);