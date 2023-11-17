﻿using RaceCalendar.Domain.Models.Events;
using System.Threading.Tasks;

namespace RaceCalendar.Domain.Commands;

public interface IUpdateEventCommand
{
    Task Execute(Event @event);
}