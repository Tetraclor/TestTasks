using NUnit.Framework;
using FluentAssertions;
using Tetraclor.TestTasks.Localization;
using System.Globalization;

namespace Tetraclor.TestTasks.Localization.Tests
{
    /*
��� desktop-���������� ����� ����������� ����� ������� ��� ������ � ��������������� ��������, ������� ����� ������������ � ���������� ������������. 
����������:
    1. �������������� �������� ������ ������ ���� �������� �� ���� ������ ����������� � ����������� ������� CultureInfo.
    2. ���� ��� ������� ��������������� �������� ������ ������ CultureInfo �� ��� �������, �� ���������� ������������ ������� �������� ������.
    3. � �������� ��������� ����� ����� ��������� ��� ������� ������� ������, ��� � ��������� ��������� (��, XML-���� ��� ����).
    4. ���������� ������������� ����������� ����������� ������������� ��������� �����.
    5. ������� ����������� ������ �������������� ���������� �������� ����� �� ����������� ���� ������ ����������� (���� ����� ���� ������).
    6. ������ ��������� ����� ����� ����������� �� ����� �����, ������� ������ ��������� ���������� �� ������ ��������� ����� ������ � ������ ���������� ������.
    7. ������������� ����������� �������� ����� ����������� � ����� ������ ��� �������� � ���������� �������� � ������������ ������������ ��������� �������� ������ ��� ������� �����.
����������
    ��� ������ � ������������� ���������� ����������� ����� LocalizationFactory, ������� ������������� ��������� ������:
GetString � ���������� �������� ������ ����������� �� � ���� ��� ���������� ��������;
RegisterSource � ������������ �������� ����� �����������.

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
        [TestCase("Name", "���", "ru-RU")]
        [TestCase("Source", "��������", "ru-RU")]
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
        [TestCase("Name", "���", "ru-RU")]
        [TestCase("Source", "��������", "ru-RU")]
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
        [TestCase("Name", "���", "ru-RU")]
        [TestCase("Source", "��������", "ru-RU")]
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
        [TestCase("Name", "���", "ru-RU")]
        [TestCase("Source", "������", "ru-RU")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "ru-RU")]
        [TestCase("Name", "der Name", "de-DE")]
        [TestCase("Source", "der Quelle", "de-DE")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "de-DE")]
        public void TestIntersectSources(string name, string localizedString, string culture)
        {
            localizationFactory.RegisterSource(dictSourceRu);
            localizationFactory.RegisterSource(dictSourceDe);
            localizationFactory.RegisterSource(dictSourceEn);
            // ������ ������� �������������� ����� �������������� �������� �� ��������� ������
            localizationFactory.RegisterSource(
                new DictionaryLocalizationSource(CultureInfo.GetCultureInfo("ru-RU"),
                new() {
                    ["Source"] = "������",
                    ["Name"] = "���"
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
        [TestCase("Name", "���", "ru-RU")]
        [TestCase("Source", "��������", "ru-RU")]
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

        [TestCase("Name", "���", "Name", "ru-RU")]
        [TestCase("Source", "��������", "������", "ru-RU")]
        [TestCase("Phone", "�������", "�������", "ru-RU")]
        [TestCase("Header", "Header", "������� ��������", "ru-RU")]
        [TestCase("NotFoundInSource", "NotFoundInSource", "NotFoundInSource", "ru-RU")]
        public void TestWhenSourceStateChange(string name, string localizedString, string localizedStringSourceChange, string culture)
        {
            var dictSource = new DictionaryLocalizationSource(CultureInfo.GetCultureInfo(culture),
               new()
               {
                   ["Source"] = "��������",
                   ["Name"] = "���",
                   ["Phone"] = "�������"
               });

            localizationFactory.RegisterSource(dictSource);

            var result = localizationFactory.GetString(name, CultureInfo.GetCultureInfo(culture));
            
            result.Should().BeEquivalentTo(localizedString);

            dictSource._localizedStrings["Header"] = "������� ��������";
            dictSource._localizedStrings["Source"] = "������";
            dictSource._localizedStrings.Remove("Name");

            result = localizationFactory.GetString(name, CultureInfo.GetCultureInfo(culture));

            result.Should().BeEquivalentTo(localizedStringSourceChange);
        }
    }
}