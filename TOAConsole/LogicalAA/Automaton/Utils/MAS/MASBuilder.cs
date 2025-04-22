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
            matrix.Headers = rowsVertexes.Select(v => v.Id).ToList();

            foreach (var rowVertex in rowsVertexes)
            {
                var row = new MASRow();

                foreach (var colVertex in colsVertexes)
                {
                    if (rowVertex == colVertex)
                    {
                        // Ищем циклы Yi -> Yi
                        var selfConditions = FindTransitionConditions(automaton, rowVertex, colVertex);
                        row.Transitions.Add(selfConditions.Any()
                            ? string.Join(" ˅ ", selfConditions)
                            : " ");
                        continue;
                    }

                    // Пропускаем пары с промежуточными Y
                    var pathExists = HasDirectPath(automaton, rowVertex, colVertex);
                    row.Transitions.Add(pathExists
                        ? string.Join(" ˅ ", FindTransitionConditions(automaton, rowVertex, colVertex))
                        : " ");
                }

                matrix.Rows.Add(row);
            }

            return matrix;
        }

        /// <summary>
        /// Проверяет существование прямого пути между элементами без промежуточных Y-вершин.
        /// </summary>
        /// <param name="automaton">Целевой автомат.</param>
        /// <param name="from">Начальный элемент.</param>
        /// <param name="to">Целевой элемен.т</param>
        /// <returns><see langword="true"/>, если путь существует без промежуточных Y-вершин, иначе - <see langword="false"/>.</returns>
        private static bool HasDirectPath(Automaton automaton, ILAAElement from, ILAAElement to)
        {
            var visited = new HashSet<ILAAElement>();
            var queue = new Queue<ILAAElement>();
            queue.Enqueue(from);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (current == to) return true;

                foreach (var next in GetNextElements(current, automaton))
                {
                    // Разрешаем переходы через не-Y элементы (X, ↓, ↑)
                    if (next is OperatorVertex op && op != from && op != to)
                        continue;

                    if (!visited.Contains(next))
                    {
                        visited.Add(next);
                        queue.Enqueue(next);
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Находит условия перехода между двумя вершинами <paramref name="from"/> и <paramref name="to"/>.
        /// </summary>
        /// <param name="automaton">Автомат, для которого необходимо создать МСА.</param>
        /// <param name="from">Вершина, от которой нужно строить логическую формулу перехода.</param>
        /// <param name="to">Вершина, до которой должна строиться логическая формула перехода.</param>
        /// <returns>Список, содержащий строковые логические формулы переходов.</returns>
        private static List<string> FindTransitionConditions
        (
            Automaton automaton,
            ILAAElement from,
            ILAAElement to
        )
        {
            var conditions = new List<string>();
            var visited = new HashSet<ILAAElement>();

            ExplorePaths(
                current: from,
                target: to,
                originalFrom: from, // Указываем исходную вершину
                currentConjunction: new List<string>(),
                conditions: conditions,
                visited: visited,
                automaton: automaton,
                hasIntermediateY: false
            );

            return conditions.Distinct().ToList();
        }

        /// <summary>
        /// Исследует корректные пути от текущего (<paramref name="originalFrom"/>) элемента автомата <paramref name="automaton"/> до целевого (<paramref name="target"/>) элемента.
        /// </summary>
        /// <param name="current">Текущая вершина автомата.</param>
        /// <param name="target">Вершина, до которой нужно исследовать все пути.</param>
        /// <param name="originalFrom">Оригинальная начальная вершина, от которой нужно исследовать все пути.</param>
        /// <param name="currentConjunction">Текущая конъюнкция (список из <see cref="ConditionalVertex"/>.Id всех условных вершин, входящих в эту конъюнкцию).</param>
        /// <param name="conditions">Строка, представляющая дизъюнкцию конъюнкций (частичную логическую формулу перехода между Y{i} и Y{j}).</param>
        /// <param name="visited">Хэш-сет для регистрации уже посещённых элементов.</param>
        /// <param name="automaton">Автомат, для которого необходимо создать МСА.</param>
        /// <param name="hasIntermediateY">Определяет, содержится ли в пути внутренняя операторная вершина.</param>
        private static void ExplorePaths
        (
            ILAAElement current,
            ILAAElement target,
            ILAAElement originalFrom,
            List<string> currentConjunction,
            List<string> conditions,
            HashSet<ILAAElement> visited,
            Automaton automaton,
            bool hasIntermediateY
        )
        {
            if (visited.Contains(current)) return;
            visited.Add(current);

            // Промежуточным считается только Y, не являющийся исходной или целевой вершиной
            if (current is OperatorVertex op && op != originalFrom && op != target)
            {
                hasIntermediateY = true;
            }

            var nextElements = GetNextElements(current, automaton);

            foreach (var next in nextElements)
            {
                var newConjunction = new List<string>(currentConjunction);
                var newVisited = new HashSet<ILAAElement>(visited);
                var newHasIntermediateY = hasIntermediateY;

                // Добавляем условие для условных вершин
                if (current is ConditionalVertex cond)
                {
                    string condition = next == cond.LBS ? $"¬X{cond.Index}" : $"X{cond.Index}";
                    newConjunction.Add(condition);
                }

                // Обрабатываем циклы Yi -> Yi
                if (next == target && !newHasIntermediateY)
                {
                    conditions.Add(newConjunction.Any()
                        ? string.Join(" ˄ ", newConjunction)
                        : "1");
                }

                // Рекурсивный вызов даже если next == current (для циклов)
                ExplorePaths(
                    next,
                    target,
                    originalFrom,
                    newConjunction,
                    conditions,
                    newVisited,
                    automaton,
                    newHasIntermediateY
                );
            }
        }




        /// <summary>
        /// Возвращает список следующих элементов (в количестве 1 или 2 элемента) для элемента <paramref name="element"/> автомата <paramref name="automaton"/>.
        /// </summary>
        /// <param name="element">Элемент, для которого нужно сгенерировать список.</param>
        /// <param name="automaton">Автомат, для которого необходимо создать МСА.</param>
        /// <returns></returns>
        private static List<ILAAElement> GetNextElements
        (
            ILAAElement element,
            Automaton automaton
        )
        {
            if (element == null) 
                return new List<ILAAElement>();

            else if (element is ConditionalVertex cv)
                return new List<ILAAElement> { cv.LBS, cv.RBS }.Where(e => e != null).ToList();

            else if (element is JumpOperator jo)
                return new List<ILAAElement> { jo.GetNext(automaton) }.Where(e => e != null).ToList();

            return new List<ILAAElement> { element.GetNext(automaton) }.Where(e => e != null).ToList();
        }
    }
}
