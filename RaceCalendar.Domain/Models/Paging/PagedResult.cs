namespace RaceCalendar.Domain.Models.Paging;

public class PagedResult<T> : PagedResultBase where T : class
{
    public PagedResult(int currentPage, int pageSize, int rowCount, IEnumerable<T> results)
        : base(currentPage, pageSize, rowCount)
    {
        Results = results;
    }

    public IEnumerable<T> Results { get; }
}
