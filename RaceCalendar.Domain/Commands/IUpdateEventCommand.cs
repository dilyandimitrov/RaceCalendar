﻿using RaceCalendar.Domain.Models.Events;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateEventCommand
{
    Task Execute(Event @event);
}
