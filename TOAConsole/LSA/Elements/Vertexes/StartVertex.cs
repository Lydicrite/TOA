using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LSA.Elements.Common;
using TOAConsole.LSA.LSAutomaton;

namespace TOAConsole.LSA.Elements.Vertexes
{
    internal class StartVertex : LSABaseElement
    {
        public StartVertex()
        {
            Id = "Yн";
        }

        public override string GetOutput() => " ------- Начало алгоритма ------- ";
        public override ILSAElement GetNext(Automaton automaton) => Next;
    }
}
