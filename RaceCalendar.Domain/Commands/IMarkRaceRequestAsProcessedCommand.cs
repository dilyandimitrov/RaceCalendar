﻿namespace RaceCalendar.Domain.Commands;

public interface IMarkRaceRequestAsProcessedCommand
{
    Task Execute(int raceRequestId);
}
