using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LSA.Elements.Common;
using TOAConsole.LSA.LSAutomaton;

namespace TOAConsole.LSA.Elements.Vertexes
{
    internal class OperatorVertex : LSABaseElement
    {
        public int Index { get; }

        public OperatorVertex(int index)
        {
            Index = index;
            Id = $"Y{index}";
        }

        public override string GetOutput() => $"Операторная вершина \"{Id}\"";
        public override ILSAElement GetNext(Automaton automaton) => Next;
    }
}
