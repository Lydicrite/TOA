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
        /// Объединяет модифицированные МСА в единую МСА.
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
        /// Возвращает словарь схожести МСА в формате [ключ: "B(M{i}, M{j})", значение: "количество равных значимых элементов"]
        /// </summary>
        /// <param name="matrices">Список объединяемых МСА.</param>
        /// <returns>Словарь схожести МСА.</returns>
        public static Dictionary<string, int> GetSimilarityDictionary(List<MatrixSchema> matrices)
        {
            var similarityDict = new Dictionary<string, int>();

            for (int i = 0; i < matrices.Count; i++)
            {
                for (int j = i + 1; j < matrices.Count; j++)
                {
                    int similarity = matrices[i].CalculateSimilarity(matrices[j]);
                    string key = $"B(M{i + 1}, M{j + 1})";
                    similarityDict.Add(key, similarity);
                }
            }

            return similarityDict;
        }

        /// <summary>
        /// Преобразует список МСА отдельных алгоритмов в список модифицированных МСА с добавлением переменных P1 ... Pn.
        /// </summary>
        /// <param name="schemes">Список объединяемых МСА.</param>
        /// <returns>Список подготовленных к объединению МСА.</returns>
        public static List<MatrixSchema> PrepareForCombine(List<MatrixSchema> schemes)
        {
            _newPVariables.Clear();
            _binaryCodes.Clear();
            _pVarCodes.Clear();

            // Шаг 1: Вычисление попарной схожести
            var similarityMatrix = CalculateSimilarityMatrix(schemes);

            // Шаг 2: Оптимизация порядка матриц
            var orderedSchemes = OptimizeOrder(schemes, similarityMatrix);

            // Шаг 3: Генерация кодов с минимальным расстоянием
            int n = (int)Math.Ceiling(Math.Log(schemes.Count, 2));
            _binaryCodes = GenerateGrayCodes(orderedSchemes.Count, n);

            // Шаг 4: Подготовка матриц
            var preparedSchemas = new List<MatrixSchema>();
            for (int i = 0; i < orderedSchemes.Count; i++)
            {
                MatrixSchema modified = ModifySchema(orderedSchemes[i], _binaryCodes[i]);
                preparedSchemas.Add(modified);
                _pVarCodes.Add(CreatePConjunction(_binaryCodes[i], orderedSchemes[i]));
            }

            return preparedSchemas;
        }

        /// <summary>
        /// Вычисляет матрицу схожести (попарную) для всех участвующих в объединении МСА.
        /// </summary>
        /// <param name="schemes">Список объединяемых МСА.</param>
        /// <returns>Попарная матрица схожести МСА.</returns>
        private static int[,] CalculateSimilarityMatrix(List<MatrixSchema> schemes)
        {
            int size = schemes.Count;
            var matrix = new int[size, size];

            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    if (i != j)
                        matrix[i, j] = schemes[i].CalculateSimilarity(schemes[j]);

            return matrix;
        }

        /// <summary>
        /// Оптимизирует порядок матриц для минимизации расстояний (для генерации кодов Грея) на основе их матрицы схожести.
        /// </summary>
        /// <param name="schemes">Список объединяемых МСА.</param>
        /// <param name="similarityMatrix">Матрица смежности МСА.</param>
        /// <returns>Список МСА в оптимальном порядке.</returns>
        private static List<MatrixSchema> OptimizeOrder(List<MatrixSchema> schemes, int[,] similarityMatrix)
        {
            // Жадный алгоритм построения порядка
            var ordered = new List<MatrixSchema> { schemes[0] };
            var remaining = new List<MatrixSchema>(schemes.Skip(1));

            while (remaining.Count > 0)
            {
                int lastIndex = schemes.IndexOf(ordered.Last());
                var next = remaining
                    .OrderByDescending(m => similarityMatrix[
                        lastIndex,
                        schemes.IndexOf(m)
                    ])
                    .First();

                ordered.Add(next);
                remaining.Remove(next);
            }

            return ordered;
        }

        /// <summary>
        /// Генерирует коды Грея для <paramref name="count"/> автоматов.
        /// </summary>
        /// <param name="count">Количество автоматов.</param>
        /// <param name="bits">Количество переменных, используемых для кодирования.</param>
        /// <returns>Список строк с кодами Грея для кодирования МСА.</returns>
        private static List<string> GenerateGrayCodes(int count, int bits)
        {
            var codes = new List<string>();
            for (int i = 0; i < count; i++)
            {
                int grayCode = i ^ (i >> 1);
                codes.Add(Convert.ToString(grayCode, 2)
                    .PadLeft(bits, '0')
                    .Substring(0, bits));
            }
            return codes;
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
