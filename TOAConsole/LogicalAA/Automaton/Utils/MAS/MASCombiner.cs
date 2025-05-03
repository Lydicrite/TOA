using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalAA.Automaton.Utils.MAS
{
    /// <summary>
    /// Проводит объединение нескольких автоматов на основе их матричных схем <see cref="MatrixSchema"/>.
    /// </summary>
    internal static class MASCombiner
    {
        /// <summary>
        /// Список двоичных строк, которыми кодируются объединяемые МСА.
        /// </summary>
        private static List<string> _binaryCodes = new List<string>();
        /// <summary>
        /// Список значений новых P-переменных, которыми кодируются объединяемые МСА.
        /// </summary>
        private static List<string> _pVarCodes = new List<string>();
        /// <summary>
        /// Список новых P-переменных, используемых в ОМСА (ID новых условных вершин).
        /// </summary>
        private static HashSet<string> _newPVariables = new HashSet<string>();

        #region Объединение матриц

        /// <summary>
        /// Объединяет модифицированные МСА в единую матрицу.
        /// </summary>
        /// <param name="modifiedSchemes">Список подготовленных к объединению МСА.</param>
        /// <param name="binaryCodes">Список двоичных кодов, используемых для кодирования объединяемых МСА.</param>
        /// <param name="varCodes">Список из конъюнкций кодирующих переменных, используемых для кодирования объединяемых МСА.</param>
        /// <param name="newVariables">Список кодирующих переменных, используемых для кодирования объединяемых МСА.</param>
        /// <returns>Объединённая МСА.</returns>
        public static MatrixSchema CombineSchemas
        (
            List<MatrixSchema> modifiedSchemes, ref List<string> binaryCodes, 
            ref List<string> varCodes, ref HashSet<string> newVariables
        )
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

            binaryCodes = _binaryCodes;
            varCodes = _pVarCodes;
            newVariables = _newPVariables;

            return combinedSchema;
        }

        /// <summary>
        /// Упорядочивает список вершин автомата.
        /// </summary>
        /// <param name="vertices">Неупорядоченный список вершин автомата.</param>
        /// <returns>Упорядоченный список вершин автомата.</returns>
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
        /// Собирает условия из всех МСА для ячейки (<paramref name="rowVertex"/>, <paramref name="colVertex"/>).
        /// </summary>
        /// <param name="schemes">Список объединяемых МСА.</param>
        /// <param name="rowVertex">ID вершины ряда.</param>
        /// <param name="colVertex">ID вершины столбца.</param>
        /// <returns>Объединённый список условий для попадания в ячейку (<paramref name="rowVertex"/>, <paramref name="colVertex"/>).</returns>
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
        /// Объединяет список из условий по особым правилам в одну строку.
        /// </summary>
        /// <param name="conditions">Объединяемый список условий.</param>
        /// <returns>Строка, содержащая формулу перехода.</returns>
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

        #endregion



        #region Подготовка к объединению

        /// <summary>
        /// Преобразует список МСА отдельных алгоритмовв список модифицированных МСА с добавлением переменных P1 ... Pn.
        /// </summary>
        /// <param name="schemes">Список объединяемых МСА.</param>
        /// <returns>Список подготовленных к объединению МСА.</returns>
        public static List<MatrixSchema> PrepareForCombine(List<MatrixSchema> schemes)
        {
            _newPVariables.Clear();
            _binaryCodes.Clear();
            _pVarCodes.Clear();

            int n = (int)Math.Ceiling(Math.Log(schemes.Count, 2));
            var mergedSchemas = new List<MatrixSchema>();

            _binaryCodes = GenerateBinaryCodes(schemes.Count, n);

            for (int i = 0; i < schemes.Count; i++)
            {
                string code = _binaryCodes[i];
                MatrixSchema modifiedSchema = ModifySchema(schemes[i], code);
                mergedSchemas.Add(modifiedSchema);

                string pConjunction = CreatePConjunction(code, schemes[i]);
                _pVarCodes.Add(pConjunction);
            }

            return mergedSchemas;
        }

        /// <summary>
        /// Генерирует двоичные коды для <paramref name="k"/> автоматов.
        /// </summary>
        /// <param name="k">Количество автоматов.</param>
        /// <param name="n">Количество переменных, используемых для кодирования.</param>
        /// <returns>Список строк с двоичными кодами для кодирования МСА.</returns>
        private static List<string> GenerateBinaryCodes(int k, int n)
        {
            return Enumerable.Range(0, k)
                .Select(i => Convert.ToString(i, 2).PadLeft(n, '0'))
                .ToList();
        }

        /// <summary>
        /// Модифицирует МСА в соответствии с кодом.
        /// </summary>
        /// <param name="schema">Модифицируемая МСА.</param>
        /// <param name="code">Строка с двоичным кодом.</param>
        /// <returns>Модифицированная МСА.</returns>
        private static MatrixSchema ModifySchema(MatrixSchema schema, string code)
        {
            var newSchema = new MatrixSchema
            {
                Headers = new List<string>(schema.Headers),
                Rows = new List<MASRow>()
            };

            // Формируем конъюнкцию переменных Pk ... Pn по коду
            var pConjunction = CreatePConjunction(code, schema);

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
        /// Создает конъюнкцию переменных P по двоичному коду <paramref name="code"/>.
        /// </summary>
        /// <param name="code">Двоичный код в строковом варианте.</param>
        /// <param name="schema">Модифицированная МСА.</param>
        /// <returns>Строка, содержащая конъюнкцию новых переменных P в значениях, заданных <paramref name="code"/>.</returns>
        private static string CreatePConjunction(string code, MatrixSchema schema)
        {
            var schemaPVars = schema.DetectPVariables();
            int lastSchemePNumber = schemaPVars.Any()
                ? schemaPVars.Max(p => int.Parse(p[1..]))
                : 0;

            var parts = new List<string>();
            for (int i = 0; i < code.Length; i++)
            {
                if (code[i] != '0' && code[i] != '1')
                    throw new ArgumentException("Код для кодирования МСА должен содержать только '0' или '1'.");

                int pNumber = i + lastSchemePNumber + (lastSchemePNumber == 0 ? 0 : 1);
                var pID = $"P{pNumber}";

                _newPVariables.Add(pID);
                parts.Add(code[i] == '0' ? $"¬{pID}" : $"{pID}");
            }
            return string.Join(" ˄ ", parts);
        }

        /// <summary>
        /// Модифицирует формулу перехода в ячейке МСА.
        /// </summary>
        /// <param name="condition">Формула перехода в ячейке.</param>
        /// <param name="pConjunction">Конъюнкция новых P-переменных.</param>
        /// <returns>Изменённая формула перехода.</returns>
        private static string ModifyCondition(string condition, string pConjunction)
        {
            if (string.IsNullOrWhiteSpace(condition)) 
                return " ";

            // Заменяем "1" на конъюнкцию P
            if (condition.Trim() == "1")
                return pConjunction;

            // Добавляем конъюнкцию P к существующим условиям
            if (!condition.Contains('˅'))
                return $"{WrapInParentheses(condition)} ˄ {pConjunction}";

            // Обработка дизъюнкций
            return $"{WrapInParentheses(condition)} ˄ {pConjunction}";
        }

        /// <summary>
        /// Заключает формулу перехода в скобки, если необходимо.
        /// </summary>
        /// <param name="condition">Обрабатываемая формула перехода.</param>
        /// <returns>Обработанная формула перехода.</returns>
        private static string WrapInParentheses(string condition)
        {
            if (condition.Contains('˅') && !condition.StartsWith('('))
                return $"({condition})";
            return condition;
        }

        #endregion
    }
}
