using System;
using System.Linq;
using System.Threading.Tasks;
using RaceCalendar.Domain.Models;
using RaceCalendar.Domain.Strategies.Results;

namespace RaceCalendar.Domain.Strategies.Results;

public class FiveKmRunFetchResultStrategy : FetchResultStrategy
{
    public FiveKmRunFetchResultStrategy()
    {
    }

    public override string Url => "https://5kmrun.bg/";

    public async override Task<(string position, string result, ResultTypes? type)> FetchResults(string firstName, string lastName, string url)
    {
        var content = await GetUrlContent(url);

        var names = $"{firstName} {lastName}";
        var matchingNamesNode = content.DocumentNode.SelectNodes("//td[@data-title='Име']").SingleOrDefault(x =>
            x.InnerText.Trim().Contains(names, StringComparison.InvariantCultureIgnoreCase));

        if (matchingNamesNode == null)
        {
            return (null, null, null);
        }

        var matchingRow = matchingNamesNode.ParentNode;

        var position = GetText(matchingRow.ChildNodes, 1);
        var result = GetText(matchingRow.ChildNodes, 7);

        return (position, result, ResultTypes.Time);
    }
}