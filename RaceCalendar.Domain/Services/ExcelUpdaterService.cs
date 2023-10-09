using ClosedXML.Excel;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Services;

public class ExcelUpdaterService : IExcelUpdaterService
{
    private readonly IGetImportDataQuery _getImportDataQuery;
    private readonly ICreateImportDataCommand _createImportDataCommand;

    public ExcelUpdaterService(
        IGetImportDataQuery getImportDataQuery,
        ICreateImportDataCommand createImportDataCommand)
    {
        _getImportDataQuery = getImportDataQuery ?? throw new ArgumentNullException(nameof(getImportDataQuery));
        _createImportDataCommand = createImportDataCommand ?? throw new ArgumentNullException(nameof(createImportDataCommand));
    }

    public async Task Update(Race race)
    {
        var importData = await _getImportDataQuery.GetLast();

        using var ms = new MemoryStream(importData.Data);
        using var workbook = new XLWorkbook(ms);

        var isNewRace = race.Id == default;

        CreateOrUpdateRace(workbook, race);
        CreateOrUpdateDistances(workbook, race);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var importDataToUpdate = new ImportData(
                null,
                DateTime.UtcNow,
                importData.Name,
                stream.Length,
                importData.ContentType,
                stream.ToArray(),
                $"{(isNewRace ? "Created" : "Updated")} {race.NameId}");

        await _createImportDataCommand.Execute(importDataToUpdate);
    }

    public async Task Delete(Race race)
    {
        var importData = await _getImportDataQuery.GetLast();

        using var ms = new MemoryStream(importData.Data);
        using var workbook = new XLWorkbook(ms);

        DeleteRace(workbook, race);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);

        var importDataToUpdate = new ImportData(
                null,
                DateTime.UtcNow,
                importData.Name,
                stream.Length,
                importData.ContentType,
                stream.ToArray(),
                $"Deleted {race.NameId}");

        await _createImportDataCommand.Execute(importDataToUpdate);
    }

    private void DeleteRace(XLWorkbook workbook, Race race)
    {
        var raceWorksheet = workbook.Worksheet("Races");

        var raceRow = FindRow(raceWorksheet, race.Id);

        if (!raceRow.Search(race.NameId.ToString()).Any())
        {
            throw new InvalidOperationException($"Race with id {race.Id} didn't match the name {race.NameId}");
        }

        raceRow.Delete();

        var distancesWorksheet = workbook.Worksheet("RaceDistances");
        foreach (var distance in race.Distances)
        {
            var distanceRow = FindRow(distancesWorksheet, distance.Id);

            if (!distanceRow.Search(race.NameId.ToString()).Any())
            {
                throw new InvalidOperationException($"Race distance with id {distance.Id} didn't match the race {race.NameId}");
            }

            distanceRow.Delete();

            if (distance.Info is not null)
            {
                var infoWorksheet = workbook.Worksheet("RaceInfo");
                foreach (var info in distance.Info)
                {
                    var infoRow = FindRow(infoWorksheet, info.Id);

                    if (!infoRow.Search(race.NameId.ToString()).Any())
                    {
                        throw new InvalidOperationException($"Race distance info with id {info.Id} didn't match the race {race.NameId}");
                    }

                    infoRow.Delete();
                }
            }
        }
    }

    private void CreateOrUpdateRace(XLWorkbook workbook, Race race)
    {
        UpdateRace(workbook, race);
        AddRace(workbook, race);
    }

    private void AddRace(XLWorkbook workbook, Race race)
    {
        if (race.Id != default)
        {
            return;
        }

        var workSheet = workbook.Worksheet("Races");

        var lastRow = workSheet.LastRowUsed();
        var row = lastRow.InsertRowsBelow(1).First();
        race.Id = lastRow.Cell("A").GetValue<int>() + 1;

        row.Cell("A").SetValue(race.Id);
        row.Cell("B").SetValue(race.Name);
        row.Cell("C").SetValue(race.NameId);
        row.Cell("D").SetValue(race.Country ?? "България");
        row.Cell("E").SetValue(race.City);
        row.Cell("F").SetValue(race.StartDate?.ToString("yyyy-MM-dd", null));
        row.Cell("G").SetValue(race.EndDate?.ToString("yyyy-MM-dd", null));
        row.Cell("H").SetValue(race.Link);

        CreateOrUpdateFlag<Terrains>(row, "I", race.Terrain);
        CreateOrUpdateFlag<Specials>(row, "J", race.Special);
        CreateOrUpdateFlag<Cancelled>(row, "L", race.Cancelled);

        row.Cell("P").SetValue(race.Latitude);
        row.Cell("Q").SetValue(race.Longitude);
    }

    private void UpdateRace(XLWorkbook workbook, Race race)
    {
        if (race.Id == default)
        {
            return;
        }

        var worksheet = workbook.Worksheet("Races");

        var row = FindRow(worksheet, race.Id);

        if (!row.Search(race.NameId.ToString()).Any())
        {
            throw new InvalidOperationException($"Race with id {race.Id} didn't match the name {race.NameId}");
        }

        row.Cell("B").SetValue(race.Name);
        row.Cell("C").SetValue(race.NameId);
        row.Cell("E").SetValue(race.City);
        row.Cell("F").SetValue(race.StartDate?.ToString("yyyy-MM-dd", null));
        row.Cell("G").SetValue(race.EndDate?.ToString("yyyy-MM-dd", null));
        row.Cell("H").SetValue(race.Link);

        CreateOrUpdateFlag<Terrains>(row, "I", race.Terrain);
        CreateOrUpdateFlag<Specials>(row, "J", race.Special);
        CreateOrUpdateFlag<Cancelled>(row, "L", race.Cancelled);

        row.Cell("P").SetValue(race.Latitude);
        row.Cell("Q").SetValue(race.Longitude);
    }

    private void CreateOrUpdateDistances(XLWorkbook workbook, Race race)
    {
        DeleteDistances(workbook, race);
        UpdateDistances(workbook, race);
        AddDistances(workbook, race);
    }

    private void DeleteDistances(XLWorkbook workbook, Race race)
    {
        var worksheet = workbook.Worksheet("RaceDistances");

        var allDistanceCells = worksheet
            .Range($"B2:B{worksheet.RowCount()}")
            .CellsUsed(x => x.GetValue<int>() == race.Id);

        var rowsToBeDeleted = new List<IXLRow>();
        foreach (var cell in allDistanceCells)
        {
            var row = cell.WorksheetRow();
            if (!race.Distances.Select(x => x.Id).Contains(row.Cell("A").GetValue<int>()))
            {
                rowsToBeDeleted.Add(row);
            }
        }

        var distanceIdsToDelete = rowsToBeDeleted.Select(x => x.Cell("A").GetValue<int>()).ToList();
        foreach (var row in rowsToBeDeleted)
        {
            row.Delete();
        }

        var infoWorkSheet = workbook.Worksheet("RaceInfo");
        var allInfoCellsToDelete = infoWorkSheet
            .Range($"D2:D{worksheet.RowCount()}")
            .CellsUsed(x => distanceIdsToDelete.Contains(x.GetValue<int>()));

        foreach (var cell in allInfoCellsToDelete)
        {
            var row = cell.WorksheetRow();
            row.Delete();
        }
    }

    private void AddDistances(XLWorkbook workbook, Race race)
    {
        var workSheet = workbook.Worksheet("RaceDistances");

        foreach (var distance in race.Distances.Where(d => d.Id == default))
        {
            var lastRow = workSheet.LastRowUsed();
            var row = lastRow.InsertRowsBelow(1).First();
            distance.Id = lastRow.Cell("A").GetValue<int>() + 1;
            distance.RaceId = race.Id;
            row.Cell("A").SetValue(distance.Id);
            row.Cell("B").SetValue(distance.RaceId);
            row.Cell("C").SetValue(race.NameId);
            row.Cell("D").SetValue(distance.Name);
            row.Cell("E").SetValue(distance.Distance);
            row.Cell("F").SetValue(distance.StartDate?.ToString("yyyy-MM-dd", null));
            row.Cell("G").SetValue(distance.StartTime?.ToString(@"hh\:mm"));
            row.Cell("H").SetValue(distance.ElevationGain);
            row.Cell("J").SetValue(distance.Link);
            row.Cell("K").SetValue(distance.ResultsLink);
            row.Cell("M").SetValue(distance.Latitude);
            row.Cell("N").SetValue(distance.Longitude);

            if (distance.Info is not null)
            {
                foreach (var info in distance!.Info)
                {
                    AddNewInfo(workbook.Worksheet("RaceInfo"), race, distance, info);
                }
            }
        }
    }

    private void UpdateDistances(XLWorkbook workbook, Race race)
    {
        var worksheet = workbook.Worksheet("RaceDistances");

        foreach (var distance in race.Distances.Where(d => d.Id != default))
        {
            var row = FindRow(worksheet, distance.Id);

            if (!row.Search(race.NameId.ToString()).Any())
            {
                throw new InvalidOperationException($"Race distance with id {distance.Id} didn't match the race {race.NameId}");
            }

            row.Cell("D").SetValue(distance.Name);
            row.Cell("E").SetValue(distance.Distance);
            row.Cell("F").SetValue(distance.StartDate?.ToString("yyyy-MM-dd", null));
            row.Cell("G").SetValue(distance.StartTime?.ToString(@"hh\:mm"));
            row.Cell("H").SetValue(distance.ElevationGain);
            row.Cell("J").SetValue(distance.Link);
            row.Cell("K").SetValue(distance.ResultsLink);
            row.Cell("M").SetValue(distance.Latitude);
            row.Cell("N").SetValue(distance.Longitude);

            UpdateInfos(workbook, race, distance);
        }
    }

    private void UpdateInfos(XLWorkbook workbook, Race race, RaceDistance distance)
    {
        if (distance.Info is null)
        {
            return;
        }

        var worksheet = workbook.Worksheet("RaceInfo");

        var allInfoCells = worksheet
            .Range($"D2:D{worksheet.RowCount()}")
            .CellsUsed(x => x.GetValue<int>() == distance.Id);

        foreach (var cell in allInfoCells)
        {
            var row = cell.WorksheetRow();
            if (!distance.Info.Select(x => x.Id).Contains(row.Cell("A").GetValue<int>()))
            {
                row.Delete();
            }
        }

        foreach (var info in distance.Info)
        {
            if (info.Id != default)
            {
                var row = FindRow(worksheet, info.Id);

                if (!row.Search(race.NameId.ToString()).Any())
                {
                    throw new InvalidOperationException($"Race distance info with id {info.Id} didn't match the race {race.NameId}");
                }

                row.Cell("E").SetValue(distance.Distance);
                row.Cell("G").SetValue(info.Value);
            }
            else
            {
                AddNewInfo(worksheet, race, distance, info);
            }
        }
    }

    private void AddNewInfo(IXLWorksheet worksheet, Race race, RaceDistance distance, RaceInfo info)
    {
        var lastRow = worksheet.LastRowUsed();
        var row = lastRow.InsertRowsBelow(1).First();
        int infoId = lastRow.Cell("A").GetValue<int>() + 1;

        info.Id = infoId;
        info.RaceId = race.Id;
        info.RaceDistanceId = distance.Id;
        
        row.Cell("A").SetValue(info.Id);
        row.Cell("B").SetValue(info.RaceId);
        row.Cell("C").SetValue(race.NameId);
        row.Cell("D").SetValue(info.RaceDistanceId);
        row.Cell("E").SetValue(distance.Distance);
        row.Cell("F").SetValue(info.Name);
        row.Cell("G").SetValue(info.Value);
    }

    private IXLRow FindRow(IXLWorksheet worksheet, int id)
    {
        var idCell = worksheet
            .Range($"A2:A{worksheet.RowCount()}")
            .CellsUsed(x => x.GetValue<int>() == id)
            .SingleOrDefault();

        if (idCell is null)
        {
            throw new InvalidOperationException($"Cell with id {id} was not found in sheet {worksheet.Name}");
        }

        var row = idCell.WorksheetRow();

        return row;
    }

    private void CreateOrUpdateFlag<T>(IXLRow row, string column, T? value) where T : struct
    {
        if (value is null)
        {
            row.Cell(column).SetValue(string.Empty);
            row.Cell(column).DataType = XLDataType.Number;
        }
        else
        {
            row.Cell(column).SetValue((int)(object)value);
        }
    }
}
