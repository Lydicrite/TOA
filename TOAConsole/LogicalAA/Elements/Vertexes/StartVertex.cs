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
    /// Представляет собой начальную вершину ЛСА - "Yн".
    /// </summary>
    internal class StartVertex : LAABaseElement
    {
        public StartVertex(int pos)
        {
            Id = "Yн";
            Position = pos;
        }

        public override string Description => "\n ------- Начало алгоритма ------- ";

        public override ILAAElement? GetNext(Automaton.Automaton automaton) => Next;
    }
}
