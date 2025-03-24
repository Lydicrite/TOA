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
    /// Представляет собой точку перехода ЛСА - "↑i" или "w↑i", где i = [0, ..., n].
    /// </summary>
    internal class JumpOperator : LSABaseElement
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
            Id = $"{(isUnconditional ? "w↑" : "↑")}{index}";
        }

        public override string GetLongDescription() =>
            $"\nВыполнение перехода в точку ↓{JumpIndex}: \"{Id}\"" +
            $"\n\tПереход безусловный: {(IsUnconditional ? "да" : "нет")}" +
            $"\n\tПозиция в списке токенов: {Position}";
        public override ILSAElement? GetNext(Automaton automaton)
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
