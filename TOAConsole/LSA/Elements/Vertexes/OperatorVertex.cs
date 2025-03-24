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
    /// Представляет собой операторную вершину ЛСА - "Yi", где i = [0, ..., n].
    /// </summary>
    internal class OperatorVertex : LSABaseElement
    {
        /// <summary>
        /// Индекс этой операторной вершины.
        /// </summary>
        public int Index { get; }

        public OperatorVertex(int index, int pos)
        {
            Index = index;
            Position = pos;
            Id = $"Y{index}";
        }

        public override string GetLongDescription() => 
            $"\nПройдена операторная вершина {Index}: \"{Id}\"" +
            $"\nПозиция в списке токенов: {Position}";
        public override ILSAElement? GetNext(Automaton automaton) => Next;
    }
}
