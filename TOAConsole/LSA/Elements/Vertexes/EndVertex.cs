using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LSA.Elements.Common;
using TOAConsole.LSA.LSAutomaton;

namespace TOAConsole.LSA.Elements.Vertexes
{
    internal class EndVertex : LSABaseElement
    {
        public EndVertex()
        {
            Id = "Yк";
        }

        public override string GetOutput() => " ------- Конец алгоритма ------- ";
        public override ILSAElement GetNext(Automaton automaton) => null;
    }
}
