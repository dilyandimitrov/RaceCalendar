using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Services.Interfaces;

public interface ISearchRacesValidatorService
{
    void ValidateSearch(GetRacesFilterInput filter);
}