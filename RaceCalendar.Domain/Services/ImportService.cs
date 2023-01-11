using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ExcelDataReader;
using Microsoft.Extensions.Logging;
using NodaTime;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Domain.Services;

public class ImportRaceService : IImportRaceService
{
    protected List<string> _messages = new List<string>();
    protected List<string> _errors = new List<string>();

    protected int _countAdded = 0;
    protected int _countUpdated = 0;
    protected int _cacheLookups = 0;

    protected Dictionary<string, dynamic> _cachedRaces = new Dictionary<string, dynamic>();
    protected Dictionary<string, dynamic> _cachedDistances = new Dictionary<string, dynamic>();

    protected string _importSource;

    private readonly ILogger<ImportRaceService> _logger;
    private readonly IGetImportDataQuery _getImportDataQuery;
    private readonly IRaceService _raceService;
    private readonly ICreateOrUpdateSystemInfoCommand _createOrUpdateSystemInfoCommand;
    private readonly ICreateRaceCommand _createRaceCommand;
    private readonly IUpdateRaceCommand _updateRaceCommand;
    private readonly ICreateRaceDistanceCommand _createRaceDistanceCommand;
    private readonly IUpdateRaceDistanceCommand _updateRaceDistanceCommand;
    private readonly ICreateRaceInfoCommand _createRaceInfoCommand;
    private readonly IUpdateRaceInfoCommand _updateRaceInfoCommand;

    public ImportRaceService(
        ILogger<ImportRaceService> logger,
        IGetImportDataQuery getImportDataQuery,
        IRaceService raceService,
        ICreateOrUpdateSystemInfoCommand createOrUpdateSystemInfoCommand,
        ICreateRaceCommand createRaceCommand,
        IUpdateRaceCommand updateRaceCommand,
        ICreateRaceDistanceCommand createRaceDistanceCommand,
        IUpdateRaceDistanceCommand updateRaceDistanceCommand,
        ICreateRaceInfoCommand createRaceInfoCommand,
        IUpdateRaceInfoCommand updateRaceInfoCommand)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _getImportDataQuery = getImportDataQuery ?? throw new ArgumentNullException(nameof(getImportDataQuery));
        _raceService = raceService ?? throw new ArgumentNullException(nameof(raceService));
        _createOrUpdateSystemInfoCommand = createOrUpdateSystemInfoCommand ?? throw new ArgumentNullException(nameof(createOrUpdateSystemInfoCommand));
        _createRaceCommand = createRaceCommand ?? throw new ArgumentNullException(nameof(createRaceCommand));
        _updateRaceCommand = updateRaceCommand ?? throw new ArgumentNullException(nameof(updateRaceCommand));
        _createRaceDistanceCommand = createRaceDistanceCommand ?? throw new ArgumentNullException(nameof(createRaceDistanceCommand));
        _updateRaceDistanceCommand = updateRaceDistanceCommand ?? throw new ArgumentNullException(nameof(updateRaceDistanceCommand));
        _createRaceInfoCommand = createRaceInfoCommand ?? throw new ArgumentNullException(nameof(createRaceInfoCommand));
        _updateRaceInfoCommand = updateRaceInfoCommand ?? throw new ArgumentNullException(nameof(updateRaceInfoCommand));
    }

    public async Task<string> Import()
    {
        FileStream? stream = null;
        MemoryStream? ms = null;
        IExcelDataReader? reader = null;
        DataSet? result = null;

        var start = Stopwatch.StartNew();

        try
        {
            try
            {
                var importData = await _getImportDataQuery.GetLast();
                using (ms = new MemoryStream(importData.Data))
                {
                    using (reader = ExcelReaderFactory.CreateReader(ms))
                    {
                        result = reader.AsDataSet();
                        _importSource = $"DB (Id: {importData.Id})";
                    }
                }
            }
            catch (Exception)
            {
                using (stream = File.Open(@"Import/racecal.xlsx", FileMode.Open, FileAccess.Read))
                {
                    using (reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        result = reader.AsDataSet();
                        _importSource = "file";
                    }
                }
            }

            if (result is null) return "Result is null";

            foreach (DataTable table in result.Tables)
            {
                switch (table.TableName)
                {
                    case "Races":
                        await ImportRaces(table);
                        break;
                    case "RaceDistances":
                        await ImportRaceDistances(table);
                        break;
                    case "RaceInfo":
                        await ImportRaceInfo(table);
                        break;
                }
            }

            await _createOrUpdateSystemInfoCommand.Execute("DbLastUpdate", DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"));
        }
        catch (Exception ex)
        {
            stream?.Dispose();
            reader?.Dispose();
            ms?.Dispose();

            return ex?.Message +
                Environment.NewLine + ex?.StackTrace +
                Environment.NewLine + ex?.InnerException?.Message +
                Environment.NewLine + ex?.InnerException?.StackTrace;
        }

        start.Stop();

        return $@"Data from {_importSource} has been imported successfully!
Time Elapsed: {start.Elapsed.TotalSeconds} seconds
Cache Lookups: {_cacheLookups}
{PrintMessages(_messages)}
{PrintMessages(_errors)}
";
    }

    private async Task ImportRaces(DataTable table)
    {
        int i = 0;
        _countAdded = _countUpdated = 0;

        foreach (DataRow row in table.Rows)
        {
            // skip the first row with the names of the columns
            if (i++ == 0) continue;

            try
            {
                bool isStartDateConfirmed;
                bool isEndDateConfirmed;

                int? id = ParseInt(row.ItemArray[0]);
                string name = row.ItemArray[1].ToString();
                string nameId = row.ItemArray[2].ToString();
                string country = row.ItemArray[3].ToString();
                string city = row.ItemArray[4].ToString();
                DateTime? startDate = ParseDate(row.ItemArray[5], out isStartDateConfirmed, country);
                DateTime? endDate = ParseDate(row.ItemArray[6], out isEndDateConfirmed, country);
                string link = row.ItemArray[7].ToString();
                Terrains? terrain = ParseTerrain(row.ItemArray[8]);
                Specials? special = ParseSpecial(row.ItemArray[9]);
                Cancelled? cancelled = ParseCancelled(row.ItemArray[11]);

                int? unconfirmedDate = null;
                if (!isStartDateConfirmed)
                {
                    string item = (row.ItemArray[5] as string).Replace("-", string.Empty);
                    unconfirmedDate = int.Parse(item);
                }

                var race = new Race(
                    id.Value,
                    name,
                    nameId,
                    country,
                    city,
                    startDate.HasValue ? LocalDateTime.FromDateTime(startDate.Value).Date : null,
                    endDate.HasValue ? LocalDateTime.FromDateTime(endDate.Value).Date : null,
                    link,
                    tags: (special & Specials.Bfla) == Specials.Bfla ? "бфла" : string.Empty,
                    cancelled,
                    terrain,
                    special);

                var raceDb = await _raceService.Get(id.Value);
                if (raceDb == null)
                {
                    _countAdded++;
                    await _createRaceCommand.Execute(race);
                }
                else
                {
                    _countUpdated++;
                    await _updateRaceCommand.Execute(race);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw;
            }
        }

        _messages.Add($"{table.TableName} - Imported: {_countAdded}, Updated: {_countUpdated}");
    }

    private async Task ImportRaceDistances(DataTable table)
    {
        int i = 0;
        _countAdded = _countUpdated = 0;

        foreach (DataRow row in table.Rows)
        {
            // skip the first row with the names of the columns
            if (i++ == 0) continue;

            try
            {
                int? id = ParseInt(row.ItemArray[0]);
                int? raceId = ParseInt(row.ItemArray[1]);
                string name = row.ItemArray[3].ToString();

                var race = await _raceService.Get(raceId.Value);
                if (race == null)
                {
                    _errors.Add($"RaceDistances - Race '{raceId}' not found!");
                    continue;
                }

                bool isDateConfirmed;
                double tmp = 0;
                if (!double.TryParse(row.ItemArray[4].ToString(), out tmp)) { }

                double? distance = tmp != 0 ? Math.Round(tmp, 4) : (double?)null;
                DateTime? startDate = ParseDate(row.ItemArray[5], out isDateConfirmed, race.Country);
                TimeSpan? startTime = ParseTime(row.ItemArray[6]);
                int? elevationGain = string.IsNullOrEmpty(row.ItemArray[7].ToString()) ? (int?)null : Int32.Parse(row.ItemArray[7].ToString());
                string price = row.ItemArray[8].ToString();
                string link = row.ItemArray[9].ToString();
                string resultsLink = row.ItemArray[10].ToString();

                int? unconfirmedDate = null;
                if (!isDateConfirmed)
                {
                    string item = (row.ItemArray[5] as string).Replace("-", string.Empty);
                    unconfirmedDate = int.Parse(item);
                }

                var raceDistance = new RaceDistance(
                    id.Value,
                    race.Id,
                    name,
                    distance,
                    startDate.HasValue ? LocalDateTime.FromDateTime(startDate.Value).Date : null,
                    startTime,
                    unconfirmedDate,
                    elevationGain,
                    price,
                    link,
                    resultsLink);

                RaceDistance raceDistanceDb = race.Distances.FirstOrDefault(d => d.Id == id.Value);
                if (raceDistanceDb == null)
                {
                    _countAdded++;
                    await _createRaceDistanceCommand.Execute(raceDistance);
                }
                else
                {
                    _countUpdated++;
                    await _updateRaceDistanceCommand.Execute(raceDistance);
                }

                // update Race Tags
                if (!string.IsNullOrEmpty(raceDistance.Name))
                {
                    race.Tags = $"{race.Tags} {raceDistance.Name}".Trim();
                    await _updateRaceCommand.Execute(race);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw;
            }
        }

        _messages.Add($"{table.TableName} - Imported: {_countAdded}, Updated: {_countUpdated}");
    }

    private async Task ImportRaceInfo(DataTable table)
    {
        int i = 0;
        _countAdded = _countUpdated = 0;

        foreach (DataRow row in table.Rows)
        {
            // skip the first row with the names of the columns
            if (i++ == 0) continue;

            try
            {
                int? id = ParseInt(row.ItemArray[0]);
                int? raceId = ParseInt(row.ItemArray[1]);
                int? raceDistanceId = ParseInt(row.ItemArray[3]);

                double tmp = 0;
                if (!double.TryParse(row.ItemArray[4].ToString(), out tmp)) { }

                double? distance = tmp != 0 ? tmp : (double?)null;
                string name = row.ItemArray[5].ToString();
                string value = row.ItemArray[6].ToString();

                var race = await _raceService.Get(raceId.Value);
                if (race == null)
                {
                    _errors.Add($"RaceInfo - Race '{raceId}' not found!");
                    continue;
                }

                RaceDistance raceDistance = race.Distances.FirstOrDefault(d => d.Id == raceDistanceId.Value);

                if (raceDistance == null)
                {
                    _errors.Add($"RaceInfo - Race '{raceDistanceId}' and distance '{distance}' not found!");
                    continue;
                }

                var raceInfo = new RaceInfo(
                    id.Value,
                    race.Id,
                    raceDistance.Id,
                    name,
                    string.IsNullOrEmpty(value) ? null : value);

                RaceInfo raceInfoDb = raceDistance.Info.FirstOrDefault(d => d.Id == id.Value);
                if (raceInfoDb == null)
                {
                    _countAdded++;
                    await _createRaceInfoCommand.Execute(raceInfo);
                }
                else
                {
                    _countUpdated++;
                    await _updateRaceInfoCommand.Execute(raceInfo);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, null);
                throw;
            }
        }

        _messages.Add($"{table.TableName} - Imported: {_countAdded}, Updated: {_countUpdated}");
    }

    private static string PrintMessages(List<string> messages)
    {
        return string.Join("\n", messages);
    }

    protected DateTime? ParseDate(object data, out bool isDateConfirmed, string country = null)
    {
        DateTime date;
        isDateConfirmed = true;

        if (string.IsNullOrEmpty(data.ToString()))
            return null;

        if (DateTime.TryParseExact(data.ToString(), "yyyy-MM-dd", null, DateTimeStyles.AssumeLocal, out date))
        {
            return date;
        }
        else if (data.ToString().Length == 7)
        {
            string[] items = (data.ToString()).Split("-");
            int year = int.Parse(items[0]);
            int month = int.Parse(items[1]);
            var daysInMonth = DateTime.DaysInMonth(year, month);

            date = new DateTime(year, month, daysInMonth);

            isDateConfirmed = false;
        }
        else
        {
            throw new ArgumentException($"Date format {data} cannot be parsed as valid date");
        }

        return date;
    }

    protected TimeSpan? ParseTime(object data)
    {
        if (string.IsNullOrEmpty(data.ToString()))
            return null;

        DateTime date;
        if (!DateTime.TryParse(data.ToString(), out date))
        {
            throw new ArgumentException($"Date format {data} cannot be parsed as valid time");
        }

        return DateTime.Parse(data.ToString()).TimeOfDay;
    }

    protected int? ParseInt(object data)
    {
        int dataInt;
        if (int.TryParse(data.ToString(), out dataInt))
        {
            return dataInt;
        }

        return null;
    }

    protected Terrains? ParseTerrain(object data)
    {
        int dataInt;
        if (int.TryParse(data.ToString(), out dataInt))
        {
            return (Terrains?)dataInt;
        }

        return null;
    }

    protected Specials? ParseSpecial(object data)
    {
        int dataInt;
        if (int.TryParse(data.ToString(), out dataInt))
        {
            return (Specials?)dataInt;
        }

        return null;
    }

    protected Cancelled? ParseCancelled(object data)
    {
        int dataInt;
        if (int.TryParse(data.ToString(), out dataInt))
        {
            return (Cancelled?)dataInt;
        }

        return null;
    }
}
