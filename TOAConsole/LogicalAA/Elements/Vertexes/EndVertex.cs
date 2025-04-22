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
    /// Представляет собой конечную вершину ЛСА - "Yк".
    /// </summary>
    internal class EndVertex : LAABaseElement
    {
        public EndVertex(int pos)
        {
            Id = "Yк";
            Position = pos;
        }

        public override string Description => "\n ------- Конец алгоритма ------- ";

        public override ILAAElement? GetNext(Automaton.Automaton automaton) => null;
    }
}
