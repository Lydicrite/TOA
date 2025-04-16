using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TOAConsole.LSA.Elements.Common;
using TOAConsole.LSA.Elements.Jumps;
using TOAConsole.LSA.Elements.Vertexes;

namespace TOAConsole.LSA.LSAutomaton
{
    /// <summary>
    /// Представляет собой класс автомата, работающего по принципу ЛСА (логической схемы алгоритма).
    /// </summary>
    internal class Automaton
    {
        public string LogicalScheme { get; private set; } = string.Empty;
        /// <summary>
        /// Список токенов автомата.
        /// <br>Заполняется во время парсинга входной строки, содержащей запись ЛСА.</br>
        /// </summary>
        public List<string> Tokens { get; private set; } = new();
        /// <summary>
        /// Словарь, содержащий точки переходов с ключом по их <see cref="JumpPoint.JumpIndex"/>.
        /// </summary>
        public Dictionary<int, ILSAElement> JumpPoints { get; private set; } = new();
        /// <summary>
        /// Список, содержащий все элементы ЛСА.
        /// </summary>
        public List<ILSAElement> Elements { get; private set; } = new();
        /// <summary>
        /// Текущий "активный" элемент автомата.
        /// </summary>
        public ILSAElement? CurrentElement { get; private set; }

        public Automaton(List<string> tokens, string logicalScheme)
        {
            Tokens = tokens;
            LogicalScheme = logicalScheme;
        }





        #region Методы настройки

        /// <summary>
        /// Добавляет новый элемент в список элементов автомата.
        /// </summary>
        /// <param name="element">Элемент для добавления.</param>
        public void AddElement(ILSAElement element)
        {
            Elements.Add(element);
            if (element is JumpPoint jp)
                JumpPoints[jp.JumpIndex] = jp;
        }

        /// <summary>
        /// Устанавливает для условной вершины её <see cref="ConditionalVertex.Value"/>.
        /// </summary>
        /// <param name="xIndex">Индекс условной вершины.</param>
        /// <param name="value">Значение, которое примет <see cref="ConditionalVertex.Value"/></param>
        public void SetConditionalValue(int xIndex, bool? value)
        {
            var cond = Elements.OfType<ConditionalVertex>().FirstOrDefault(x => x.Index == xIndex);
            if (cond != null)
                cond.Value = value;
        }

        /// <summary>
        /// Устанавливает значения условных вершин на основе бинарной строки.
        /// </summary>
        /// <param name="binaryValues">Строка, состоящая из n символов '0' и '1', где n равно количеству условных вершин в автомате.</param>
        public void SetConditionsFromBinary(string binaryValues)
        {
            var conditionalVertices = Elements.OfType<ConditionalVertex>().OrderBy(v => v.Index).ToList();

            if (!Regex.IsMatch(binaryValues, @"^[01]+$"))
                throw new ArgumentException("Строка должна содержать только 0 и 1");

            if (binaryValues.Length != conditionalVertices.Count)
                throw new ArgumentException($"Ожидается {conditionalVertices.Count} символов, получено {binaryValues.Length}");

            for (int i = 0; i < conditionalVertices.Count; i++)
            {
                SetConditionalValue(
                    conditionalVertices[i].Index,
                    binaryValues[i] == '1'
                );
            }
        }

        /// <summary>
        /// Устанавливает значения условных вершин на неопределённые (null).
        /// </summary>
        public void ResetConditions()
        {
            var conditionalVertices = Elements.OfType<ConditionalVertex>().OrderBy(v => v.Index).ToList();

            for (int i = 0; i < conditionalVertices.Count; i++)
            {
                SetConditionalValue(
                    conditionalVertices[i].Index,
                    null
                );
            }
        }

        #endregion





        #region Методы выполнения работы

        /// <summary>
        /// Выполняет алгоритм автомата с предварительной установкой значений условных вершин на основе бинарной строки.
        /// </summary>
        /// <param name="binaryValues">Строка, состоящая из n символов '0' и '1', где n равно количеству условных вершин в автомате.</param>
        /// <param name="verbose">Определяет, нужно ли заполнять возвращаемую коллекцию строк.</param>
        /// <returns>Коллекция строк, описывающая ход работы автомата при заданных условиях.</returns>
        public IEnumerable<string?> Run(string binaryValues, bool verbose = true)
        {
            SetConditionsFromBinary(binaryValues);
            return Run(verbose);
        }

        /// <summary>
        /// Выполняет алгоритм автомата.
        /// </summary>
        /// <param name="verbose">Определяет, нужно ли заполнять возвращаемую коллекцию строк.</param>
        /// <returns>Коллекция строк, описывающая ход работы автомата при заданных условиях.</returns>
        public IEnumerable<string?> Run(bool verbose = true)
        {
            var outputs = new List<string?>();
            CurrentElement = Elements.OfType<StartVertex>().FirstOrDefault();
            var visited = new HashSet<ILSAElement>();

            while (CurrentElement != null && CurrentElement is not EndVertex)
            {
                if (verbose)
                    outputs.Add(CurrentElement.GetLongDescription());
                else
                    outputs.Add(CurrentElement.Id);

                if (visited.Contains(CurrentElement))
                {
                    if (verbose)
                        Console.Write("\n► Обнаружен цикл! ◄");
                    break;
                }

                visited.Add(CurrentElement);
                CurrentElement = CurrentElement.GetNext(this);
            }

            if (CurrentElement is EndVertex)
            {
                if (verbose)
                    outputs.Add(CurrentElement.GetLongDescription());
                else
                    outputs.Add(CurrentElement.Id);
            }

            return outputs;
        }



        #region Обнаружение циклов

        /// <summary>
        /// Обнаруживает циклы для конкретной комбинации условий.
        /// </summary>
        /// <param name="binaryValues">Строка, состоящая из n символов '0' и '1', где n равно количеству условных вершин в автомате.</param>
        /// <returns>Словарь с ключом в виде значений условных вершин и значениями в виде описания циклов.</returns>
        public Dictionary<string, List<string>> DetectLoopsForConditions(string binaryValues)
        {
            SetConditionsFromBinary(binaryValues);
            var path = Run(verbose: false).Where(x => x != null).Select(ExtractId).ToList();
            return FindUniqueLoops(path);
        }

        /// <summary>
        /// Находит все возможные циклы для всех комбинаций условий
        /// </summary>
        /// <returns>Словарь, содержащий словарь с ключом в виде значений условных вершин и значениями в виде описания циклов.</returns>
        public Dictionary<string, Dictionary<string, List<string>>> FindAllPossibleLoops()
        {
            var allCycles = new Dictionary<string, Dictionary<string, List<string>>>();
            var conditionals = Elements.OfType<ConditionalVertex>().OrderBy(v => v.Index).ToList();
            int n = conditionals.Count;

            for (int i = 0; i < Math.Pow(2, n); i++)
            {
                string binary = Convert.ToString(i, 2).PadLeft(n, '0');
                var cycles = DetectLoopsForConditions(binary);
                if (cycles.Count > 0)
                    allCycles[binary] = cycles;
            }

            return allCycles;
        }

        private Dictionary<string, List<string>> FindUniqueLoops(List<string> path)
        {
            var cycles = new Dictionary<string, List<string>>();
            var visited = new HashSet<string>();

            if (path != null)
            {
                for (int i = 0; i < path.Count; i++)
                {
                    for (int j = i + 1; j < path.Count; j++)
                    {
                        if (path[i] == path[j])
                        {
                            var cycle = path.Skip(i).Take(j - i).ToList();
                            var normalized = NormalizeLoop(cycle);

                            if (!cycles.ContainsKey(normalized.Key))
                                cycles.Add(normalized.Key, normalized.Value);
                        }
                    }
                }
            }

            return cycles;
        }

        private (string Key, List<string> Value) NormalizeLoop(List<string> loop)
        {
            var startIndex = loop.FindIndex(x => x.StartsWith("↓"));
            if (startIndex > 0)
                loop = loop.Skip(startIndex).Concat(loop.Take(startIndex)).ToList();

            return (string.Join("→", loop), loop);
        }

        #endregion



        #region Работа с автоматом в различных режимах

        /// <summary>
        /// Запускает первый режим взаимодействия пользователя с автоматом.
        /// <br>От пользователя требуется последовательный ввод значений логических условий Х, 
        /// а автомат выводит подробный результат каждого шага моделирования работы.</br>
        /// </summary>
        public void RunInteractive()
        {
            ResetConditions();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║                Режим работы M1                 ║");
            Console.WriteLine("╠════════════════════════════════════════════════╣");
            Console.WriteLine("║ Вам необходимо вводить логическое значение для ║");
            Console.WriteLine("║ каждой встреченной в ходе работы алгоритма     ║");
            Console.WriteLine("║ условной вершины.                              ║");
            Console.WriteLine("║                                                ║");
            Console.WriteLine("║ Вы получите подробную информацию о каждом      ║");
            Console.WriteLine("║ пройденном элементе алгоритма, пока алгоритм   ║");
            Console.WriteLine("║ не войдёт в цикл или пока алгоритм не дойдёт   ║");
            Console.WriteLine("║ до конечной операторной вершины Yк.            ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            Console.Write($"\n►► Начата работа с алгоритмом \"{LogicalScheme}\" в режиме M1 ◄◄\n");
            Console.ResetColor();

            CurrentElement = Elements.OfType<StartVertex>().FirstOrDefault();
            var visited = new HashSet<ILSAElement>();
            var inputs = string.Empty;

            while (CurrentElement != null && CurrentElement is not EndVertex)
            {
                if (CurrentElement is ConditionalVertex cv && !cv.Value.HasValue)
                {
                    Console.Write(CurrentElement.GetLongDescription() + "\n");
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"\n►► Введите логическое значение для \"{cv.Id}\" ('0' или '1'): ");
                    Console.ResetColor();
                    var input = Console.ReadLine()?.Trim();
                    while (input != "0" && input != "1")
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("►► Некорректный ввод. Введите '0' или '1': ");
                        Console.ResetColor();
                        input = Console.ReadLine()?.Trim();
                    }
                    inputs += input;
                    cv.Value = input == "1";
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write($"\n►► Для веришны \"{cv.Id}\" установлено значение '{input}' ◄◄\n");
                    Console.ResetColor();
                }
                else
                {
                    Console.Write(CurrentElement.GetLongDescription() + "\n");
                }

                if (visited.Contains(CurrentElement))
                {
                    Console.Write("\n► Обнаружен цикл! ◄");
                    Console.Write($"{GetLoopsForConditions(inputs)}");
                    break;
                }
                visited.Add(CurrentElement);
                CurrentElement = CurrentElement.GetNext(this);
            }

            if (CurrentElement is EndVertex)
                Console.Write(CurrentElement.GetLongDescription() + "\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\n►► Работа с алгоритмом \"{LogicalScheme}\" в режиме M1 завершена ◄◄\n\n\n\n");
            Console.ResetColor();
        }

        /// <summary>
        /// Запускает второй режим взаимодействия пользователя с автоматом.
        /// <br>От пользователя требуется ввести в строку значения логических условий всех Х, 
        /// а автомат выводит подробный результат моделирования работы.</br>
        /// </summary>
        public void RunOnetime()
        {
            ResetConditions();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║                Режим работы M2                 ║");
            Console.WriteLine("╠════════════════════════════════════════════════╣");
            Console.WriteLine("║ Вам необходимо ввести в одну строку все        ║");
            Console.WriteLine("║ логические значения для условных вершин.       ║");
            Console.WriteLine("║                                                ║");
            Console.WriteLine("║ Вы получите подробную информацию о каждом      ║");
            Console.WriteLine("║ пройденном элементе алгоритма, пока алгоритм   ║");
            Console.WriteLine("║ не войдёт в цикл или пока алгоритм не дойдёт   ║");
            Console.WriteLine("║ до конечной операторной вершины Yк.            ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            Console.Write($"\n►► Начата работа с алгоритмом \"{LogicalScheme}\" в режиме M2 ◄◄\n");
            Console.ResetColor();

            CurrentElement = Elements.OfType<StartVertex>().FirstOrDefault();
            var conditionalVertices = Elements.OfType<ConditionalVertex>().OrderBy(v => v.Index).ToList();
            int expectedLength = conditionalVertices.Count;
            var visited = new HashSet<ILSAElement>();
            var inputs = string.Empty;

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\n►► Введите логические значения (строку из {expectedLength} '0' или '1' без пробелов): ");
            Console.ResetColor();
            inputs = Console.ReadLine()?.Trim();
            while (inputs == string.Empty || !Regex.IsMatch(inputs, $"^[01]{{{expectedLength}}}$"))
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"►► Некорректный ввод. Введите строку из {{expectedLength}} '0' или '1' без пробелов: ");
                Console.ResetColor();
                inputs = Console.ReadLine()?.Trim();
            }
            SetConditionsFromBinary(inputs);
            foreach (var cv in conditionalVertices)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write($"\n►► Для веришны \"{cv.Id}\" установлено значение '{cv.Value.Value}' ◄◄\n");
                Console.ResetColor();
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\n►► Результаты работы алгоритма ◄◄\n");
            Console.ResetColor();
            while (CurrentElement != null && CurrentElement is not EndVertex)
            {
                Console.Write(CurrentElement.GetLongDescription() + "\n");

                if (visited.Contains(CurrentElement))
                {
                    Console.Write("\n► Обнаружен цикл! ◄");
                    Console.Write($"{GetLoopsForConditions(inputs)}");
                    break;
                }
                visited.Add(CurrentElement);
                CurrentElement = CurrentElement.GetNext(this);
            }

            if (CurrentElement is EndVertex)
                Console.Write(CurrentElement.GetLongDescription() + "\n");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\n►► Работа с алгоритмом \"{LogicalScheme}\" в режиме M2 завершена ◄◄\n\n\n\n");
            Console.ResetColor();
        }

        public void RunToGetAllResults()
        {
            ResetConditions();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("╔════════════════════════════════════════════════╗");
            Console.WriteLine("║                Режим работы M3                 ║");
            Console.WriteLine("╠════════════════════════════════════════════════╣");
            Console.WriteLine("║ Вы получите краткую информацию о ходе работы   ║");
            Console.WriteLine("║ алгоритма при каждой возможной комбинации      ║");
            Console.WriteLine("║ логических условий, а также информацию об      ║");
            Console.WriteLine("║ обнаруженных при этих условиях циклах.         ║");
            Console.WriteLine("╚════════════════════════════════════════════════╝");
            Console.Write($"\n►► Начата работа с алгоритмом \"{LogicalScheme}\" в режиме M3 ◄◄\n");
            Console.ResetColor();
            Console.WriteLine($"{GetResults()}");
            Console.WriteLine($"{GetAllLoops()}");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\n►► Работа с алгоритмом \"{LogicalScheme}\" в режиме M3 завершена ◄◄\n\n\n\n");
            Console.ResetColor();
        }

        #endregion

        #endregion





        #region Вывод информации

        /// <summary>
        /// Возвращает форматированную строку, содержащую токены автомата.
        /// </summary>
        /// <returns>Форматированная строка, содержащая токены автомата.</returns>
        public string GetTokens()
        {
            return $"\n► Токены ЛСА: [{Tokens.Count}] {{ {string.Join(" ", Tokens)} }} ◄\n";
        }

        public string GetLoopsForConditions(string binaryValues)
        {
            var allLoops = DetectLoopsForConditions(binaryValues);
            var loopsDescr = $"\n► Найденные циклы для условий {binaryValues}: \n{{";

            foreach (var condition in allLoops)
            {
                loopsDescr += $"\n\t∞: {string.Join(" → ", condition.Value)}";
            }
            loopsDescr += $"\n}}\n◄\n";

            return loopsDescr;
        }

        /// <summary>
        /// Получает данные обо всех циклах, найденных при тестировании автомата со всеми возможными вариантами входных данных.
        /// </summary>
        /// <returns>Строка, содержащая отформатированную информацию о циклах.</returns>
        public string GetAllLoops()
        {
            var allLoops = FindAllPossibleLoops();
            var loopsDescr = $"\n► Найденные циклы: [{allLoops.Count}] \n{{";

            foreach (var condition in allLoops)
            {
                loopsDescr += $"\n\tУсловия: \"{condition.Key}\", ";
                foreach (var loop in condition.Value)
                    loopsDescr += $"\n\t\t∞: {string.Join(" → ", loop.Value)}";
            }
            loopsDescr += $"\n}}\n◄\n";

            return loopsDescr;
        }

        /// <summary>
        /// Получает краткие результаты работы алгоритма для всех возможных комбинаций значений условных вершин.
        /// </summary>
        /// <returns>Строка, содержащая отформатированную информацию о результатах работы алгоритма для всех возможных комбинаций значений условных вершин.</returns>
        public string GetResults()
        {
            var allResults = GenerateResultsDictionary();
            var resultsDescr = $"\n► Результаты работы алгоритма для всех возможных комбинаций значений условных вершин: [{allResults.Count()}] \n{{";

            foreach (var result in allResults)
            {
                resultsDescr += $"\n\tУсловия: \"{result.Key}\", \n\t\tход работы алгоритма: {result.Value}";
            }
            resultsDescr += $"\n}}\n◄\n";

            return resultsDescr;
        }

        /// <summary>
        /// Создаёт и возвращает словарь с ключами в виде входных значений условных вершин и значениями в виде строки из пройденных вершин.
        /// </summary>
        /// <returns>Словарь, созданный по описанным правилам.</returns>
        public Dictionary<string, string> GenerateResultsDictionary()
        {
            var conditionals = Elements.OfType<ConditionalVertex>().OrderBy(v => v.Index).ToList();
            int n = conditionals.Count;
            var table = new Dictionary<string, string>();

            for (int i = 0; i < Math.Pow(2, n); i++)
            {
                string binary = Convert.ToString(i, 2).PadLeft(n, '0');
                SetConditionsFromBinary(binary);
                var runOut = Run(verbose: false);
                var path = runOut.Select(s => ExtractId(s)).ToList();
                table[binary] = string.Join(" → ", path);
            }

            return table;
        }

        public void PrintAlgorithmInfo()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"\n\n\n►► Дополнительная информация об алгоритме \"{LogicalScheme}\" ◄◄\n");
            Console.ResetColor();

            Console.WriteLine($"{GetTokens()}");
            Console.WriteLine($"{GetAllLoops()}");
            Console.WriteLine($"{GetResults()}");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"►► Вывод дополнительной информации об алгоритме \"{LogicalScheme}\" завершён ◄◄\n");
            Console.ResetColor();
        }

        #endregion





        #region Дополнительные методы

        private string? ExtractId(string? description)
        {
            if (description == null) 
                return "";
            if (description.Contains("Yн")) 
                return "Yн";
            if (description.Contains("Yк")) 
                return "Yк";
            return description.Split('\"').Length > 1 ? description.Split('\"')[1] : description;
        }

        #endregion
    }
}
