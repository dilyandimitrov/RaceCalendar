using System;
using System.Linq;
using System.Threading.Tasks;
using RaceCalendar.Domain.Models;

namespace RaceCalendar.Domain.Strategies.Results;

public class RaceTrackingFetchResultStrategy : FetchResultStrategy
{
    public RaceTrackingFetchResultStrategy()
    {
    }

    public override string Url => "race-tracking.com";

    public async override Task<(string position, string result, ResultTypes? type)> FetchResults(string firstName, string lastName, string url)
    {
        var (positionM, resultM, typeM) = await FetchResultsInternal(firstName, lastName, url + "&gender=M");

        if (positionM is not null && resultM is not null && typeM is not null)
        {
            return (positionM, resultM, typeM);
        }

        return await FetchResultsInternal(firstName, lastName, url + "&gender=F");
    }

    private async Task<(string position, string result, ResultTypes? type)> FetchResultsInternal(string firstName, string lastName, string url)
    {
        url = url.Replace("race.php", "results.php");
        var content = await GetUrlContent(url);

        var matchingNamesNode = content.DocumentNode.SelectNodes("//td").SingleOrDefault(x =>
            x.HasClass("t5") &&
            x.InnerText.Trim().Contains(firstName, StringComparison.InvariantCultureIgnoreCase) &&
            x.InnerText.Trim().Contains(lastName, StringComparison.InvariantCultureIgnoreCase));

        if (matchingNamesNode == null)
        {
            return (null, null, null);
        }

        try
        {
            var matchingRow = matchingNamesNode.ParentNode;

            var position = GetText(matchingRow.ChildNodes, 0);
            var result = GetText(matchingRow.ChildNodes, 5);

            if (content.DocumentNode.InnerHtml.Contains("RATCU"))
            {
                result = matchingRow.ChildNodes[6].ChildNodes[0].InnerText.TrimEnd('k', 'm').Trim();
                return (position, result, ResultTypes.Distance);
            }

            return (position, result, ResultTypes.Time);
        }
        catch (Exception)
        {
            return (null, null, null);
        }
    }
}