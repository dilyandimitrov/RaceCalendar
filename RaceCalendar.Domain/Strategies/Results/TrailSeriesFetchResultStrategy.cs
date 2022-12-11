using System;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Strategies.Results;

public class TrailSeriesFetchResultStrategy : FetchResultStrategy
{
    public TrailSeriesFetchResultStrategy()
    {
    }

    public override string Url => "https://trailseries.bg/";

    public async override Task<(string position, string result, ResultTypes? type)> FetchResults(string firstName, string lastName, string url)
    {
        var content = await GetUrlContent(url);

        var allMatchingFirstName = content.DocumentNode.SelectNodes("//td").Where(x => string.Equals(x.InnerHtml, firstName, StringComparison.InvariantCultureIgnoreCase)).ToList();

        HtmlNode matchingRow = null;
        foreach (var node in allMatchingFirstName)
        {
            var parent = node.ParentNode;
            if (parent.ChildNodes.SingleOrDefault(x => string.Equals(x.InnerHtml, lastName, StringComparison.InvariantCultureIgnoreCase)) != null)
            {
                matchingRow = parent;
                break;
            }
        }

        if (matchingRow == null)
        {
            return (null, null, null);
        }

        var result = "";
        var index = matchingRow.ChildNodes.Count - 2;
        do
        {
            result = matchingRow.ChildNodes[index--].InnerText.Trim();
        } while (string.IsNullOrWhiteSpace(result));

        var position = GetText(matchingRow.ChildNodes, 1);

        return (position, result, ResultTypes.Time);
    }
}