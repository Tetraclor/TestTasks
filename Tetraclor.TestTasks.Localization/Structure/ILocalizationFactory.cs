using System.Globalization;

namespace Tetraclor.TestTasks.Localization
{
    public interface ILocalizationFactory
    {
        string GetString(string name, CultureInfo cultureInfo = null);
        LocalizationFactory RegisterSource(ILocalizationSource localizationSource);
    }
}
