using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceCalendar.Api.Requests;
using RaceCalendar.Api.Responses;
using RaceCalendar.Api.Utils;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Queries;
using RaceCalendar.Domain.Services.Interfaces;
using System.Security;

namespace RaceCalendar.Api.Controllers;

[Route("v1/api/[Controller]")]
[ApiController]
public class EventCommentController : ControllerBase
{
    private readonly IEventCommentService _eventCommentService;
    private readonly IGetEventQuery _getEventQuery;

    public EventCommentController(
        IEventCommentService eventCommentService,
        IGetEventQuery getEventQuery)
    {
        _eventCommentService = eventCommentService ?? throw new ArgumentNullException(nameof(eventCommentService));
        _getEventQuery = getEventQuery ?? throw new ArgumentNullException(nameof(getEventQuery));
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task Create([FromBody] EventCommentCreateRequest request)
    {
        var eventComment = new EventComment(
            request.Id,
            request.EventId,
            request.ParentCommentId,
            request.Text,
            User.GetUserId(),
            DateTime.UtcNow,
            null);

        await _eventCommentService.Create(eventComment);
    }

    [HttpPut]
    [Route("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task Update([FromBody] UpdateEventCommentRequest request)
    {
        var eventComment = new EventComment(
            request.Id,
            request.EventId,
            request.ParentCommentId,
            request.Text,
            request.CreatedBy,
            DateTime.UtcNow,
            null);

        await _eventCommentService.Update(eventComment);
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task Delete([FromRoute] long id)
    {
        await _eventCommentService.Delete(id, User.GetUserId());
    }

    [HttpGet]
    public async Task<IEnumerable<EventCommentResponse>> GetComments([FromQuery] long eventId)
    {
        var @event = await _getEventQuery.QueryAsync(eventId);

        if (@event is null)
        {
            return Enumerable.Empty<EventCommentResponse>();
        }

        var isLoggedIn = HttpContext.Request.Headers.Authorization.FirstOrDefault() is not null;

        if (!@event.IsPublic && !isLoggedIn)
        {
            throw new SecurityException();
        }

        var comments = await _eventCommentService.GetCommentsByEvent(eventId);

        var repliesGroups = comments.GroupBy(c => c.ParentCommentId);

        var result = comments
            .Where(c => c.ParentCommentId is null)
            .Select(c =>
            {
                return new EventCommentResponse(
                    c.Id,
                    c.EventId,
                    c.Text,
                    repliesGroups
                        .FirstOrDefault(g => g.Key == c.Id)
                        ?.Select(c => new EventCommentResponse(
                            c.Id,
                            c.EventId,
                            c.Text,
                            null,
                            c.CreatedOn,
                            c.ModifiedOn,
                            c.CreatedBy,
                            c.CreatedByUser!.FirstName,
                            c.CreatedByUser!.LastName))
                        .ToList(),
                    c.CreatedOn,
                    c.ModifiedOn,
                    c.CreatedBy,
                    c.CreatedByUser!.FirstName,
                    c.CreatedByUser!.LastName);
            })
            .ToList();

        return result;
    }
}
