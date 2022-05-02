using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;
using System.Resources;

namespace Tetraclor.TestTasks.Localization
{
    public class DictionaryLocalizationSource : ILocalizationSource
    {
        public CultureInfo CultureInfo { get; private set; } = CultureInfo.InvariantCulture;
        Dictionary<string, string> _localizedStrings = new();


        public static DictionaryLocalizationSource Ru = new DictionaryLocalizationSource(
            CultureInfo.GetCultureInfo("ru-RU"),
            new(){
                ["Name"] = "Имя",
                ["Source"] = "Источник"
            });

        public static DictionaryLocalizationSource En = new DictionaryLocalizationSource(
           CultureInfo.GetCultureInfo("en-EN"),
           new()
           {
               ["Name"] = "Name",
               ["Source"] = "Source"
           });

        public static DictionaryLocalizationSource De = new DictionaryLocalizationSource(
            CultureInfo.GetCultureInfo("de-DE"),
            new()
            {
                ["Name"] = "der Name",
                ["Source"] = "der Quelle"
            });


        public DictionaryLocalizationSource(CultureInfo cultureInfo, Dictionary<string, string> localizedStrings)
        {
            CultureInfo = cultureInfo;
            _localizedStrings = localizedStrings;
        }

        public LocalizedString GetString(string name)
        {
            return _localizedStrings.TryGetValue(name, out string value) 
                ? new LocalizedString(name, value) 
                : new LocalizedString(name, name, true);
        }
    }
}
