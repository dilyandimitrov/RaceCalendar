﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceCalendar.Api.Responses;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;

namespace RaceCalendar.Api.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("v1/api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly IGetImportDataQuery _getImportDataQuery;
        private readonly ICreateImportDataCommand _createImportDataCommand;
        private readonly IImportRaceService _importRaceService;
        private readonly IDeleteAllRacesCommand _deleteAllRacesCommand;

        public ImportController(
            IGetImportDataQuery getImportDataQuery,
            ICreateImportDataCommand createImportDataCommand,
            IImportRaceService importRaceService,
            IDeleteAllRacesCommand deleteAllRacesCommand)
        {
            _getImportDataQuery = getImportDataQuery ?? throw new ArgumentNullException(nameof(getImportDataQuery));
            _createImportDataCommand = createImportDataCommand ?? throw new ArgumentNullException(nameof(createImportDataCommand));
            _importRaceService = importRaceService ?? throw new ArgumentNullException(nameof(importRaceService));
            _deleteAllRacesCommand = deleteAllRacesCommand ?? throw new ArgumentNullException(nameof(deleteAllRacesCommand));
        }

        [HttpGet]
        [Route("list_uploaded_files")]
        public async Task<IEnumerable<ImportDataResponse>> ListUploadedFiles()
        {
            var result = (await _getImportDataQuery.GetAll())
                .Select(x => new ImportDataResponse(
                    x.Id,
                    x.UploadedOn,
                    x.Name,
                    x.SizeInBytes,
                    x.ContentType,
                    x.Notes))
                .ToList();

            return result;
        }

        [HttpGet("run")]
        public async Task<ImportResultResponse> Import()
        {
            var result = await _importRaceService.Import();
            return new ImportResultResponse(result);
        }

        [HttpPost]
        [Route("upload")]
        public async Task PostFile(IFormFile uploadedFile, string? notes)
        {
            byte[] fileBytes;
            using (var ms = new MemoryStream())
            {
                uploadedFile.CopyTo(ms);
                fileBytes = ms.ToArray();
            }

            var importData = new ImportData(
                null,
                DateTime.UtcNow,
                uploadedFile.FileName,
                uploadedFile.Length,
                uploadedFile.ContentType,
                fileBytes,
                notes);

            await _createImportDataCommand.Execute(importData);
        }

        [HttpGet]
        [Route("download")]
        public async Task<IActionResult> Download(long? id = null)
        {
            var importData = await _getImportDataQuery.GetLast(id);

            if (importData == null)
            {
                return NotFound("The file doesn't exist in the database!");
            }

            return File(importData.Data, importData.ContentType, importData.Name);
        }

        [HttpDelete]
        [Route("delete")]
        public async Task Delete()
        {
            await _deleteAllRacesCommand.Execute();
        }
    }
}
