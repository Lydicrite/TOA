using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalExpressionParser.Utils
{
    [Serializable]
    internal class ExpressionParseException : Exception
    {
        public int Position { get; }
        public ExpressionParseException(string message, int pos) : base($"{message} (Позиция: {pos})")
            => Position = pos;
    }
}
