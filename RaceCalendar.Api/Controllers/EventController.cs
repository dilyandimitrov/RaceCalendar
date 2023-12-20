using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceCalendar.Api.Requests;
using RaceCalendar.Api.Utils;
using RaceCalendar.Domain.Commands;
using RaceCalendar.Domain.Models.Events;
using RaceCalendar.Domain.Services.Interfaces;
using System.Security;

namespace RaceCalendar.Api.Controllers;

[Route("v1/api/[Controller]")]
[ApiController]
public class EventController : ControllerBase
{
    private readonly IEventService _eventService;
    private readonly ISearchEventsService _searchEventsService;
    private readonly IUpdateEventVisitorsCountCommand _updateEventVisitorsCountCommand;

    public EventController(
        IEventService eventService,
        ISearchEventsService searchEventsService,
        IUpdateEventVisitorsCountCommand updateEventVisitorsCountCommand)
    {
        _eventService = eventService ?? throw new ArgumentNullException(nameof(eventService));
        _searchEventsService = searchEventsService ?? throw new ArgumentNullException(nameof(searchEventsService));
        _updateEventVisitorsCountCommand = updateEventVisitorsCountCommand ?? throw new ArgumentNullException(nameof(updateEventVisitorsCountCommand));
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task Create([FromBody] CreateEventRequest eventRequest)
    {
        var @event = new Event(
            eventRequest.Id,
            eventRequest.Name,
            eventRequest.Description,
            eventRequest.City,
            eventRequest.Latitude,
            eventRequest.Longitude,
            eventRequest.StartDate,
            eventRequest.StartTime.ToTimeSpan(),
            eventRequest.Distance,
            eventRequest.ElevationGain,
            eventRequest.Link,
            eventRequest.Terrain,
            eventRequest.Cancelled,
            eventRequest.IsPublic,
            eventRequest.MaxParticipants,
            eventRequest.Contact,
            User.GetUserId(),
            DateTime.UtcNow,
            null);

        await _eventService.Create(@event);
    }

    [HttpPut]
    [Route("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task Update([FromBody] UpdateEventRequest eventRequest)
    {
        var @event = new Event(
            eventRequest.Id,
            eventRequest.Name,
            eventRequest.Description,
            eventRequest.City,
            eventRequest.Latitude,
            eventRequest.Longitude,
            eventRequest.StartDate,
            eventRequest.StartTime.ToTimeSpan(),
            eventRequest.Distance,
            eventRequest.ElevationGain,
            eventRequest.Link,
            eventRequest.Terrain,
            eventRequest.Cancelled,
            eventRequest.IsPublic,
            eventRequest.MaxParticipants,
            eventRequest.Contact,
            eventRequest.CreatedBy,
            eventRequest.CreatedOn,
            null);

        await _eventService.Update(@event);
    }

    [HttpDelete]
    [Route("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task Delete([FromRoute] long id)
    {
        await _eventService.Delete(id, User.GetUserId());
    }

    [HttpGet]
    [Route("get/{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [AllowAnonymous]
    public async Task<ActionResult<Event>> Get([FromRoute] long id)
    {
        var @event = await _eventService.Get(id);

        if (@event is null)
        {
            return NoContent();
        }

        var isLoggedIn = HttpContext.Request.Headers.Authorization.FirstOrDefault() is not null;
        if (!@event.IsPublic && !isLoggedIn)
        {
            throw new SecurityException();
        }

        if (!isLoggedIn || @event.CreatedBy != User.GetUserId())
        {
            var visitorsCount = (@event.VisitorsCount ?? 0) + 1;
            await _updateEventVisitorsCountCommand.Execute(@event.Id, visitorsCount);
            @event.VisitorsCount = visitorsCount;
        }

        if (!User.Claims.Any() || User.GetUserId() != @event.CreatedBy)
        {
            @event.VisitorsCount = null;
        }

        return @event;
    }

    [HttpGet]
    [Route("get")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public async Task<IEnumerable<Event>> Get()
    {
        return await _searchEventsService.GetUpcomingEvents();
    }

    [HttpGet]
    [Route("get-public")]
    public async Task<IEnumerable<Event>> GetPublic()
    {
        return await _searchEventsService.GetUpcomingEvents(true);
    }
}
