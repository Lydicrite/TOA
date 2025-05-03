using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TOAConsole.LogicalAA.Elements.Vertexes;
using TOAConsole.LogicalExpressionParser;

namespace TOAConsole.LogicalAA.Automaton.Utils.MAS
{
    /// <summary>
    /// Представляет матричную схему алгоритма.
    /// </summary>
    internal sealed class MatrixSchema
    {
        /// <summary>
        /// Список заголовков столбцов МСА.
        /// </summary>
        public List<string> Headers { get; set; } = new();
        /// <summary>
        /// Список строк МСА.
        /// </summary>
        public List<MASRow> Rows { get; set; } = new();



        public override string ToString()
        {
            var sb = new StringBuilder();
            var columnWidths = CalculateColumnWidths();

            // Верхняя граница таблицы
            sb.AppendLine(BuildTopBorder(columnWidths));

            // Заголовки столбцов
            sb.AppendLine(BuildHeaderLine(columnWidths));
            sb.AppendLine(BuildMiddleBorder(columnWidths));

            // Строки данных
            for (int i = 0; i < Headers.Count; i++)
            {
                sb.AppendLine(BuildDataLine(Headers[i], Rows[i].Transitions, columnWidths));
                if (i < Headers.Count - 1)
                    sb.AppendLine(BuildMiddleBorder(columnWidths));
            }

            // Нижняя граница таблицы
            sb.AppendLine(BuildBottomBorder(columnWidths));

            return sb.ToString();
        }

        /// <summary>
        /// Форматирует строку с заголовками столбцов МСА.
        /// </summary>
        /// <param name="widths">Ширины столбцов.</param>
        /// <returns>Отформатированная строка заголовков.</returns>
        private string BuildHeaderLine(List<int> widths)
        {
            var sb = new StringBuilder();
            sb.Append('│');
            sb.Append(" ".PadRight(widths[0]));
            sb.Append('│');

            for (int i = 0; i < Headers.Count; i++)
            {
                sb.Append(Headers[i].Center(widths[i + 1]));
                sb.Append('│');
            }

            return sb.ToString();
        }

        /// <summary>
        /// Форматирует строку с данными матрицы для указанной строки.
        /// </summary>
        /// <param name="rowHeader">Заголовок строки.</param>
        /// <param name="transitions">Данные переходов.</param>
        /// <param name="widths">Ширины столбцов.</param>
        /// <returns>Отформатированная строка данных.</returns>
        private static string BuildDataLine(string rowHeader, List<string> transitions, List<int> widths)
        {
            var sb = new StringBuilder();
            sb.Append('│');
            sb.Append(rowHeader.PadRight(widths[0]));
            sb.Append('│');

            for (int i = 0; i < transitions.Count; i++)
            {
                sb.Append(ProcessConditions(transitions[i]).Center(widths[i + 1]));
                sb.Append('│');
            }

            return sb.ToString();
        }

        /// <summary>
        /// Вычисляет оптимальные ширины столбцов для форматирования.
        /// </summary>
        /// <returns>Список ширин столбцов.</returns>
        private List<int> CalculateColumnWidths()
        {
            var widths = new List<int>();

            // Ширина первого столбца (заголовки строк)
            int firstColumnWidth = Headers.Max(h => h.Length) + 2;
            widths.Add(firstColumnWidth);

            // Ширина остальных столбцов (должно совпадать с количеством переходов)
            int columnsCount = Headers.Count;

            for (int i = 0; i < columnsCount; i++)
            {
                int maxWidth = Headers[i].Length + 2;

                if (Rows.Count > 0)
                {
                    int maxDataWidth = Rows.Max(r =>
                        ProcessConditions(r.Transitions[i]).Length);
                    maxWidth = Math.Max(maxWidth, maxDataWidth + 2);
                }

                widths.Add(maxWidth);
            }

            return widths;
        }

        /// <summary>
        /// Обрабатывает логические условия для корректного отображения в МСА.
        /// </summary>
        /// <param name="input">Исходная строка условий.</param>
        /// <returns>Отформатированная строка с добавлением скобок.</returns>
        private static string ProcessConditions(string input)
        {
            if (string.IsNullOrEmpty(input) || input == "0" || input == "1")
                return input;

            // Разделяем на части по ИЛИ
            var orParts = input.Split(new[] { " ˅ " }, StringSplitOptions.RemoveEmptyEntries);

            // Обрабатываем только если больше одной части
            if (orParts.Length > 1)
            {
                return string.Join(" ˅ ", orParts.Select
                    (
                        part =>
                        {
                            // Добавляем скобки если есть И
                            if (part.Contains(" ˄ ") && !IsWrappedInParentheses(part))
                                return $"({part})";
                            return part;
                        }
                    )
                );
            }

            return input;
        }



        #region Упрощение и минимизация

        /// <summary>
        /// Упрощает логические выражения в ячейках МСА.
        /// </summary>
        /// <returns>Новая МСА, содержащая упрощённые логические выражения в своих ячейках.</returns>
        /// <exception cref="AggregateException"></exception>
        public MatrixSchema Simplify()
        {
            var simplifiedMatrix = this.DeepCopy();

            var exceptions = new List<Exception>();

            for (int rowIdx = 0; rowIdx < simplifiedMatrix.Rows.Count; rowIdx++)
            {
                var row = simplifiedMatrix.Rows[rowIdx];

                for (int colIdx = 0; colIdx < row.Transitions.Count; colIdx++)
                {
                    string originalCondition = row.Transitions[colIdx];

                    if (string.IsNullOrWhiteSpace(originalCondition))
                        continue;

                    try
                    {
                        // Парсинг и упрощение
                        var parsed = LEParser.Parse(originalCondition);
                        var originalExpr = new LogicalExpression(parsed);

                        // Установка порядка переменных
                        var variables = originalExpr.TruthTable.First()
                            .Take(originalExpr.TruthTable[0].Length - 1)
                            .Select((_, i) => originalExpr.Variables[i])
                            .ToArray();
                        originalExpr.SetVariableOrder(variables);

                        var simplifiedExpr = originalExpr.Expand();

                        // Проверка эквивалентности
                        if (!originalExpr.Equals(simplifiedExpr))
                            throw new InvalidOperationException
                            (
                                $"Упрощение нарушило эквивалентность: {originalCondition} -> {simplifiedExpr}"
                            );

                        // Форматирование ячейки
                        var cellString = simplifiedExpr
                            .ToString()
                            .Replace('~', '¬')
                            .Replace('&', '˄')
                            .Replace('|', '˅');


                        // Удаление внешних скобок если они есть
                        if (cellString.StartsWith("(") && cellString.EndsWith(")"))
                            cellString = cellString.Substring(1, cellString.Length - 2);

                        // Разбиение на конъюнкции и нормализация
                        var parts = cellString
                            .Split(new[] { '˅' }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(p => p.Trim())
                            .ToList();

                        var cleanedParts = parts
                            .Select(p => p
                                .Replace("(", "")
                                .Replace(")", "")
                                .Trim()
                            )
                            .Where(p => !string.IsNullOrEmpty(p))
                            .ToList();

                        // Сборка финальной строки с обёрткой в скобки
                        cellString = cleanedParts.Any()
                            ? string.Join(" ˅ ", cleanedParts.Select(p => cleanedParts.Count() > 1 ? $"({p})" : p))
                            : " ";

                        row.Transitions[colIdx] = cellString;

                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(new Exception(
                            $"\nОшибка в ячейке [{Headers[rowIdx]} -> {Headers[colIdx]}]: {ex.Message}",
                            ex
                        ));
                    }
                }
            }

            if (exceptions.Any())
                throw new AggregateException(
                    "Ошибки при упрощении логических выражений",
                    exceptions
                );

            return simplifiedMatrix;
        }

        /// <summary>
        /// Минимизирует МСА с помощью распределения сдвигов.
        /// </summary>
        /// <returns>Новая минимизированная МСА с упрощёнными логическими выражениями.</returns>
        public MatrixSchema Minimize()
        {
            // Шаг 1: Первоначальное упрощение
            var minimized = this.Simplify();

            // Шаг 2: Распределение сдвигов для P-переменных
            minimized = ApplyShiftDistribution(minimized);

            // Шаг 3: Финальное упрощение
            return minimized.Simplify();
        }

        /// <summary>
        /// Применяет распределение сдвигов для минимизации МСА.
        /// </summary>
        /// <param name="schema">МСА, в которой небходимо выполнить распределение сдвигов.</param>
        /// <returns>Новая МСА, подвергнутая распределению сдвигов.</returns>
        private static MatrixSchema ApplyShiftDistribution(MatrixSchema schema)
        {
            Regex regex = new Regex(@"(?<!\S)¬?P\d+\b");
            var pVariables = schema.DetectPVariables();
            var newSchema = schema.DeepCopy();

            foreach (var pVar in pVariables)
            {
                for (int colIdx = 0; colIdx < newSchema.Headers.Count; colIdx++)
                {
                    // Анализ столбца на постоянство значения P-переменной
                    var column = newSchema.GetColumn(colIdx);
                    var columnValues = new HashSet<string>();
                    foreach (var cell in column)
                    {
                        if (cell.Replace(" ", "") == string.Empty)
                            continue;

                        var matched = regex.Matches(cell).Select(m => m.Value).ToList();
                        foreach (var m in matched)
                            if (m.Contains(pVar) || m == pVar)
                                columnValues.Add(m);
                    }

                    if (columnValues.Any())
                    {
                        string constValue = columnValues.First();
                        if (columnValues.All(v => v == constValue))
                        {
                            // Подстановка константы в строки
                            string replacement = constValue.Contains('¬') ? "(0)" : "(1)";
                            for (int cIdx = 0; cIdx < newSchema.Rows.Count; cIdx++)
                            {
                                newSchema.Rows[colIdx].Transitions[cIdx] =
                                    newSchema.Rows[colIdx].Transitions[cIdx]
                                        .Replace(pVar, replacement);
                            }
                        }
                    }
                }
            }

            var msastr = newSchema.ToString();

            return newSchema;
        }

        #endregion



        #region Формирование границ таблицы

        /// <summary>
        /// Проверяет, обёрнуто ли выражение в скобки.
        /// </summary>
        /// <param name="value">Проверяемое выражение.</param>
        /// <returns><see langword="true"/>, если <paramref name="value"/> обёрнуто в скобки, иначе <see langword="false"/>.</returns>
        private static bool IsWrappedInParentheses(string value)
        {
            return value.StartsWith("(") && value.EndsWith(")");
        }

        /// <summary>
        /// Строит верхнюю границу таблицы.
        /// </summary>
        /// <param name="widths">Список из ширин столбцов.</param>
        /// <returns>Строка, представляющая верхнюю границу таблицы МСА.</returns>
        private static string BuildTopBorder(List<int> widths)
        {
            return "┌" + string.Join("┬", widths.Select(w => new string('─', w))) + "┐";
        }

        /// <summary>
        /// Строит границу между строками таблицы.
        /// </summary>
        /// <param name="widths">Список из ширин столбцов.</param>
        /// <returns>Строка, представляющая границу между строками таблицы МСА.</returns>
        private static string BuildMiddleBorder(List<int> widths)
        {
            return "├" + string.Join("┼", widths.Select(w => new string('─', w))) + "┤";
        }

        /// <summary>
        /// Строит нижнюю границу таблицы.
        /// </summary>
        /// <param name="widths">Список из ширин столбцов.</param>
        /// <returns>Строка, представляющая нижнюю границу таблицы МСА.</returns>
        private static string BuildBottomBorder(List<int> widths)
        {
            return "└" + string.Join("┴", widths.Select(w => new string('─', w))) + "┘";
        }

        #endregion



        #region Доступ к элементам

        /// <summary>
        /// Индексатор для доступа к строкам матрицы (MASRow).
        /// </summary>
        /// <param name="rowIndex">Индекс искомого элемента.</param>
        /// <returns>Объект <see cref="MASRow"/>, расположенный по переданному индексу.</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public MASRow this[int rowIndex]
        {
            get
            {
                if (rowIndex < 0 || rowIndex >= Rows.Count)
                    throw new IndexOutOfRangeException("Индекс строки за пределами диапазона");
                return Rows[rowIndex];
            }
        }

        /// <summary>
        /// Индексатор для доступа к элементам матрицы (ячейкам).
        /// </summary>
        /// <param name="rowIndex">Индекс строки искомого элемента.</param>
        /// <param name="columnIndex">Индекс столбца искомого элемента.</param>
        /// <returns>Искомый элемент.</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public string this[int rowIndex, int columnIndex]
        {
            get
            {
                if (rowIndex < 0 || rowIndex >= Rows.Count)
                    throw new IndexOutOfRangeException("Индекс строки за пределами диапазона");
                if (columnIndex < 0 || columnIndex >= Headers.Count)
                    throw new IndexOutOfRangeException("Индекс столбца за пределами диапазона");
                return Rows[rowIndex].Transitions[columnIndex];
            }
        }

        /// <summary>
        /// Получает список содержимого столбца.
        /// </summary>
        /// <param name="columnIndex">Индекс столбца.</param>
        /// <returns>Элемент МСА, расположенный по переданным индексам.</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public List<string> GetColumn(int columnIndex)
        {
            if (columnIndex < 0 || columnIndex >= Headers.Count)
                throw new ArgumentOutOfRangeException(
                    nameof(columnIndex),
                    $"Индекс столбца {columnIndex} выходит за пределы диапазона [0, {Headers.Count - 1}]"
                );

            var column = new List<string>();
            foreach (var row in Rows)
            {
                if (columnIndex >= row.Transitions.Count)
                    throw new InvalidOperationException(
                        $"Строка не содержит столбец с индексом {columnIndex}"
                    );

                column.Add(row.Transitions[columnIndex]);
            }

            return column;
        }

        #endregion
    }



    #region Дополнительные классы для работы со строками

    /// <summary>
    /// Методы расширения для работы со строками.
    /// </summary>
    internal static class Extensions
    {
        /// <summary>
        /// Метод расширения для центрирования текста в ячейке.
        /// </summary>
        /// <param name="text">Исходный текст.</param>
        /// <param name="totalWidth">Общая ширина ячейки.</param>
        /// <returns>Отцентрированная строка.</returns>
        public static string Center(this string text, int totalWidth)
        {
            if (text.Length >= totalWidth) return text;

            int padding = totalWidth - text.Length;
            int leftPadding = padding / 2;
            int rightPadding = padding - leftPadding;

            return new string(' ', leftPadding) + text + new string(' ', rightPadding);
        }

        /// <summary>
        /// Возвращает глубокую копию для данной МСА.
        /// </summary>
        /// <param name="schema">Исходная МСА.</param>
        /// <returns>Копия исходной МСА.</returns>
        public static MatrixSchema DeepCopy(this MatrixSchema schema)
        {
            return new MatrixSchema
            {
                Headers = new List<string>(schema.Headers),
                Rows = schema.Rows.Select(r => new MASRow
                {
                    Transitions = new List<string>(r.Transitions)
                }).ToList()
            };
        }

        /// <summary>
        /// Обнаруживает все условные вершины, чьё <see cref="ConditionalVertex.ID"/> начинается с буквы 'P'.
        /// </summary>
        /// <param name="schema">Исходная МСА.</param>
        /// <returns>Список из <see cref="ConditionalVertex.ID"/> искомых условных вершин.</returns>
        public static List<string> DetectPVariables(this MatrixSchema schema)
        {
            var variables = new HashSet<string>();
            var pattern = @"\bP\d+\b";

            foreach (var row in schema.Rows)
            {
                foreach (var cell in row.Transitions)
                {
                    var matches = Regex.Matches(cell, pattern);
                    foreach (Match m in matches)
                        variables.Add(m.Value);
                }
            }

            return variables.ToList();
        }
    }

    /// <summary>
    /// Представляет строку матричной схемы алгоритма.
    /// </summary>
    internal class MASRow
    {
        /// <summary>
        /// Список условий переходов для строки.
        /// </summary>
        public List<string> Transitions { get; set; } = new();

        /// <summary>
        /// Индексатор для доступа к элементам строки МСА.
        /// </summary>
        /// <param name="rowIndex">Индекс искомого элемента.</param>
        /// <returns>Элемент, расположенный по переданному индексу.</returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public string this[int rowIndex]
        {
            get
            {
                if (rowIndex < 0 || rowIndex >= Transitions.Count)
                    throw new IndexOutOfRangeException("Индекс строки за пределами диапазона");
                return Transitions[rowIndex];
            }
        }
    }

    #endregion
}
