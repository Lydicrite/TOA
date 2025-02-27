using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheoryOfAutomatons.Automaton.Common;

namespace TheoryOfAutomatons.Automaton.MooreAutomaton
{
    /// <summary>
    /// Реализует наследуемый класс состояния Автомата Мура.
    /// </summary>
    internal class MooreAutomatonState : AutomatonState<DFMooreAutomaton, MooreSelfTransition>
    {
        /// <summary>
        /// Выход этого состояния.
        /// </summary>
        public char Output { get; set; }
        /// <summary>
        /// Функция переходов этого состояния.
        /// </summary>
        public Dictionary<char, MooreAutomatonState> Transitions { get; set; }

        /// <summary>
        /// Представляет состояние Автомата Мура.
        /// </summary>
        /// <param name="automaton">Автомат Мура, которому принадлежит это состояние.</param>
        /// <param name="index">Индекс состояния.</param>
        /// <param name="userDefinedText">Смысл того, что представляет собой это состояние.</param>
        /// <param name="initialPosition">Начальная позиция центра состояния.</param>
        public MooreAutomatonState(DFMooreAutomaton automaton, int index, string userDefinedText, Point initialPosition)
            : base(automaton, index, userDefinedText, initialPosition)
        {
            Output = '\0';
            Transitions = new Dictionary<char, MooreAutomatonState>(automaton.InputAlphabet.Count);
            SelfTransition = new MooreSelfTransition(this);
            automaton.Transitions.Add(SelfTransition);
        }

        /// <summary>
        /// Отрисовывает состояние со всеми необходимыми обозначениями.
        /// </summary>
        /// <param name="g">Объект Graphics для отрисовки.</param>
        /// <param name="highlightBoundary">Определяет, нужно ли подсвечивать граничную область состояния.</param>
        /// <param name="highlightInner">Определяет, нужно ли подсвечивать внутреннюю область состояния.</param>
        public override void Draw(Graphics g, bool highlightBoundary = false, bool highlightInner = false)
        {
            base.Draw(g, highlightBoundary, highlightInner);
        }



        #region Контекстное меню

        /// <summary>
        /// Заполняет и показывает контекстное меню этого состояния.
        /// </summary>
        /// <param name="location">Точка для отображения контекстного меню.</param>
        public override void ShowContextMenu(Point location)
        {
            ContextMenuStrip contextMenu = new ContextMenuStrip();

            // Добавление связей с другими состояниями
            if (Automaton.StatesAlphabet.Any())
            {
                ToolStripMenuItem addTransitionMenu = new ToolStripMenuItem("Добавить связь: ...");
                foreach (var input in Automaton.InputAlphabet)
                {
                    bool transitionExists = Transitions.ContainsKey(input);

                    ToolStripMenuItem inputMenu = new ToolStripMenuItem($"при входе \"{input}\"...");
                    foreach (var state in Automaton.StatesAlphabet)
                    {
                        bool specificTransitionExists = Transitions.Contains(new KeyValuePair<char, MooreAutomatonState>(input, state));

                        if (!transitionExists && !specificTransitionExists)
                        {
                            inputMenu.DropDownItems.Add(
                                new ToolStripMenuItem($"перейти в состояние \"{state.Name}\"",
                                null, (s, e) => AddTransition(input, state))
                            );
                        }
                    }

                    if (inputMenu.DropDownItems.Count > 0)
                        addTransitionMenu.DropDownItems.Add(inputMenu);
                }

                if (addTransitionMenu.DropDownItems.Count > 0)
                    contextMenu.Items.Add(addTransitionMenu);
            }

            // Редактирование выходов
            ToolStripMenuItem editOutputMenu = new ToolStripMenuItem("Задать выход");
            foreach (var output in Automaton.OutputAlphabet)
            {
                editOutputMenu.DropDownItems.Add(
                    new ToolStripMenuItem(
                        Output == output ? $"[✓] {output}" : $"[ ] {output}",
                        null, (s, e) => ToggleOutput(output))
                );
            }
            contextMenu.Items.Add(editOutputMenu);

            // Удаление связей
            if (Transitions.Any())
            {
                ToolStripMenuItem removeTransitionMenu = new ToolStripMenuItem("Удалить связь...");
                foreach (var func in Transitions)
                {
                    removeTransitionMenu.DropDownItems.Add(
                        new ToolStripMenuItem(
                            $"по входу \"{func.Key}\" с состоянием \"{func.Value.Name}\"",
                            null, (s, e) => RemoveTransition(func.Key))
                    );
                }
                contextMenu.Items.Add(removeTransitionMenu);
            }

            // Разделитель и пункты управления состоянием
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(new ToolStripMenuItem("Удалить это состояние",
                null, (s, e) => Automaton.DeleteState(this)));
            contextMenu.Items.Add(new ToolStripSeparator());
            contextMenu.Items.Add(new ToolStripMenuItem("Отмена"));

            contextMenu.Show(Automaton.Container, location);
        }

        /// <summary>
        /// Добавляет новый переход.
        /// </summary>
        /// <param name="input">Входной символ, по которому совершается переход.</param>
        /// <param name="state">Состояние, в которое совершается переход.</param>
        public override void AddTransition(char input, AutomatonState<DFMooreAutomaton, MooreSelfTransition> state)
        {
            MooreAutomatonState mooreState = state as MooreAutomatonState;
            Transitions.Add(input, mooreState);

            // Инициализация выхода по умолчанию
            if (Output == '\0')
            {
                Output = '\0';
                Automaton.OutputFunction[this] = '\0';
            }

            Automaton.TransitionFunction.Add(Tuple.Create(input, this), mooreState);

            if (state != this)
                Automaton.Transitions.Add(AutomatonTransition<MooreAutomatonState>.CreateTransition(this, mooreState, input));
            else
            {
                Automaton.Transitions.Remove(SelfTransition);
                SelfTransition.AppendInput(input);
                Automaton.Transitions.Add(SelfTransition);
            }

            if (Transitions.All(kvp => kvp.Value == this) && Transitions.Count() == Automaton.InputAlphabet.Count())
                IsCyclic = true;
            else
                IsCyclic = false;

            Automaton.OnTransitionsChanged();
        }

        /// <summary>
        /// Удаляет существующий переход.
        /// </summary>
        /// <param name="input">Входной символ, по которому осуществляется поиск удаляемого перехода.</param>
        protected override void RemoveTransition(char input)
        {
            Transitions.Remove(input);
            Automaton.TransitionFunction.Remove(Tuple.Create(input, this));

            var tr = Automaton.Transitions.Find(t => t.From == this && t.Annotation.Contains(input));
            if (!(tr is MooreSelfTransition))
                Automaton.Transitions.Remove(tr);
            else
            {
                Automaton.Transitions.Remove(SelfTransition);
                SelfTransition.RemoveInput(input);
                Automaton.Transitions.Add(SelfTransition);
            }

            if (Transitions.All(kvp => kvp.Value == this) && Transitions.Count() == Automaton.InputAlphabet.Count())
                IsCyclic = true;
            else
                IsCyclic = false;

            Automaton.OnTransitionsChanged();
        }

        /// <summary>
        /// Добавляет или изменяет выходной символ.
        /// </summary>
        /// <param name="output">Выходной символ.</param>
        private void ToggleOutput(char output)
        {
            if (Output == output)
            {
                Output = '\0';
                Automaton.OutputFunction.Remove(this);
            }
            else
            {
                Output = output;
                Automaton.OutputFunction[this] = output;
            }

            Automaton.OnTransitionsChanged();
        }

        #endregion



        /// <summary>
        /// Получает текст, который будет отображаться во внутренней области состояния.
        /// </summary>
        /// <returns>Строка, текст которой будет отображаться во внутренней области состояния.</returns>
        protected override string GetDisplayText()
        {
            return $"{Name}/{Output}";
        }
    }
}