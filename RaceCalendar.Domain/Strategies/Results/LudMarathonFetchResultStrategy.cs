using System;
using System.Linq;
using System.Threading.Tasks;
using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Strategies.Results;

public class LudMarathonFetchResultStrategy : FetchResultStrategy
{
    public LudMarathonFetchResultStrategy()
    {
    }

    public override string Url => "https://ludmarathon.com/";

    public async override Task<(string position, string result, ResultTypes? type)> FetchResults(string firstName, string lastName, string url)
    {
        var content = await GetUrlContent(url);

        var matchingNamesNode = content.DocumentNode.SelectNodes("//td").SingleOrDefault(x =>
            x.InnerText.Trim().Contains(firstName, StringComparison.InvariantCultureIgnoreCase) &&
            x.InnerText.Trim().Contains(lastName, StringComparison.InvariantCultureIgnoreCase));

        if (matchingNamesNode == null)
        {
            return (null, null, null);
        }

        var matchingRow = matchingNamesNode.ParentNode;

        var position = GetText(matchingRow.ChildNodes, 1);
        var result = GetText(matchingRow.ChildNodes, 25);

        return (position, result, ResultTypes.Time);
    }
}