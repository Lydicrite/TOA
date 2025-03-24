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



        public Automaton(List<string> tokens)
        {
            Tokens = tokens;
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
        public void SetConditionalValue(int xIndex, bool value)
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

            for (int i = 0; i < binaryValues.Length; i++)
            {
                SetConditionalValue(
                    conditionalVertices[i].Index,
                    binaryValues[i] == '1'
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
                        outputs.Add("\n[Обнаружен цикл!]");
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

        /// <summary>
        /// Обнаруживает циклы для конкретной комбинации условий.
        /// </summary>
        /// <param name="binaryValues">Строка, состоящая из n символов '0' и '1', где n равно количеству условных вершин в автомате.</param>
        /// <returns>Словарь с ключом в виде значений условных вершин и значениями в виде описания циклов.</returns>
        public Dictionary<string, List<string>> DetectCyclesForConditions(string binaryValues)
        {
            SetConditionsFromBinary(binaryValues);
            var path = Run(verbose: false).Where(x => x != null).Select(ExtractId).ToList();
            return FindUniqueCycles(path);
        }

        /// <summary>
        /// Находит все возможные циклы для всех комбинаций условий
        /// </summary>
        /// <returns>Словарь, содержащий словарь с ключом в виде значений условных вершин и значениями в виде описания циклов.</returns>
        public Dictionary<string, Dictionary<string, List<string>>> FindAllPossibleCycles()
        {
            var allCycles = new Dictionary<string, Dictionary<string, List<string>>>();
            var conditionals = Elements.OfType<ConditionalVertex>().OrderBy(v => v.Index).ToList();
            int n = conditionals.Count;

            for (int i = 0; i < Math.Pow(2, n); i++)
            {
                string binary = Convert.ToString(i, 2).PadLeft(n, '0');
                var cycles = DetectCyclesForConditions(binary);
                if (cycles.Count > 0)
                    allCycles[binary] = cycles;
            }

            return allCycles;
        }

        private Dictionary<string, List<string>> FindUniqueCycles(List<string> path)
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
                            var normalized = NormalizeCycle(cycle);

                            if (!cycles.ContainsKey(normalized.Key))
                                cycles.Add(normalized.Key, normalized.Value);
                        }
                    }
                }
            }

            return cycles;
        }

        #endregion



        #region Вывод информации

        /// <summary>
        /// Возвращает форматированную строку, содержащую токены автомата.
        /// </summary>
        /// <returns>Форматированная строка, содержащая токены автомата.</returns>
        public string GetTokens()
        {
            return $"Токены ЛСА: [{Tokens.Count}] {{ {string.Join(" ", Tokens)} }}";
        }

        /// <summary>
        /// Получает данные обо всех циклах, найденных при тестировании автомата со всеми возможными вариантами входных данных.
        /// </summary>
        /// <returns>Строка, содержащая отформатированную информацию о циклах.</returns>
        public string GetLoops()
        {
            var allCycles = FindAllPossibleCycles();
            var loopsDescr = $"Найденные циклы: [{allCycles.Count}] \n{{";

            foreach (var condition in allCycles)
            {
                loopsDescr += $"\nУсловия: {condition.Key}";
                foreach (var cycle in condition.Value)
                    loopsDescr += $"\n\tЦикл: {string.Join(" → ", cycle.Value)}";
            }
            loopsDescr += $"\n}}";

            return loopsDescr;
        } 

        /// <summary>
        /// Создаёт и возвращает словарь с ключами в виде входных значений условных вершин и значениями в виде строки из пройденных вершин.
        /// </summary>
        /// <returns>Словарь, созданный по описанным правилам.</returns>
        public Dictionary<string, string> GenerateFinalsTable()
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

        private (string Key, List<string> Value) NormalizeCycle(List<string> cycle)
        {
            var startIndex = cycle.FindIndex(x => x.StartsWith("↓"));
            if (startIndex > 0)
                cycle = cycle.Skip(startIndex).Concat(cycle.Take(startIndex)).ToList();

            return (string.Join("→", cycle), cycle);
        }

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
