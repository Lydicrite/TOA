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
    /// Представляет собой начальную вершину ЛСА - "Yн".
    /// </summary>
    internal class StartVertex : LSABaseElement
    {
        public StartVertex(int pos)
        {
            Id = "Yн";
            Position = pos;
        }

        public override string GetLongDescription() => "\n ------- Начало алгоритма ------- ";
        public override ILSAElement? GetNext(Automaton automaton) => Next;
    }
}
