using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LogicalAA.Elements.Common;
using TOAConsole.LogicalAA.Automaton;

namespace TOAConsole.LogicalAA.Elements.Jumps
{
    /// <summary>
    /// Представляет собой точку перехода ЛСА - "↓i", где i = [0, ..., n].
    /// </summary>
    internal class JumpPoint : LAABaseElement
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

        public override string Description =>
            $"\nПройдена точка перехода {JumpIndex}: \"{Id}\"" +
            $"\n\tПозиция в списке токенов: {Position}";

        public override ILAAElement? GetNext(Automaton.Automaton automaton) => Next;
    }
}
