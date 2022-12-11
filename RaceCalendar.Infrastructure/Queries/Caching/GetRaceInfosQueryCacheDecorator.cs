using Microsoft.Extensions.Caching.Memory;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Queries.Caching;

public class GetRaceInfosQueryCacheDecorator : IGetRaceInfosQuery
{
    private readonly IGetRaceInfosQuery _getRaceInfosQuery;
    private readonly IMemoryCache _memoryCache;

    public GetRaceInfosQueryCacheDecorator(
        IGetRaceInfosQuery getRaceInfosQuery,
        IMemoryCache memoryCache)
    {
        _getRaceInfosQuery = getRaceInfosQuery ?? throw new ArgumentNullException(nameof(getRaceInfosQuery));
        _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
    }

    public Task<IDictionary<int, IEnumerable<RaceInfo>>> Get(int raceId, ISet<int> raceDistanceIds)
    {
        var cacheKey = $"raceinfos-{raceId}";

        return _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            return await _getRaceInfosQuery.Get(raceId, raceDistanceIds);
        });
    }
}
