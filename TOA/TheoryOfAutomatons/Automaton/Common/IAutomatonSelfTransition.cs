using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheoryOfAutomatons.Utils.Helpers;

namespace TOA.TheoryOfAutomatons.Automaton.Common
{
    internal interface IAutomatonSelfTransition : IAutomatonFromToTransition
    {
        /// <summary>
        /// Добавляет входной символ в список входных символов.
        /// </summary>
        /// <param name="input">Символ для добавления.</param>
        public void AppendInput(char input);

        /// <summary>
        /// Удаляет символ из списка входных символов.
        /// </summary>
        /// <param name="input">Символ для удаления.</param>
        public void RemoveInput(char input);
    }
}
