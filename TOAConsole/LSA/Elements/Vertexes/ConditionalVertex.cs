using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LSA.Elements.Common;
using TOAConsole.LSA.LSAutomaton;

namespace TOAConsole.LSA.Elements.Vertexes
{
    /// <summary>
    /// Представляет собой условную вершину ЛСА - "Xi", где i = [0, ..., n].
    /// </summary>
    internal class ConditionalVertex : LSABaseElement
    {
        /// <summary>
        /// Индекс этой условной вершины.
        /// </summary>
        public int Index { get; }
        /// <summary>
        /// Логическое значение этой условной вершины
        /// </summary>
        public bool Value { get; set; }
        /// <summary>
        /// Левый потомок этой вершины.
        /// <br>Возвращается методом GetNext(), если значение <see cref="Value"/> равно <c>false</c>.</br>
        /// </summary>
        public ILSAElement? LBS { get; set; }
        /// <summary>
        /// Правый потомок этой вершины.
        /// <br>Возвращается методом GetNext(), если значение <see cref="Value"/> равно <c>true</c>.</br>
        /// </summary>
        public ILSAElement? RBS { get; set; }

        public ConditionalVertex(int index, int pos)
        {
            Index = index;
            Position = pos;
            Id = $"X{index}";
        }

        public override string GetLongDescription() =>
            $"\nПройдена условная вершина {Index}: \"{Id}\"" +
            $"\n\tЗначение условия: {(Value ? 1 : 0)}" +
            $"\n\tПозиция в списке токенов: {Position}";
        public override ILSAElement? GetNext(Automaton automaton) => Value ? RBS : LBS;
    }
}
