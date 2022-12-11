using System;

namespace RaceCalendar.Domain.Models.Paging;

public abstract class PagedResultBase
{
    protected PagedResultBase(int currentPage, int pageSize, int rowCount)
    {
        CurrentPage = currentPage;
        PageSize = pageSize;
        RowCount = rowCount;
    }

    public int CurrentPage { get; }
    public int PageSize { get; }
    public int RowCount { get; }

    public int FirstRowOnPage
    {
        get { return (CurrentPage - 1) * PageSize + 1; }
    }

    public int LastRowOnPage
    {
        get { return Math.Min(CurrentPage * PageSize, RowCount); }
    }

    public int PageCount
    {
        get
        {
            var pageCount = (double)RowCount / PageSize;
            return (int)Math.Ceiling(pageCount);
        }
    }
}
