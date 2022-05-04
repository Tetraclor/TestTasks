using Microsoft.Extensions.Localization;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Threading;
using System;

/*
Для desktop-приложения нужно разработать набор классов для работы с локализованными строками, которые будут отображаться в интерфейсе пользователя. 
Требования:
   + 1. Локализованное значение строки должно быть доступно по коду строки локализации и переданному объекту CultureInfo.
   + 2. Если при запросе локализованного значения строки объект CultureInfo не был передан, то необходимо использовать текущую культуру потока.
   +? 3. В качестве источника строк могут выступать как ресурсы текущей сборки, так и сторонние источники (БД, XML-файл или иное).
   + 4. Необходимо предусмотреть возможность подключения произвольного источника строк.
   + 5. Система локализации должна самостоятельно определять источник строк по переданному коду строки локализации (коды могут быть любыми).
   + 6. Разные источники могут иметь пересечения по кодам строк, система должна корректно определять из какого источника брать строку в каждом конкретном случае.
   ? 7. Предусмотреть возможность хранения строк локализаций в полях класса без привязки к конкретной культуре с возможностью последующего получения значения строки для нужного языка.
Реализация
    Для работы с локализациями необходимо разработать класс LocalizationFactory, который предоставляет следующие методы:
GetString – возвращает значение строки локализации по её коду для переданной культуры;
RegisterSource – регистрирует источник строк локализаций.

*/

namespace Tetraclor.TestTasks.Localization
{
    /// <summary>
    /// Версия ILocalizationFactory в реальном времени, 
    /// то есть локализированные строки не кешируются из источников, 
    /// а извлекаются каждый раз при запросе
    /// </summary>
    public class LocalizationFactory : ILocalizationFactory
    {
        int lastOrder = 0;

        readonly Dictionary<CultureInfo, List<ILocalizationSource>> _localizationSources = new();
        readonly Dictionary<ILocalizationSource, int> _localizationSourcesOrder = new();

        ILocalizationSource _defaultLocalizationSource = new DefaultLocalizationSource();

        /// <summary>
        /// Локализация name в соотсветсвии с переданным cultureInfo, если не передано,
        /// то используется CultureInfo.DefaultThreadCurrentCulture
        /// </summary>
        /// <param name="name">Код строки которую нужно локализировать</param>
        /// <param name="cultureInfo"></param>
        /// <returns></returns>
        public string GetString(string name, CultureInfo cultureInfo = null)
        {
            if (name == null) return null;
            if (cultureInfo == null) cultureInfo = Thread.CurrentThread.CurrentCulture;

            var localizedString = new LocalizedString(name, name);

            if (_localizationSources.TryGetValue(cultureInfo, out List<ILocalizationSource> localizationSourcesForCultureInfo) == false)
            {
                return localizedString;
            }

            if(localizationSourcesForCultureInfo.Count != 0) 
            {
                localizedString = localizationSourcesForCultureInfo
                    .OrderByDescending(v => _localizationSourcesOrder[v])
                    .Select(v => v.GetString(name))
                    .Where(v => v.ResourceNotFound == false)
                    .FirstOrDefault()
                    ?? localizedString;
            }

            return localizedString;
        }

        /// <summary>
        /// Регистрация источника локализированных строк, порядок регистарции имеет значение, 
        /// при пересечении кода строки источники зарегестрированные позднее в приоритете.
        /// </summary>
        /// <param name="localizationSource"></param>
        /// <returns>this</returns>
        public ILocalizationFactory RegisterSource(ILocalizationSource localizationSource)
        {
            if (localizationSource == null) throw new ArgumentNullException(nameof(localizationSource), $"{nameof(localizationSource)} cannot be null"); ;

            if (_localizationSources.TryGetValue(localizationSource.CultureInfo, out List<ILocalizationSource> sourcesForCultureInfo) == false)
            {
                sourcesForCultureInfo = new List<ILocalizationSource>();
                _localizationSources[localizationSource.CultureInfo] = sourcesForCultureInfo;
            }
            sourcesForCultureInfo.Add(localizationSource);

            _localizationSourcesOrder[localizationSource] = lastOrder++;
            return this;
        }

        private class DefaultLocalizationSource : ILocalizationSource
        {
            public CultureInfo CultureInfo => CultureInfo.InvariantCulture;

            public LocalizedString GetString(string name) => new LocalizedString(name, name);
        }
    }
}


