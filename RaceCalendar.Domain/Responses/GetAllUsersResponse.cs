namespace RaceCalendar.Domain.Responses
{
    public class GetAllUsersResponse
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public IEnumerable<GetAllUserRacesResponse> Races { get; set; }
    }
}