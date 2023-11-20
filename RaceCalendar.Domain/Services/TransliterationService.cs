using RaceCalendar.Domain.Services.Interfaces;
using System.Text;
using System.Text.RegularExpressions;

namespace RaceCalendar.Domain.Services;

public class TransliterationService : ITransliterationService
{
    private const string ALL_CHARS_BG_REGEX = "[абвгдежзийклмнопрстуфхцчшщъьюя]+";
    private const string ALL_CHARS_LATIN_REGEX = "\\w+";

    private readonly IDictionary<string, string> cyrilicLatinMap = new Dictionary<string, string>()
        {
            {"а", "a"},
            {"б", "b"},
            {"в", "v"},
            {"г", "g"},
            {"д", "d"},
            {"е", "e"},
            {"ж", "zh"},
            {"з", "z"},
            {"и", "i"},
            {"й", "y"},
            {"к", "k"},
            {"л", "l"},
            {"м", "m"},
            {"н", "n"},
            {"о", "o"},
            {"п", "p"},
            {"р", "r"},
            {"с", "s"},
            {"т", "t"},
            {"у", "u"},
            {"ф", "f"},
            {"х", "h"},
            {"ц", "ts"},
            {"ч", "ch"},
            {"ш", "sh"},
            {"щ", "sht"},
            {"ъ", "a"},
            {"ь", "y"},
            {"ю", "yu"},
            {"я", "ya"}
        };

    private readonly IDictionary<string, string> latinCyrilicMap = new Dictionary<string, string>()
        {
            {"a", "а"},
            {"b", "б"},
            {"v", "в"},
            {"g", "г"},
            {"d", "д"},
            {"e", "е"},
            {"zh", "ж"},
            {"z", "з"},
            {"i", "и"},
            {"y", "й"},
            {"k", "к"},
            {"l", "л"},
            {"m", "м"},
            {"n", "н"},
            {"o", "о"},
            {"p", "п"},
            {"r", "р"},
            {"s", "с"},
            {"t", "т"},
            {"u", "у"},
            {"f", "ф"},
            {"h", "х"},
            {"ts", "ц"},
            {"ch", "ч"},
            {"sh", "ш"},
            {"sht", "щ"},
            {"yu", "ю"},
            {"ya", "я"}
        };

    public string GetWord(string word)
    {
        word = word.ToLower();
        StringBuilder result = new StringBuilder();
        if (IsBG(word))
        {
            var latinWords = word.ToCharArray().ToList().Select(x =>
            {
                if (cyrilicLatinMap.TryGetValue(x.ToString(), out var latin))
                {
                    return latin;
                }

                return x.ToString();
            });

            return string.Join("", latinWords);
        }
        else if (IsLatin(word))
        {
            var wordSb = new StringBuilder(word);
            var singleLettersOnly = wordSb
                .Replace("sht", "щ")
                .Replace("zh", "ж")
                .Replace("ts", "ц")
                .Replace("ch", "ч")
                .Replace("sh", "ш")
                .Replace("yu", "ю")
                .Replace("ya", "я");

            var cyrilicWord = singleLettersOnly.ToString().ToCharArray().ToList().Select(x =>
            {
                if (latinCyrilicMap.TryGetValue(x.ToString(), out var cyrilic))
                {
                    return cyrilic;
                }

                return x.ToString();
            });

            return string.Join("", cyrilicWord);
        }

        return result.ToString();
    }

    private bool IsBG(string word)
    {
        return new Regex(ALL_CHARS_BG_REGEX).IsMatch(word);
    }

    private bool IsLatin(string word)
    {
        return new Regex(ALL_CHARS_LATIN_REGEX).IsMatch(word);
    }
}
