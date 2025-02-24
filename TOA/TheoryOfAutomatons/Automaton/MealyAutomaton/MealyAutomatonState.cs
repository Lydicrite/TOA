using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheoryOfAutomatons.Automaton.Common;

namespace TheoryOfAutomatons.Automaton.MealyAutomaton
{
    /// <summary>
    /// Реализует наследуемый класс состояния Автомата Мили.
    /// </summary>
    internal class MealyAutomatonState : AutomatonState<DFMealyAutomaton, MealySelfTransition>
    {
        /// <summary>
        /// Функция выходов этого состояния.
        /// </summary>
        public Dictionary<char, char> Outputs { get; set; }
        /// <summary>
        /// Функция переходов этого состояния.
        /// </summary>
        public Dictionary<char, MealyAutomatonState> Transitions { get; set; }

        /// <summary>
        /// Представляет состояние Автомата Мили.
        /// </summary>
        /// <param name="automaton">Автомат Мили, которому принадлежит это состояние.</param>
        /// <param name="index">Индекс состояния.</param>
        /// <param name="userDefinedText">Смысл того, что представляет собой это состояние.</param>
        /// <param name="initialPosition">Начальная позиция центра состояния.</param>
        public MealyAutomatonState(DFMealyAutomaton automaton, int index, string userDefinedText, Point initialPosition)
            : base(automaton, index, userDefinedText, initialPosition)
        {
            Transitions = new Dictionary<char, MealyAutomatonState>(automaton.InputAlphabet.Count);
            Outputs = new Dictionary<char, char>(automaton.OutputAlphabet.Count);
            SelfTransition = new MealySelfTransition(this);
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
            ContextMenu contextMenu = new ContextMenu();

            // Добавление связей с другими состояниями
            if (Automaton.StatesAlphabet.Any())
            {
                MenuItem addTransitionMenu = new MenuItem("Добавить связь: ...");
                foreach (var input in Automaton.InputAlphabet)
                {
                    bool transitionExists = Transitions.ContainsKey(input);

                    MenuItem inputMenu = new MenuItem($"при входе \"{input}\"...");
                    foreach (var state in Automaton.StatesAlphabet)
                    {
                        bool specificTransitionExists = Transitions.Contains(new KeyValuePair<char, MealyAutomatonState>(input, state));

                        if (!transitionExists && !specificTransitionExists)
                        {
                            inputMenu.MenuItems.Add(
                                new MenuItem($"перейти в состояние \"{state.Name}\"",
                                (s, e) => AddTransition(input, state))
                            );
                        }
                    }

                    if (inputMenu.MenuItems.Count > 0)
                        addTransitionMenu.MenuItems.Add(inputMenu);
                }

                if (addTransitionMenu.MenuItems.Count > 0)
                    contextMenu.MenuItems.Add(addTransitionMenu);
            }

            // Редактирование выходов для существующих связей
            if (Transitions.Any())
            {
                MenuItem editOutputsMenu = new MenuItem("Задать/изменить выходы...");
                foreach (var input in Transitions.Keys)
                {
                    MenuItem inputMenu = new MenuItem($"для входа '{input}':");

                    foreach (var output in Automaton.OutputAlphabet)
                    {
                        bool isCurrent = Outputs.ContainsKey(input) && Outputs[input] == output;

                        inputMenu.MenuItems.Add(
                            new MenuItem(
                                isCurrent ? $"[✓] {output}" : $"[ ] {output}",
                                (s, e) => ToggleOutput(input, output)
                            )
                        );
                    }

                    editOutputsMenu.MenuItems.Add(inputMenu);
                }
                contextMenu.MenuItems.Add(editOutputsMenu);
            }

            // Удаление связей
            if (Transitions.Any())
            {
                MenuItem removeTransitionMenu = new MenuItem("Удалить связь...");
                foreach (var func in Transitions)
                {
                    removeTransitionMenu.MenuItems.Add(
                        new MenuItem(
                            $"по входу \"{func.Key}\" с состоянием \"{func.Value.Name}\"",
                            (s, e) => RemoveTransition(func.Key)
                        )
                    );
                }
                contextMenu.MenuItems.Add(removeTransitionMenu);
            }

            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add(new MenuItem("Удалить это состояние", (s, e) => Automaton.DeleteState(this)));
            contextMenu.MenuItems.Add("-");
            contextMenu.MenuItems.Add(new MenuItem("Отмена"));

            contextMenu.Show(Automaton.Container, location);
        }

        /// <summary>
        /// Добавляет новый переход.
        /// </summary>
        /// <param name="input">Входной символ, по которому совершается переход.</param>
        /// <param name="state">Состояние, в которое совершается переход.</param>
        public override void AddTransition(char input, AutomatonState<DFMealyAutomaton, MealySelfTransition> state)
        {
            MealyAutomatonState mealyState = state as MealyAutomatonState;
            Transitions.Add(input, mealyState);

            // Инициализация пустого выхода при создании связи
            if (!Outputs.ContainsKey(input))
            {
                Outputs.Add(input, '\0');
                Automaton.OutputFunction.Add(Tuple.Create(input, this), '\0');
            }

            Automaton.TransitionFunction.Add(Tuple.Create(input, this), mealyState);

            if (state != this)
                Automaton.Transitions.Add(AutomatonTransition<MealyAutomatonState>.CreateTransition(this, mealyState, input));
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
            if (!(tr is MealySelfTransition))
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
        /// Добавляет или изменяет часть выходной функции.
        /// </summary>
        /// <param name="input">Входной символ.</param>
        /// <param name="output">Выходной символ.</param>
        public void ToggleOutput(char input, char output)
        {
            if (Outputs.ContainsKey(input))
            {
                if (Outputs[input] == output)
                {
                    Outputs.Remove(input);
                    Automaton.OutputFunction.Remove(Tuple.Create(input, this));
                }
                else
                {
                    Outputs[input] = output;
                    Automaton.OutputFunction[Tuple.Create(input, this)] = output;
                }
            }
            else
            {
                Outputs.Add(input, output);
                Automaton.OutputFunction.Add(Tuple.Create(input, this), output);
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
            return Name;
        }
    }
}