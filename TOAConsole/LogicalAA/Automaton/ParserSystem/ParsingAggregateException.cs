using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalAA.Automaton.ParserSystem
{
    internal class ParsingError
    {
        public string Message { get; }
        public int Position { get; }

        public ParsingError(string message, int position)
        {
            Message = message;
            Position = position;
        }
    }

    internal class ParsingAggregateException : Exception
    {
        public List<ParsingError> Errors { get; }

        public ParsingAggregateException(List<ParsingError> errors)
            : base($"\tНайдено {errors.Count} ошибок при парсинге ЛСА: \n")
        {
            Errors = errors;
        }

        public override string Message
        {
            get
            {
                int ix = 0;
                string innerErrors = string.Empty;
                foreach (var error in Errors.OrderBy(e => e.Position))
                {
                    ix++;
                    innerErrors += $"\n\t\t{ix}) [позиция {error.Position}] - {error.Message}\n";
                }

                return base.Message + innerErrors;
            }
        }
    }
}
