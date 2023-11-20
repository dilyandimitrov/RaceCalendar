using HtmlAgilityPack;
using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Strategies.Results;

public abstract class FetchResultStrategy
{
    public abstract string Url { get; }

    public abstract Task<(string position, string result, ResultTypes? type)> FetchResults(string firstName, string lastName, string url);

    public bool CanApply(string url) => url.Contains(Url);

    protected async Task<HtmlDocument> GetUrlContent(string url)
    {
        var htmlWeb = new HtmlWeb();
        htmlWeb.OverrideEncoding = System.Text.Encoding.UTF8;
        var content = await htmlWeb.LoadFromWebAsync(url);
        return content;
    }

    protected static string GetText(HtmlNodeCollection nodes, int index)
    {
        return nodes.Count > index ? nodes[index].InnerText.Trim() : null;
    }

    protected int? GetInt(HtmlNodeCollection nodes, int index)
    {
        var result = GetText(nodes, index);

        if (int.TryParse(result, out var resultInt))
        {
            return resultInt;
        }

        return null;
    }
}