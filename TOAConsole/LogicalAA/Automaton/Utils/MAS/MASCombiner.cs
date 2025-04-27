using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalAA.Automaton.Utils.MAS
{
    internal class MASCombiner
    {
        #region Объединение матриц

        /// <summary>
        /// Объединяет модифицированные МСА в единую матрицу.
        /// </summary>
        public static MatrixSchema CombineSchemas(List<MatrixSchema> modifiedSchemes)
        {
            // Собираем все уникальные вершины из всех МСА
            var allVertices = modifiedSchemes
                .SelectMany(s => s.Headers)
                .Distinct()
                .ToList();

            allVertices = OrderVertices(allVertices);

            // Создаем новую матрицу
            var combinedSchema = new MatrixSchema
            {
                Headers = allVertices,
                Rows = new List<MASRow>()
            };

            // Заполняем строки объединенной матрицы
            foreach (var rowVertex in allVertices)
            {
                var newRow = new MASRow();
                foreach (var colVertex in allVertices)
                {
                    var conditions = CollectConditions(modifiedSchemes, rowVertex, colVertex);
                    newRow.Transitions.Add(MergeConditions(conditions));
                }
                combinedSchema.Rows.Add(newRow);
            }

            return combinedSchema;
        }

        private static List<string> OrderVertices(IEnumerable<string> vertices)
        {
            var sorted = new List<string>();

            if (vertices.Contains("Yн"))
                sorted.Add("Yн");

            var otherVertices = vertices
                .Where(v => v != "Yн" && v != "Yк")
                .OrderBy(v => int.Parse(v.Substring(1)))
                .ToList();

            sorted.AddRange(otherVertices);

            if (vertices.Contains("Yк"))
                sorted.Add("Yк");

            return sorted;
        }

        /// <summary>
        /// Собирает условия из всех МСА для ячейки (rowVertex, colVertex).
        /// </summary>
        private static List<string> CollectConditions(List<MatrixSchema> schemes, string rowVertex, string colVertex)
        {
            var conditions = new List<string>();
            foreach (var schema in schemes)
            {
                int rowIndex = schema.Headers.IndexOf(rowVertex);
                int colIndex = schema.Headers.IndexOf(colVertex);

                if (rowIndex != -1 && colIndex != -1)
                {
                    string condition = schema.Rows[rowIndex].Transitions[colIndex];
                    conditions.Add(condition.Trim());
                }
                else
                {
                    conditions.Add(" ");
                }
            }

            return conditions;
        }

        /// <summary>
        /// Объединяет условия по правилам 4.2.1–4.2.3.
        /// </summary>
        private static string MergeConditions(List<string> conditions)
        {
            var nonEmpty = conditions
                .Where(c => !string.IsNullOrWhiteSpace(c) && c != " ")
                .ToList();

            if (nonEmpty.Count == 0)
                return " ";

            if (nonEmpty.Count == 1)
                return nonEmpty[0];

            bool allSame = nonEmpty.All(c => c == nonEmpty[0]);
            if (allSame)
                return nonEmpty[0];

            // Объединяем через дизъюнкцию
            return $"({string.Join(") ˅ (", nonEmpty)})";
        }

        /// <summary>
        /// Проверяет эквивалентность условий (временная реализация).
        /// </summary>
        private static bool ConditionsAreEqual(string a, string b)
        {
            return a.Equals(b, StringComparison.Ordinal);
        }

        #endregion





        #region Подготовка к объединению

        /// <summary>
        /// Объединяет список МСА в набор модифицированных МСА с добавлением переменных P1...Pn.
        /// </summary>
        /// <param name="schemes"></param>
        /// <returns></returns>
        public static List<MatrixSchema> PrepareForCombine(List<MatrixSchema> schemes)
        {
            // Определяем количество дополнительных переменных P
            int n = (int)Math.Ceiling(Math.Log(schemes.Count, 2));
            var mergedSchemas = new List<MatrixSchema>();

            // Генерируем двоичные коды для каждой МСА
            var codes = GenerateBinaryCodes(schemes.Count, n);

            // Модифицируем каждую МСА
            for (int i = 0; i < schemes.Count; i++)
            {
                MatrixSchema modifiedSchema = ModifySchema(schemes[i], codes[i]);
                mergedSchemas.Add(modifiedSchema);
            }

            return mergedSchemas;
        }

        /// <summary>
        /// Генерирует двоичные коды для K автоматов.
        /// </summary>
        /// <param name="k"></param>
        /// <param name="n"></param>
        /// <returns></returns>
        private static List<string> GenerateBinaryCodes(int k, int n)
        {
            return Enumerable.Range(0, k)
                .Select(i => Convert.ToString(i, 2).PadLeft(n, '0'))
                .ToList();
        }

        /// <summary>
        /// Модифицирует МСА в соответствии с кодом.
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        private static MatrixSchema ModifySchema(MatrixSchema schema, string code)
        {
            var newSchema = new MatrixSchema
            {
                Headers = new List<string>(schema.Headers),
                Rows = new List<MASRow>()
            };

            // Формируем конъюнкцию переменных P1 ... Pn по коду
            var pConjunction = CreatePConjunction(code);

            foreach (var row in schema.Rows)
            {
                var newRow = new MASRow();
                foreach (var condition in row.Transitions)
                {
                    string modifiedCondition = ModifyCondition(condition, pConjunction);
                    newRow.Transitions.Add(modifiedCondition);
                }
                newSchema.Rows.Add(newRow);
            }

            return newSchema;
        }



        /// <summary>
        /// Создает конъюнкцию переменных P по коду (например, "P1 ˄ ¬P2")
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static string CreatePConjunction(string code)
        {
            var parts = new List<string>();
            for (int i = 0; i < code.Length; i++)
            {
                parts.Add(code[i] == '0' ? $"¬P{i + 1}" : $"P{i + 1}");
            }
            return string.Join(" ˄ ", parts);
        }

        /// <summary>
        /// Модифицирует условие в ячейке
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="pConjunction"></param>
        /// <returns></returns>
        private static string ModifyCondition(string condition, string pConjunction)
        {
            if (string.IsNullOrWhiteSpace(condition)) 
                return " ";

            // Заменяем "1" на конъюнкцию P
            if (condition.Trim() == "1")
                return pConjunction;

            // Добавляем конъюнкцию P к существующим условиям
            if (!condition.Contains("˅"))
                return $"{WrapInParentheses(condition)} ˄ {pConjunction}";

            // Обработка дизъюнкций
            return $"{WrapInParentheses(condition)} ˄ {pConjunction}";
        }

        /// <summary>
        /// Заключает выражение в скобки, если необходимо
        /// </summary>
        /// <param name="condition"></param>
        /// <returns></returns>
        private static string WrapInParentheses(string condition)
        {
            if (condition.Contains("˅") && !condition.StartsWith("("))
                return $"({condition})";
            return condition;
        }

        #endregion
    }
}
