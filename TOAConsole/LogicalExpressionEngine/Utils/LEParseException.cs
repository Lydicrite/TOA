using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalExpressionEngine.Utils
{
    /// <summary>
    /// Представляет исключения, возникающее при работе парсера логических выражений.
    /// </summary>
    [Serializable]
    internal sealed class LEParseException : Exception
    {
        /// <summary>
        /// Позиция в списке токенов, где произошла ошибка.
        /// </summary>
        public int Position { get; }
        public LEParseException(string message, int pos) : base($"{message} (Позиция: {pos})")
            => Position = pos;
    }
}
