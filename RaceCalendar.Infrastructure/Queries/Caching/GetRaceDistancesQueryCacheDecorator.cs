using Microsoft.Extensions.Caching.Memory;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Queries.Caching
{
    public class GetRaceDistancesQueryCacheDecorator : IGetRaceDistancesQuery
    {
        private readonly IGetRaceDistancesQuery _getRaceDistancesQuery;
        private readonly IMemoryCache _memoryCache;

        public GetRaceDistancesQueryCacheDecorator(
            IGetRaceDistancesQuery getRaceDistancesQuery,
            IMemoryCache memoryCache)
        {
            _getRaceDistancesQuery = getRaceDistancesQuery ?? throw new ArgumentNullException(nameof(getRaceDistancesQuery));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public Task<IEnumerable<RaceDistance>> Get(int raceId, ISet<int>? raceDistanceIds = null)
        {
            var cacheKey = $"distances-{raceId}";

            return _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _getRaceDistancesQuery.Get(raceId, raceDistanceIds);
            });
        }
    }
}
