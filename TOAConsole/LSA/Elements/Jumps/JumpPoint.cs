using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LSA.Elements.Common;
using TOAConsole.LSA.LSAutomaton;

namespace TOAConsole.LSA.Elements.Jumps
{
    /// <summary>
    /// Представляет собой точку перехода ЛСА - "↓i", где i = [0, ..., n].
    /// </summary>
    internal class JumpPoint : LSABaseElement
    {
        /// <summary>
        /// Индекс этой точки перехода.
        /// </summary>
        public int JumpIndex { get; }

        public JumpPoint(int index, int pos)
        {
            JumpIndex = index;
            Position = pos;
            Id = $"↓{index}";
        }

        public override string GetLongDescription() => 
            $"\nПройдена точка перехода {JumpIndex}: \"{Id}\"" +
            $"\n\tПозиция в списке токенов: {Position}";
        public override ILSAElement? GetNext(Automaton automaton) => Next;
    }
}
