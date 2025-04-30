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
    /// Представляет собой оператор перехода в точку перехода ЛСА - "↑i" или "w↑i", где i = [0, ..., n] - индекс точки.
    /// </summary>
    internal class JumpOperator : LAABaseElement
    {
        /// <summary>
        /// Индекс точки перехода, соответствующей этому оператору.
        /// </summary>
        public int JumpIndex { get; }
        /// <summary>
        /// Определяет, является ли переход безусловным.
        /// </summary>
        public bool IsUnconditional { get; }

        public JumpOperator(int index, int pos, bool isUnconditional)
        {
            JumpIndex = index;
            Position = pos;
            IsUnconditional = isUnconditional;
            ID = $"{(isUnconditional ? "w↑" : "↑")}{index}";
        }

        public override string Description =>
            $"\nВыполнение перехода в точку ↓{JumpIndex}: \"{ID}\"" +
            $"\n\tПереход безусловный: {(IsUnconditional ? "да" : "нет")}" +
            $"\n\tПозиция в списке токенов: {Position}";

        public override ILAAElement? GetNext(Automaton.Automaton automaton)
        {
            if (automaton.JumpPoints.TryGetValue(JumpIndex, out var jumpPoint))
            {
                Next = jumpPoint;
                return jumpPoint;
            }
            throw new InvalidOperationException($"Точка перехода \"↓{JumpIndex}\" не найдена.");
        }
    }
}
