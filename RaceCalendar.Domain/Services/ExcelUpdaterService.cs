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
                "Updated From Code");

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
                "Updated From Code");

        await _createImportDataCommand.Execute(importDataToUpdate);
    }

    private void DeleteRace(XLWorkbook workbook, Race race)
    {
        var raceWorksheet = workbook.Worksheet("Races");

        var raceIdCell = raceWorksheet.Range($"A2:A{raceWorksheet.RowCount()}").CellsUsed(x => x.GetValue<int>() == race.Id).SingleOrDefault();

        if (raceIdCell is null)
        {
            throw new InvalidOperationException($"Race with id {race.Id} was not found");
        }

        var raceRow = raceIdCell.WorksheetRow();

        if (!raceRow.Search(race.NameId.ToString()).Any())
        {
            throw new InvalidOperationException($"Race with id {race.Id} didn't match the name {race.NameId}");
        }

        raceRow.Delete();

        var distancesWorksheet = workbook.Worksheet("RaceDistances");
        foreach (var distance in race.Distances)
        {
            var distanceIdCell = distancesWorksheet.Range($"A2:A{distancesWorksheet.RowCount()}").CellsUsed(x => x.GetValue<int>() == distance.Id).SingleOrDefault();

            if (distanceIdCell is null)
            {
                throw new InvalidOperationException($"Race distance with id {distance.Id} was not found");
            }

            var distanceRow = distanceIdCell.WorksheetRow();

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
                    var infoIdCell = infoWorksheet.Range($"A2:A{infoWorksheet.RowCount()}").CellsUsed(x => x.GetValue<int>() == info.Id).SingleOrDefault();

                    if (infoIdCell is null)
                    {
                        throw new InvalidOperationException($"Race distance info with id {info.Id} was not found");
                    }

                    var infoRow = infoIdCell.WorksheetRow();

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
        if (race.StartDate.HasValue)
        {
            row.Cell("F").SetValue(race.StartDate.Value.ToString("yyyy-MM-dd", null));
        }
        if (race.EndDate.HasValue)
        {
            row.Cell("G").SetValue(race.EndDate.Value.ToString("yyyy-MM-dd", null));
        }
        row.Cell("H").SetValue(race.Link);

        if (race.Terrain == 0 || race.Terrain is null)
        {
            row.Cell("I").SetValue(string.Empty);
            row.Cell("I").DataType = XLDataType.Number;
        }
        else
        {
            row.Cell("I").SetValue((int)race.Terrain);
        }

        if (race.Special == 0 || race.Special is null)
        {
            row.Cell("J").SetValue(string.Empty);
            row.Cell("J").DataType = XLDataType.Number;
        }
        else
        {
            row.Cell("J").SetValue((int)race.Special);
        }

        if (race.Cancelled == 0 || race.Cancelled is null)
        {
            row.Cell("L").SetValue(string.Empty);
            row.Cell("L").DataType = XLDataType.Number;
        }
        else
        {
            row.Cell("L").SetValue((int)race.Cancelled);
        }
    }

    private void UpdateRace(XLWorkbook workbook, Race race)
    {
        if (race.Id == default)
        {
            return;
        }

        var workSheet = workbook.Worksheet("Races");

        var raceIdCell = workSheet.Range($"A2:A{workSheet.RowCount()}").CellsUsed(x => x.GetValue<int>() == race.Id).SingleOrDefault();

        if (raceIdCell is null)
        {
            throw new InvalidOperationException($"Race with id {race.Id} was not found");
        }

        var row = raceIdCell.WorksheetRow();

        if (!row.Search(race.NameId.ToString()).Any())
        {
            throw new InvalidOperationException($"Race with id {race.Id} didn't match the name {race.NameId}");
        }

        row.Cell("B").SetValue(race.Name);
        row.Cell("C").SetValue(race.NameId);
        row.Cell("E").SetValue(race.City);
        if (race.StartDate.HasValue)
        {
            row.Cell("F").SetValue(race.StartDate.Value.ToString("yyyy-MM-dd", null));
        }
        if (race.EndDate.HasValue)
        {
            row.Cell("G").SetValue(race.EndDate.Value.ToString("yyyy-MM-dd", null));
        }
        row.Cell("H").SetValue(race.Link);

        if (race.Terrain == 0 || race.Terrain is null)
        {
            row.Cell("I").SetValue(string.Empty);
            row.Cell("I").DataType = XLDataType.Number;
        }
        else
        {
            row.Cell("I").SetValue((int)race.Terrain);
        }

        if (race.Special == 0 || race.Special is null)
        {
            row.Cell("J").SetValue(string.Empty);
            row.Cell("J").DataType = XLDataType.Number;
        }
        else
        {
            row.Cell("J").SetValue((int)race.Special);
        }

        if (race.Cancelled == 0 || race.Cancelled is null)
        {
            row.Cell("L").SetValue(string.Empty);
            row.Cell("L").DataType = XLDataType.Number;
        }
        else
        {
            row.Cell("L").SetValue((int)race.Cancelled);
        }
    }

    private void CreateOrUpdateDistances(XLWorkbook workbook, Race race)
    {
        DeleteDistances(workbook, race);
        UpdateDistances(workbook, race);
        AddDistances(workbook, race);
    }

    private void DeleteDistances(XLWorkbook workbook, Race race)
    {
        var workSheet = workbook.Worksheet("RaceDistances");

        var allDistanceCells = workSheet
            .Range($"B2:B{workSheet.RowCount()}")
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
            .Range($"D2:D{workSheet.RowCount()}")
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

            if (distance.StartDate.HasValue)
            {
                row.Cell("F").SetValue(distance.StartDate.Value.ToString("yyyy-MM-dd", null));
            }

            if (distance.StartTime.HasValue)
            {
                row.Cell("G").SetValue(distance.StartTime.Value.ToString(@"hh\:mm"));
            }

            row.Cell("H").SetValue(distance.ElevationGain);
            row.Cell("J").SetValue(distance.Link);
            row.Cell("K").SetValue(distance.ResultsLink);

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
        var workSheet = workbook.Worksheet("RaceDistances");

        foreach (var distance in race.Distances.Where(d => d.Id != default))
        {
            var distanceIdCell = workSheet.Range($"A2:A{workSheet.RowCount()}").CellsUsed(x => x.GetValue<int>() == distance.Id).SingleOrDefault();

            if (distanceIdCell is null)
            {
                throw new InvalidOperationException($"Race distance with id {distance.Id} was not found");
            }

            var row = distanceIdCell.WorksheetRow();

            if (!row.Search(race.NameId.ToString()).Any())
            {
                throw new InvalidOperationException($"Race distance with id {distance.Id} didn't match the race {race.NameId}");
            }

            row.Cell("D").SetValue(distance.Name);
            row.Cell("E").SetValue(distance.Distance);

            if (distance.StartDate.HasValue)
            {
                row.Cell("F").SetValue(distance.StartDate.Value.ToString("yyyy-MM-dd", null));
            }
            if (distance.StartTime.HasValue)
            {
                row.Cell("G").SetValue(distance.StartTime.Value.ToString(@"hh\:mm"));
            }

            row.Cell("H").SetValue(distance.ElevationGain);
            row.Cell("J").SetValue(distance.Link);
            row.Cell("K").SetValue(distance.ResultsLink);

            UpdateInfos(workbook, race, distance);
        }
    }

    private void UpdateInfos(XLWorkbook workbook, Race race, RaceDistance distance)
    {
        if (distance.Info is null)
        {
            return;
        }

        var workSheet = workbook.Worksheet("RaceInfo");

        var allInfoCells = workSheet
            .Range($"D2:D{workSheet.RowCount()}")
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
                var infoIdCell = workSheet.Range($"A2:A{workSheet.RowCount()}").CellsUsed(x => x.GetValue<int>() == info.Id).SingleOrDefault();

                if (infoIdCell is null)
                {
                    throw new InvalidOperationException($"Race distance info with id {info.Id} was not found");
                }

                var row = infoIdCell.WorksheetRow();

                if (!row.Search(race.NameId.ToString()).Any())
                {
                    throw new InvalidOperationException($"Race distance info with id {info.Id} didn't match the race {race.NameId}");
                }

                row.Cell("G").SetValue(info.Value);
            }
            else
            {
                AddNewInfo(workSheet, race, distance, info);
            }
        }
    }

    private void AddNewInfo(IXLWorksheet workSheet, Race race, RaceDistance distance, RaceInfo info)
    {
        var lastRow = workSheet.LastRowUsed();
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
}
