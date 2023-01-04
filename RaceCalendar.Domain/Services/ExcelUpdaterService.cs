using ClosedXML.Excel;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;
using System;
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
        var workbook = new XLWorkbook(ms);

        UpdateRace(workbook, race);
        UpdateDistances(workbook, race);

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

    private void UpdateRace(XLWorkbook workbook, Race race)
    {
        var workSheet = workbook.Worksheet("Races");

        var raceCells = workSheet.Search(race.NameId);
        IXLCell? raceCell = null;
        foreach (var cell in raceCells)
        {
            var targetCell = cell.WorksheetRow().Search(race.Id.ToString()).SingleOrDefault();

            if (targetCell is not null)
            {
                raceCell = targetCell;
                break;
            }
        }

        if (raceCell is null)
        {
            return;
        }

        var row = raceCell.WorksheetRow();
        row.Cell("B").SetValue(race.Name);
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
    }

    private void UpdateDistances(XLWorkbook workbook, Race race)
    {
        foreach (var distance in race.Distances)
        {
            var workSheet = workbook.Worksheet("RaceDistances");

            var distanceCells = workSheet.Search(distance.Id.ToString());
            IXLCell? distanceCell = null;
            foreach (var cell in distanceCells)
            {
                var targetCell = cell.WorksheetRow().Search(race.Id.ToString()).SingleOrDefault();

                if (targetCell is not null)
                {
                    distanceCell = targetCell;
                    break;
                }
            }

            if (distanceCell is null)
            {
                return;
            }

            var row = distanceCell.WorksheetRow();
            row.Cell("D").SetValue(distance.Name);
            row.Cell("E").SetValue(distance.Distance);
            if (distance.StartDate.HasValue)
            {
                row.Cell("F").SetValue(distance.StartDate.Value.ToString("yyyy-MM-dd", null));
            }
            if (distance.StartTime.HasValue)
            {
                row.Cell("F").SetValue(distance.StartTime.Value.ToString(@"hh\.mm"));
            }
            row.Cell("H").SetValue(distance.ElevationGain);
            row.Cell("J").SetValue(distance.Link);
            row.Cell("K").SetValue(distance.ResultsLink);
        }
    }
}
