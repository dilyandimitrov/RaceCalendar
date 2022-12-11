using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Services.Interfaces;
using System;

namespace RaceCalendar.Domain.Services;

public class SearchRacesValidatorService : ISearchRacesValidatorService
{
    public void ValidateSearch(GetRacesFilterInput filter)
    {
        if (!filter.FromDate.HasValue && !filter.ToDate.HasValue)
        {
            if (string.IsNullOrEmpty(filter.Text))
            {
                throw new InvalidOperationException("You must provide search text.");
            }

            if (filter.Text.Trim().Length < 3)
            {
                throw new InvalidOperationException("Search text should be 3 or more characters.");
            }
        }

        if (filter.ShowPrevious.HasValue && !filter.FromDate.HasValue)
        {
            throw new InvalidOperationException("Not supported");
        }
    }
}
