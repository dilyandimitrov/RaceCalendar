﻿using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface IRaceService
{
    Task Update(Race race);
    Task<Race?> Get(string nameId, ISet<int>? distanceIds = null);
    Task<Race?> Get(int raceId, ISet<int>? distanceIds = null);
    Task Delete(int raceId);
}
