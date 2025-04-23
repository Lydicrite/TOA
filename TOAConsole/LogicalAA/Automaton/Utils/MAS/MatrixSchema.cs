using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalAA.Automaton.Utils.MAS
{
    /// <summary>
    /// Представляет матричную схему алгоритма.
    /// </summary>
    internal class MatrixSchema
    {
        /// <summary>
        /// Список заголовков столбцов таблицы.
        /// </summary>
        public List<string> Headers { get; set; } = new();
        /// <summary>
        /// Список строк таблицы.
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
            for (int i = 0; i < Headers.Count - 1; i++)
            {
                sb.AppendLine(BuildDataLine(Headers[i], Rows[i].Transitions, columnWidths));
                if (i < Headers.Count - 2)
                    sb.AppendLine(BuildMiddleBorder(columnWidths));
            }

            // Нижняя граница таблицы
            sb.AppendLine(BuildBottomBorder(columnWidths));

            return sb.ToString();
        }

        /// <summary>
        /// Форматирует строку с заголовками столбцов матрицы.
        /// </summary>
        /// <param name="widths">Ширины столбцов.</param>
        /// <returns>Отформатированная строка заголовков.</returns>
        private string BuildHeaderLine(List<int> widths)
        {
            var sb = new StringBuilder();
            sb.Append("│");
            sb.Append(" ".PadRight(widths[0]));
            sb.Append("│");

            for (int i = 0; i < Headers.Count; i++)
            {
                sb.Append(Headers[i].Center(widths[i + 1]));
                sb.Append("│");
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
        private string BuildDataLine(string rowHeader, List<string> transitions, List<int> widths)
        {
            var sb = new StringBuilder();
            sb.Append("│");
            sb.Append(rowHeader.PadRight(widths[0]));
            sb.Append("│");

            for (int i = 0; i < transitions.Count; i++)
            {
                sb.Append(ProcessConditions(transitions[i]).Center(widths[i + 1]));
                sb.Append("│");
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
        /// Обрабатывает логические условия для корректного отображения в матрице.
        /// </summary>
        /// <param name="input">Исходная строка условий.</param>
        /// <returns>Отформатированная строка с добавлением скобок.</returns>
        private string ProcessConditions(string input)
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



        #region Формирование границ таблицы

        private static bool IsWrappedInParentheses(string value)
        {
            return value.StartsWith("(") && value.EndsWith(")");
        }

        private static string BuildTopBorder(List<int> widths)
        {
            return "┌" + string.Join("┬", widths.Select(w => new string('─', w))) + "┐";
        }

        private static string BuildMiddleBorder(List<int> widths)
        {
            return "├" + string.Join("┼", widths.Select(w => new string('─', w))) + "┤";
        }

        private static string BuildBottomBorder(List<int> widths)
        {
            return "└" + string.Join("┴", widths.Select(w => new string('─', w))) + "┘";
        }

        #endregion
    }



    #region Дополнительные классы для работы со строками

    /// <summary>
    /// Методы расширения для работы со строками.
    /// </summary>
    internal static class StringExtensions
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
    }

    #endregion
}
