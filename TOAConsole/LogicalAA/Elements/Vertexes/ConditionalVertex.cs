using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LogicalAA.Elements.Common;
using TOAConsole.LogicalAA.Automaton;

namespace TOAConsole.LogicalAA.Elements.Vertexes
{
    /// <summary>
    /// Представляет собой условную вершину ЛСА - "Xi", где i = [0, ..., n].
    /// </summary>
    internal class ConditionalVertex : LAABaseElement
    {
        /// <summary>
        /// Уникальный индекс условной вершины.
        /// </summary>
        public int Index { get; }
        /// <summary>
        /// Префикс вершины (X или P).
        /// </summary>
        public string Prefix { get; }
        /// <summary>
        /// Число из исходного токена.
        /// </summary>
        public int OriginalNumber { get; }
        /// <summary>
        /// Логическое значение этой условной вершины
        /// </summary>
        public bool? Value { get; set; }
        /// <summary>
        /// Левый потомок этой вершины.
        /// <br>Возвращается методом GetNext(), если значение <see cref="Value"/> равно <c>false</c>.</br>
        /// </summary>
        public ILAAElement? LBS { get; set; }
        /// <summary>
        /// Правый потомок этой вершины.
        /// <br>Возвращается методом GetNext(), если значение <see cref="Value"/> равно <c>true</c>.</br>
        /// </summary>
        public ILAAElement? RBS { get; set; }

        public ConditionalVertex(string prefix, int originalNumber, int index, int pos)
        {
            Prefix = prefix;
            OriginalNumber = originalNumber;
            Index = index;
            Position = pos;
            ID = $"{Prefix}{OriginalNumber}";
        }

        public override string Description
        {
            get
            {
                string condValue = Value.HasValue ? $"{(Value.Value ? 1 : 0)}" : "не установлено";
                return
                    $"\nПройдена условная вершина {Index}: \"{ID}\"" +
                    $"\n\tЗначение условия: {condValue}" +
                    $"\n\tПозиция в списке токенов: {Position}";
            }
        }

        public override ILAAElement? GetNext(Automaton.Automaton automaton) => Value.HasValue ? (Value.Value ? RBS : LBS) : null;
    }
}
