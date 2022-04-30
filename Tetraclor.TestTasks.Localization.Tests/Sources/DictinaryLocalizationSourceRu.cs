using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Globalization;

namespace Tetraclor.TestTasks.Localization
{
    public class DictinaryLocalizationSourceRu : ILocalizationSource
    {
        public CultureInfo CultureInfo { get; private set; } = CultureInfo.CreateSpecificCulture("ru-RU");
        Dictionary<string, string> _localizedStrings = new ()
        {
            ["Name"] = "Имя",
            ["Source"] = "Источник"
        };

        public LocalizedString GetString(string name)
        {
            return _localizedStrings.TryGetValue(name, out string value) 
                ? new LocalizedString(name, value) 
                : new LocalizedString(name, name, true);
        }
    }
}
