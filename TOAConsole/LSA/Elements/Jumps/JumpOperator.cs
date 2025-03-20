using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LSA.Elements.Common;
using TOAConsole.LSA.LSAutomaton;

namespace TOAConsole.LSA.Elements.Jumps
{
    internal class JumpOperator : LSABaseElement
    {
        public int JumpIndex { get; }
        public bool IsUnconditional { get; }

        public JumpOperator(int index, bool isUnconditional)
        {
            JumpIndex = index;
            IsUnconditional = isUnconditional;
            Id = $"{(isUnconditional ? "w↑" : "↑")}{index}";
        }

        public override string GetOutput() => $"Оператор перехода {Id}";
        public override ILSAElement GetNext(Automaton automaton)
        {
            if (automaton.JumpPoints.TryGetValue(JumpIndex, out var jumpPoint))
                return jumpPoint;
            throw new InvalidOperationException($"Точка перехода ↓{JumpIndex} не найдена.");
        }
    }
}
