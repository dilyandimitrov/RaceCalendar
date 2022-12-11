using System;

namespace RaceCalendar.Domain.Responses
{
    public class GetAllUserRacesResponse
    {
        public string UserId { get; set; }
        public string Name { get; set; }
        public string NameId { get; set; }
        public DateTime? StartDate { get; set; }
        public string Distance { get; set; }
    }
}