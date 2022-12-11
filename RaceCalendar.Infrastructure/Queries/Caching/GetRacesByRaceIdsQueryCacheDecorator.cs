using Microsoft.Extensions.Caching.Memory;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Queries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RaceCalendar.Infrastructure.Queries.Caching
{
    public class GetRacesByRaceIdsQueryCacheDecorator : IGetRacesByRaceIdsQuery
    {
        private readonly IGetRacesByRaceIdsQuery _getRacesByRaceIdsQuery;
        private readonly IMemoryCache _memoryCache;

        public GetRacesByRaceIdsQueryCacheDecorator(
            IGetRacesByRaceIdsQuery getRacesByRaceIdsQuery,
            IMemoryCache memoryCache)
        {
            _getRacesByRaceIdsQuery = getRacesByRaceIdsQuery ?? throw new ArgumentNullException(nameof(getRacesByRaceIdsQuery));
            _memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
        }

        public Task<IReadOnlyDictionary<int, Race>> Get(ISet<int> raceIds)
        {
            var cacheKey = $"races-{string.Join("-", raceIds.Distinct())}";

            return _memoryCache.GetOrCreateAsync(cacheKey, async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _getRacesByRaceIdsQuery.Get(raceIds);
            });
        }
    }
}
