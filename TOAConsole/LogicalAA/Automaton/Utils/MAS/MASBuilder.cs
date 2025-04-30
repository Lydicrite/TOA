using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LogicalAA.Elements.Common;
using TOAConsole.LogicalAA.Elements.Jumps;
using TOAConsole.LogicalAA.Elements.Vertexes;

namespace TOAConsole.LogicalAA.Automaton.Utils.MAS
{
    /// <summary>
    /// Строит матричную схему алгоритма (МСА) <see cref="MatrixSchema"/> для объекта <see cref="Automaton"/>.
    /// </summary>
    internal static class MASBuilder
    {
        /// <summary>
        /// Создает матричную схему алгоритма (МСА) для переданного автомата <paramref name="automaton"/>.
        /// </summary>
        /// <param name="automaton">Автомат, для которого необходимо создать МСА.</param>
        /// <returns>Объект <see cref="MatrixSchema"/>, представляющий собой МСА.</returns>
        public static MatrixSchema GenerateMAS(Automaton automaton)
        {
            var operatorVertices = automaton.Elements
                .OfType<OperatorVertex>()
                .OrderBy(v => v.Index)
                .ToList();

            var start = automaton.Elements.OfType<StartVertex>().First();
            var end = automaton.Elements.OfType<EndVertex>().First();

            var rowsVertexes = new List<ILAAElement> { start };
            rowsVertexes.AddRange(operatorVertices);

            var colsVertexes = rowsVertexes;
            colsVertexes.Add(end);

            var matrix = new MatrixSchema();
            matrix.Headers = rowsVertexes.Select(v => v.ID).ToList();

            // Получаем все возможные пути для всех комбинаций условий
            var pathConditions = AnalyzeAllPaths(automaton, rowsVertexes, colsVertexes);

            // Заполняем матрицу на основе собранных условий
            foreach (var rowVertex in rowsVertexes)
            {
                var row = new MASRow();

                foreach (var colVertex in colsVertexes)
                {
                    var key = (From: rowVertex, To: colVertex);
                    row.Transitions.Add(pathConditions.TryGetValue(key, out var conditions)
                        ? FormatConditions(conditions)
                        : " ");
                }

                matrix.Rows.Add(row);
            }

            return matrix;
        }

        private static Dictionary<(ILAAElement From, ILAAElement To), List<List<string>>> AnalyzeAllPaths
        (
            Automaton automaton,
            List<ILAAElement> rows,
            List<ILAAElement> cols
        )
        {
            var pathConditions = new Dictionary<(ILAAElement, ILAAElement), List<List<string>>>();
            var conditionals = automaton.Elements.OfType<ConditionalVertex>().ToList();
            var totalConditions = conditionals.Count;

            // Перебираем все возможные комбинации условий
            for (int i = 0; i < Math.Pow(2, totalConditions); i++)
            {
                string binary = Convert.ToString(i, 2).PadLeft(totalConditions, '0');
                automaton.SetConditionsFromBinary(binary);

                // Получаем путь для текущей комбинации
                var path = GetExecutionPath(automaton);

                // Анализируем путь на наличие допустимых переходов
                AnalyzePath(path, rows, cols, binary, conditionals, pathConditions);
            }

            return pathConditions;
        }

        private static List<ILAAElement> GetExecutionPath(Automaton automaton)
        {
            var path = new List<ILAAElement>();
            var current = automaton.Elements.First();
            var visited = new Dictionary<ILAAElement, int>();
            const int maxLoopIterations = 3;

            while (current != null && current is not EndVertex)
            {
                // Защита от бесконечных циклов
                if (visited.TryGetValue(current, out var count) && count >= maxLoopIterations) break;
                visited[current] = visited.TryGetValue(current, out var c) ? c + 1 : 1;

                path.Add(current);
                current = current.GetNext(automaton);
            }

            if (current is EndVertex) path.Add(current);

            return path;
        }

        private static void AnalyzePath
        (
            List<ILAAElement> path,
            List<ILAAElement> rows,
            List<ILAAElement> cols,
            string binaryConditions,
            List<ConditionalVertex> conditionals,
            Dictionary<(ILAAElement, ILAAElement), List<List<string>>> results
        )
        {
            // Собираем все Y-вершины в пути
            var yVertices = path
                .Select((el, idx) => new { Element = el, Index = idx })
                .Where(x => IsOperatorVertex(x.Element))
                .ToList();

            // Анализируем все возможные пары
            for (int i = 0; i < yVertices.Count; i++)
            {
                for (int j = i + 1; j < yVertices.Count; j++)
                {
                    var from = yVertices[i].Element;
                    var to = yVertices[j].Element;
                    var start = yVertices[i].Index;
                    var end = yVertices[j].Index;

                    // Проверяем отсутствие промежуточных Y
                    if (HasIntermediateOV(path, start, end)) continue;

                    var key = (from, to);
                    var conditions = CollectConditionsForSegment(path, start, end, binaryConditions, conditionals);

                    if (!results.ContainsKey(key)) results[key] = new List<List<string>>();
                    results[key].Add(conditions);
                }
            }
        }

        private static List<string> CollectConditionsForSegment
        (
            List<ILAAElement> path,
            int startIdx,
            int endIdx,
            string binaryConditions,
            List<ConditionalVertex> conditionals
        )
        {
            var conditions = new List<string>();
            var sortedConditionals = conditionals.OrderBy(c => c.Index).ToList();

            for (int i = startIdx; i < endIdx; i++)
            {
                if (path[i] is ConditionalVertex cv)
                {
                    int cvIndex = sortedConditionals.IndexOf(cv);
                    if (cvIndex < 0 || cvIndex >= binaryConditions.Length) continue;

                    bool requiresRBS = path[i + 1] == cv.RBS;
                    bool actualValue = binaryConditions[cvIndex] == '1';

                    conditions.Add(requiresRBS == actualValue
                        ? (requiresRBS ? $"{cv.ID}" : $"¬{cv.ID}")
                        : (requiresRBS ? $"¬{cv.ID}" : $"{cv.ID}"));
                }
            }

            return conditions;
        }



        private static bool IsOperatorVertex(ILAAElement element)
            => element is StartVertex || element is OperatorVertex || element is EndVertex;

        private static bool HasIntermediateOV(List<ILAAElement> path, int start, int end)
        {
            for (int i = start + 1; i < end; i++)
                if (IsOperatorVertex(path[i])) return true;

            return false;
        }

        private static bool HasIntermediateOV(List<ILAAElement> path)
        {
            return path.Skip(1).Take(path.Count - 2).Any(IsOperatorVertex);
        }

        private static string FormatConditions(List<List<string>> conditionGroups)
        {
            if (conditionGroups.Count == 0) 
                return " ";

            var disjunction = new List<string>();

            foreach (var group in conditionGroups)
            {
                if (group.Count == 0) 
                    return "1";

                disjunction.Add($"{string.Join(" ˄ ", group)}");
            }

            return string.Join(" ˅ ", disjunction.Distinct());
        }
    }
}
