using Microsoft.Extensions.Localization;
using System.Globalization;


namespace Tetraclor.TestTasks.Localization
{
    public interface ILocalizationSource
    {
        CultureInfo CultureInfo { get; }
        LocalizedString GetString(string name);
    }
}
