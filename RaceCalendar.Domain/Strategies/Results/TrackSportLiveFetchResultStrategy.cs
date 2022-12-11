using System;
using System.Linq;
using System.Threading.Tasks;
using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Strategies.Results;

public class TrackSportLiveFetchResultStrategy : FetchResultStrategy
{
    public TrackSportLiveFetchResultStrategy()
    {
    }

    public override string Url => "tracksport";

    public async override Task<(string position, string result, ResultTypes? type)> FetchResults(string firstName, string lastName, string url)
    {
        var content = await GetUrlContent(url);

        var names = $"{firstName} {lastName}";
        var matchingNamesNode = content.DocumentNode.SelectNodes("//td").SingleOrDefault(x => string.Equals(x.InnerText.Trim(), names, StringComparison.InvariantCultureIgnoreCase));

        if (matchingNamesNode == null)
        {
            return (null, null, null);
        }

        var matchingRow = matchingNamesNode.ParentNode;

        if (matchingRow.HasClass("dnf"))
        {
            return ("DNF", null, null);
        }

        if (matchingRow.HasClass("dns"))
        {
            return ("DNS", null, null);
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