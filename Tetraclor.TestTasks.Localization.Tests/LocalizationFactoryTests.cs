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
        LocalizationFactory localizationFactory;

        [SetUp]
        public void Setup()
        {
            dictSourceRu = new DictinaryLocalizationSourceRu();
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
    }
}