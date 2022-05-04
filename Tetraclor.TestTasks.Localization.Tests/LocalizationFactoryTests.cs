using NUnit.Framework;
using FluentAssertions;
using Tetraclor.TestTasks.Localization;
using System.Globalization;

namespace Tetraclor.TestTasks.Localization.Tests
{
    /*
Для desktop-приложения нужно разработать набор классов для работы с локализованными строками, которые будут отображаться в интерфейсе пользователя. 
Требования:
    1. Локализованное значение строки должно быть доступно по коду строки локализации и переданному объекту CultureInfo.
    2. Если при запросе локализованного значения строки объект CultureInfo не был передан, то необходимо использовать текущую культуру потока.
    3. В качестве источника строк могут выступать как ресурсы текущей сборки, так и сторонние источники (БД, XML-файл или иное).
    4. Необходимо предусмотреть возможность подключения произвольного источника строк.
    5. Система локализации должна самостоятельно определять источник строк по переданному коду строки локализации (коды могут быть любыми).
    6. Разные источники могут иметь пересечения по кодам строк, система должна корректно определять из какого источника брать строку в каждом конкретном случае.
    7. Предусмотреть возможность хранения строк локализаций в полях класса без привязки к конкретной культуре с возможностью последующего получения значения строки для нужного языка.
Реализация
    Для работы с локализациями необходимо разработать класс LocalizationFactory, который предоставляет следующие методы:
GetString – возвращает значение строки локализации по её коду для переданной культуры;
RegisterSource – регистрирует источник строк локализаций.

*/
    public class LocalizationFactoryTests
    {
        ILocalizationSource dictSourceRu;
        ILocalizationSource dictSourceEn;
        ILocalizationSource dictSourceDe;

        ILocalizationSource resourceRu;

        LocalizationFactory localizationFactory;

        [SetUp]
        public void Setup()
        {
            dictSourceRu = DictionaryLocalizationSource.Ru;
            dictSourceEn = DictionaryLocalizationSource.En;
            dictSourceDe = DictionaryLocalizationSource.De;

            localizationFactory = new LocalizationFactory();
        }

        [TestCase(null, null)]
        [TestCase("", "")]
        [TestCase("Name", "Name")]
        public void TestIfLocalAndSourceNull(string name, string localizedString)
        {
            var result = localizationFactory.GetString(name);

            result.Should().BeEquivalentTo(localizedString);
        }

        [TestCase(null, null, "en-EN")]
        [TestCase("", "", "en-EN")]
        [TestCase("Name", "Name", "en-EN")]
        [TestCase(null, null, "de-DE")]
        [TestCase("", "", "de-DE")]
        [TestCase("Name", "Name", "de-DE")]
        public void TestIfSourceNull(string name, string localizedString, string culture)
        {
            var result = localizationFactory.GetString(name, CultureInfo.GetCultureInfo(culture));

            result.Should().BeEquivalentTo(localizedString);
        }

        [TestCase(null, null, "en-EN")]
        [TestCase("", "", "en-EN")]
        [TestCase("Name", "Name", "en-EN")]
        [TestCase("Source", "Source", "en-EN")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "en-EN")]
        [TestCase(null, null, "ru-RU")]
        [TestCase("", "", "ru-RU")]
        [TestCase("Name", "Имя", "ru-RU")]
        [TestCase("Source", "Источник", "ru-RU")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "en-EN")]
        public void TestAddOneRuSourceIfCultureNull(string name, string localizedString, string currentCulture)
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(currentCulture);
            localizationFactory.RegisterSource(dictSourceRu);

            var result = localizationFactory.GetString(name);

            result.Should().BeEquivalentTo(localizedString);
        }

        [TestCase(null, null, "en-EN")]
        [TestCase("", "", "en-EN")]
        [TestCase("Name", "Name", "en-EN")]
        [TestCase("Source", "Source", "en-EN")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "en-EN")]
        [TestCase(null, null, "ru-RU")]
        [TestCase("", "", "ru-RU")]
        [TestCase("Name", "Имя", "ru-RU")]
        [TestCase("Source", "Источник", "ru-RU")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "ru-RU")]
        public void TestAddOneRuSource(string name, string localizedString, string culture)
        {
            localizationFactory.RegisterSource(dictSourceRu);

            var result = localizationFactory.GetString(name, CultureInfo.GetCultureInfo(culture));

            result.Should().BeEquivalentTo(localizedString);
        }

        [TestCase("Name", "Name", "en-EN")]
        [TestCase("Source", "Source", "en-EN")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "en-EN")]
        [TestCase("Name", "Имя", "ru-RU")]
        [TestCase("Source", "Источник", "ru-RU")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "ru-RU")]
        [TestCase("Name", "der Name", "de-DE")]
        [TestCase("Source", "der Quelle", "de-DE")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "de-DE")]
        public void TestAddThreeSource(string name, string localizedString, string culture)
        {
            localizationFactory.RegisterSource(dictSourceRu);
            localizationFactory.RegisterSource(dictSourceDe);
            localizationFactory.RegisterSource(dictSourceEn);

            var result = localizationFactory.GetString(name, CultureInfo.GetCultureInfo(culture));

            result.Should().BeEquivalentTo(localizedString);
        }

        [TestCase("Name", "Name", "en-EN")]
        [TestCase("Source", "Source", "en-EN")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "en-EN")]
        [TestCase("Name", "ФИО", "ru-RU")]
        [TestCase("Source", "Родник", "ru-RU")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "ru-RU")]
        [TestCase("Name", "der Name", "de-DE")]
        [TestCase("Source", "der Quelle", "de-DE")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "de-DE")]
        public void TestIntersectSources(string name, string localizedString, string culture)
        {
            localizationFactory.RegisterSource(dictSourceRu);
            localizationFactory.RegisterSource(dictSourceDe);
            localizationFactory.RegisterSource(dictSourceEn);
            // Ресурс который регистрируется позже переопределяет значения по одинковым ключам
            localizationFactory.RegisterSource(
                new DictionaryLocalizationSource(CultureInfo.GetCultureInfo("ru-RU"),
                new() {
                    ["Source"] = "Родник",
                    ["Name"] = "ФИО"
                }));

            var result = localizationFactory.GetString(name, CultureInfo.GetCultureInfo(culture));

            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo(culture);
            var resultIfCultureNull = localizationFactory.GetString(name);

            result.Should().BeEquivalentTo(localizedString);
            resultIfCultureNull.Should().BeEquivalentTo(localizedString);
        }

        [TestCase("Name", "Name", "en-EN")]
        [TestCase("Source", "Source", "en-EN")]
        [TestCase("Header", "Main Page", "en-EN")]
        [TestCase("Name", "Имя", "ru-RU")]
        [TestCase("Source", "Источник", "ru-RU")]
        [TestCase("Header", "Header", "ru-RU")]
        public void TestDetermSource(string name, string localizedString, string culture)
        {
            localizationFactory.RegisterSource(dictSourceRu);
            localizationFactory.RegisterSource(
               new DictionaryLocalizationSource(CultureInfo.GetCultureInfo("en-EN"),
               new()
               {
                   ["Source"] = "Source",
                   ["Name"] = "Name",
                   ["Header"] = "Main Page"
               }));
            var result = localizationFactory.GetString(name, CultureInfo.GetCultureInfo(culture));

            result.Should().BeEquivalentTo(localizedString);
        }

        [TestCase("Name", "Имя", "Name", "ru-RU")]
        [TestCase("Source", "Источник", "Родник", "ru-RU")]
        [TestCase("Phone", "Телефон", "Телефон", "ru-RU")]
        [TestCase("Header", "Header", "Главная страница", "ru-RU")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "NotFoundInSource", "ru-RU")]
        public void TestWhenSourceStateChange(string name, string localizedString, string localizedStringSourceChange, string culture)
        {
            var dictSource = new DictionaryLocalizationSource(CultureInfo.GetCultureInfo(culture),
               new()
               {
                   ["Source"] = "Источник",
                   ["Name"] = "Имя",
                   ["Phone"] = "Телефон"
               });

            localizationFactory.RegisterSource(dictSource);

            var result = localizationFactory.GetString(name, CultureInfo.GetCultureInfo(culture));
            
            result.Should().BeEquivalentTo(localizedString);

            dictSource._localizedStrings["Header"] = "Главная страница";
            dictSource._localizedStrings["Source"] = "Родник";
            dictSource._localizedStrings.Remove("Name");

            result = localizationFactory.GetString(name, CultureInfo.GetCultureInfo(culture));

            result.Should().BeEquivalentTo(localizedStringSourceChange);
        }
    }
}