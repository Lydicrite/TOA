using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LogicalAA.Automaton;

namespace TOAConsole.LogicalAA.Elements.Common
{
    /// <summary>
    /// Интерфейс, содержащий общие и базовые свойства для всех элементов ЛСА.
    /// </summary>
    internal interface ILAAElement
    {
        /// <summary>
        /// Позиция элемента в списке токенов.
        /// </summary>
        int Position { get; }
        /// <summary>
        /// Строковый идентификатор элемента.
        /// </summary>
        string? Id { get; }
        /// <summary>
        /// Потомок этого элемента (возможно null)
        /// </summary>
        ILAAElement? Next { get; set; }

        /// <summary>
        /// Возвращает подробное описание элемента.
        /// </summary>
        /// <returns></returns>
        string Description { get; }

        /// <summary>
        /// Получает потомка для этого элемента.
        /// <br>Где нужно, использует <paramref name="automaton"/> для поиска потомка.</br>
        /// </summary>
        /// <param name="automaton">Объект, реализующий ЛСА.</param>
        /// <returns></returns>
        ILAAElement? GetNext(Automaton.Automaton automaton);
    }

    /// <summary>
    /// Класс, предоставляющий возможности реализации для общих и базовых свойств всех элементов ЛСА.
    /// </summary>
    internal abstract class LAABaseElement : ILAAElement
    {
        public int Position { get; protected set; }
        public string? Id { get; protected set; }
        public ILAAElement? Next { get; set; }

        public abstract string Description { get; }

        public abstract ILAAElement? GetNext(Automaton.Automaton automaton);
    }
}
