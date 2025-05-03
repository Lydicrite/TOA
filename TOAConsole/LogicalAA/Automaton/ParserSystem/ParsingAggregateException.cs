﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalAA.Automaton.ParserSystem
{
    /// <summary>
    /// Представляет контейнер для данных об ошибке, происходящей при парсинге.
    /// </summary>
    internal sealed class ParsingError
    {
        /// <summary>
        /// Сообщение об ошибке.
        /// </summary>
        public string Message { get; }
        /// <summary>
        /// Позиция в списке токенов, где произошла ошибка.
        /// </summary>
        public int Position { get; }

        /// <summary>
        /// Создаёт новую ошибку парсинга с необходимыми данными.
        /// </summary>
        /// <param name="message">Сообщение об ошибке.</param>
        /// <param name="position">Позиция в списке токенов, где произошла ошибка.</param>
        public ParsingError(string message, int position)
        {
            Message = message;
            Position = position;
        }
    }

    /// <summary>
    /// Представляет класс исключения, возникающего при появлении ошибок при парсинге.
    /// </summary>
    internal sealed class ParsingAggregateException : Exception
    {
        /// <summary>
        /// Список ошибок парсинга.
        /// </summary>
        public List<ParsingError> Errors { get; }

        /// <summary>
        /// Создаёт исключение со списком обнаруженных ошибок парсинга.
        /// </summary>
        /// <param name="errors">Список ошибок парсинга.</param>
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
