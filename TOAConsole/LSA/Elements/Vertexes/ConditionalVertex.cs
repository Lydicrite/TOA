using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LSA.Elements.Common;
using TOAConsole.LSA.LSAutomaton;

namespace TOAConsole.LSA.Elements.Vertexes
{
    internal class ConditionalVertex : LSABaseElement
    {
        public int Index { get; }
        public bool Value { get; set; }
        public ILSAElement LBS { get; set; }
        public ILSAElement RBS { get; set; }

        public ConditionalVertex(int index)
        {
            Index = index;
            Id = $"X{index}";
        }

        public override string GetOutput() => $"Условие {Id}: {Value}";
        public override ILSAElement GetNext(Automaton automaton) => Value ? RBS : LBS;
    }
}
