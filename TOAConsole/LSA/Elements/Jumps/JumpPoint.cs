using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LSA.Elements.Common;
using TOAConsole.LSA.LSAutomaton;

namespace TOAConsole.LSA.Elements.Jumps
{
    internal class JumpPoint : LSABaseElement
    {
        public int JumpIndex { get; }

        public JumpPoint(int index)
        {
            JumpIndex = index;
            Id = $"↓{index}";
        }

        public override string GetOutput() => $"Точка перехода {Id}";
        public override ILSAElement GetNext(Automaton automaton) => Next;
    }
}
