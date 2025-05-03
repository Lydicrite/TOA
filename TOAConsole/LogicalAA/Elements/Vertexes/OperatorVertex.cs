using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LogicalAA.Elements.Common;
using TOAConsole.LogicalAA.Automaton;

namespace TOAConsole.LogicalAA.Elements.Vertexes
{
    /// <summary>
    /// Представляет собой операторную вершину ЛСА - "Yi", где i = [0, ..., n].
    /// </summary>
    internal sealed class OperatorVertex : LAABaseElement
    {
        /// <summary>
        /// Индекс этой операторной вершины.
        /// </summary>
        public int Index { get; }

        public OperatorVertex(int index, int pos)
        {
            Index = index;
            Position = pos;
            ID = $"Y{index}";
        }

        public override string Description =>
            $"\nПройдена операторная вершина {Index}: \"{ID}\"" +
            $"\nПозиция в списке токенов: {Position}";

        public override ILAAElement? GetNext(Automaton.Automaton automaton) => Next;
    }
}
