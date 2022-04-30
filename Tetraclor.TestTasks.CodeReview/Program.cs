using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;

/*

Дан исходный код функции. Функция выполняет поиск значения переданного атрибута элемента Xml-документа.
На вход функции передается путь до Xml-файла;
    название элемента (в котором ищется значение);
    название атрибута,
    значение которого необходимо получить.
Необходимо указать на недостатки и возможные ошибки исходного кода и предложить свою реализацию алгоритма.

 */

namespace Tetraclor.TestTasks.CodeReview
{
    public class Program
    {
        static void Main(string[] args)
        {
            SomeClass.GetAttributeValueFromXmlFile("example.xml", "", "");
        }
    }

    public class SomeClass
    {
        // 0. Считываются возможные закоментированные строки, значение будет считано из комментариев, что является ошибочным поведением
        // 0. Функция считывает только первое возможное значение
        // 0. Функция считает значение даже если xml не валиден
        // 0. Функция не обрабатывает невалидные значения входных параметров, такие как пустые строки и null
        // 1. Неплохо бы иметь Summury у функции
        // 2. Func1 ничего не говорит о том что делает эта функция
        // 3. Название входного параметра input не очевидно,
        // хотя должно указывать на то что это путь до файла xml
        // Также можно запутаться с порядком параметроф функции, так как они имеют один и тот же тип string
        // Решение: передача объекта, FluentAPI, но возомжно это излишне
        public static string Func1(string input, string elementName, string attrName)
        {
            // 4. Везде используется var, а тут явное объявление типа, стиль должен быть единым
            // 5. usign System.IO; Если нет конфликтов с наименованием File
            string[] lines = System.IO.File.ReadAllLines(input);
            // Можно назвать attrValue
            string result = null;

            foreach (var line in lines)
            {
                // 6. Опечатка с наименованием startElEndex - Index, сокращение El - лучше писать польностью
                var startElEndex = line.IndexOf(elementName);

                // 7. Так как после if кода нет, лучше для избежания излишней вложенности инвертировать if
                // if (startElEndex == -1) continue;
                if (startElEndex != -1)
                {
                    // 8. То же что и пункт 7.
                    if (line[startElEndex - 1] == '<')
                    {
                        // То же что пунтк 9
                        var endElIndex = line.IndexOf('>', startElEndex - 1);
                        // 10. Сменился стиль наименования, по логике ожидается startAttrIndex
                        var attrStartIndex = line.IndexOf(attrName, startElEndex, endElIndex - startElEndex + 1);

                        // 11. То же что и пункт 7.
                        if (attrStartIndex != -1)
                        {
                            // 12. Тут неплохо бы вставить комментарий смысла этого выражения и числа 2
                            // int вместо var
                            int valueStartIndex = attrStartIndex + attrName.Length + 2;

                            // 13. Почему не использовать IndexOf(line, valueStartIndex + 1)
                            while (line[valueStartIndex] != '"')
                            {
                                // 14. Если следовать пункту 13 лучше использовать Substring
                                // много раз пересоздается строка result
                                result += line[valueStartIndex];
                                valueStartIndex++;
                            }

                            break;
                        }
                    }
                }
            }

            return result;
        }

        // Пример где исправлено только оформление кода

        /// <summary>
        ///  Функция выполняет поиск значения переданного атрибута элемента Xml-документа.
        /// </summary>
        /// <param name="pathToXmlFile">путь до файла</param>
        /// <param name="elementName">название элемента (в котором ищется значение);</param>
        /// <param name="attrName">название атрибута</param>
        /// <returns>значение атрибута</returns>
        public static string GetAttributeValueFromXmlFile(string pathToXmlFile, string elementName, string attrName)
        {
            var lines = File.ReadAllLines(pathToXmlFile);
            string attrValue = null;

            foreach (var line in lines)
            {
                var startElementIndex = line.IndexOf(elementName);

                if (startElementIndex == 1)
                    continue;

                if (line[startElementIndex - 1] != '<')
                    continue;
                
                var endElementIndex = line.IndexOf('>', startElementIndex - 1);
                var startAttrIndex = line.IndexOf(attrName, startElementIndex, endElementIndex - startElementIndex + 1);

                if (startAttrIndex == -1)
                    continue;

                // От начала атрибута пропускаем его длину символ = и символ "
                var valueStartIndex = startAttrIndex + attrName.Length + 2; 

                while (line[valueStartIndex] != '"')
                {
                    attrValue += line[valueStartIndex];
                    valueStartIndex++;
                }

                break;
            }

            return attrValue;
        }

        // Пример альтернативной возможной реализации
        public static string GetAttributeValueFromXmlFile_UseRegex(string pathToXmlFile, string elementName, string attrName)
        {
            var xmlText = File.ReadAllText(pathToXmlFile);

            var regex = new Regex(@"<\s*" + elementName + @"\s[^>]+\" + attrName + @"\s*=\s*""");
            var match = regex.Match(xmlText);

            if (match.Success == false)
                return null;
            
            var closeQuotesIndex = xmlText.IndexOf('"', match.Index + match.Length);

            if (closeQuotesIndex == -1)
                return null;

            var attrValue = xmlText.Substring(match.Index, closeQuotesIndex);

            return attrValue;
        }

        // Лучше всего использовать xml parser
        public static string GetAttributeValueFromXmlFile_UseXmlParser(string pathToXmlFile, string elementName, string attrName)
        {
            var xml = new XmlDocument();
            xml.Load(pathToXmlFile);

            var value = xml.SelectNodes($"//{elementName}/@{attrName}");
            return null;
        }
    }
}
