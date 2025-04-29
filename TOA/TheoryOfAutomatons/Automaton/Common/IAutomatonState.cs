using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheoryOfAutomatons.Automaton.Common;
using TheoryOfAutomatons.Automaton.MealyAutomaton;
using TheoryOfAutomatons.Utils.Helpers;

namespace TOA.TheoryOfAutomatons.Automaton.Common
{
    internal interface IAutomatonState
    {
        #region Параметры

        /// <summary>
        /// Определяет тип Автомата.
        /// </summary>
        AutomatonType Type { get; }
        /// <summary>
        /// Индекс состояния.
        /// </summary>
        int Index { get; }
        /// <summary>
        /// Имя состояния
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Определённый пользователем текст - смысл того, что представляет собой это состояние.
        /// </summary>
        string UserDefinedText { get; }
        /// <summary>
        /// Автомат, которому принадлежит это состояние.
        /// </summary>
        IDFAutomaton Automaton { get; }
        /// <summary>
        /// Перемещается ли в текущий момент это состояние.
        /// </summary>
        bool IsMoving { get; set; }
        /// <summary>
        /// Является ли состояние цикличным (все ли переходы являются самопереходами).
        /// </summary>
        bool IsCyclic { get; set; }
        /// <summary>
        /// Является ли состояние входным.
        /// </summary>
        bool IsInput { get; set; }
        /// <summary>
        /// Центр области состояния.
        /// </summary>
        Point StateCenter { get; }
        /// <summary>
        /// Все возможные точки для начала отрисовки переходов из этого состояния.
        /// </summary>
        List<Point> TransitionStartPoints { get; }
        /// <summary>
        /// Самопереход состояния.
        /// </summary>
        IAutomatonSelfTransition SelfTransition { get; }
        /// <summary>
        /// Функция переходов этого состояния.
        /// </summary>
        Dictionary<char, IAutomatonState> Transitions { get; }
        /// <summary>
        /// Функция выходов этого состояния.
        /// </summary>
        Dictionary<char, char> Outputs { get; }
        /// <summary>
        /// Выход этого состояния.
        /// </summary>
        char Output { get; set; }

        #endregion



        #region Отрисовки

        /// <summary>
        /// Отрисовывает состояние со всеми необходимыми обозначениями.
        /// </summary>
        /// <param name="g">Объект Graphics для отрисовки.</param>
        /// <param name="highlightBoundary">Определяет, нужно ли подсвечивать граничную область состояния.</param>
        /// <param name="highlightInner">Определяет, нужно ли подсвечивать внутреннюю область состояния.</param>
        void Draw(Graphics g, bool highlightBoundary = false, bool highlightInner = false);

        /// <summary>
        /// Отрисовывает круглую "рамку", обозначающую то, что данное состояние находится в режиме перемещения
        /// </summary>
        /// <param name="g"></param>
        void DrawMovingFrame(Graphics g);

        /// <summary>
        /// Определяет все возможные точки для начала отрисовки переходов из этого состояния.
        /// </summary>
        void CreateTransitionStartPoints();

        /// <summary>
        /// Проверяет, находится ли переданная точка во внутренней области состояния.
        /// </summary>
        /// <param name="point">Точка для проверки.</param>
        /// <returns>True, если переданная точка находится во внутренней области состояния; иначе - false.</returns>
        bool IsInInnerArea(Point point);

        /// <summary>
        /// Проверяет, находится ли переданная точка во внешней области состояния.
        /// </summary>
        /// <param name="point">Точка для проверки.</param>
        /// <returns>True, если переданная точка находится во внешней области состояния; иначе - false.</returns>
        bool IsInBoundaryArea(Point point);

        /// <summary>
        /// Устанавливает новую центральную точку состояния с учётом ограничений.
        /// </summary>
        /// <param name="newPosition">Новая центральная точка.</param>
        void SetPosition(Point newPosition);

        #endregion



        #region Контекстное меню

        /// <summary>
        /// Заполняет и показывает контекстное меню этого состояния.
        /// </summary>
        /// <param name="location">Точка для отображения контекстного меню.</param>
        void ShowContextMenu(Point location);

        /// <summary>
        /// Добавляет новый переход.
        /// </summary>
        /// <param name="input">Входной символ, по которому совершается переход.</param>
        /// <param name="state">Состояние, в которое совершается переход.</param>
        void AddTransition(char input, IAutomatonState state);

        /// <summary>
        /// Удаляет существующий переход.
        /// </summary>
        /// <param name="input">Входной символ, по которому осуществляется поиск удаляемого перехода.</param>
        void RemoveTransition(char input);

        /// <summary>
        /// Добавляет или изменяет часть выходной функции.
        /// </summary>
        /// <param name="input">Входной символ.</param>
        /// <param name="output">Выходной символ.</param>
        void ToggleOutput(char input, char output);

        #endregion



        /// <summary>
        /// Получает текст, который будет отображаться во внутренней области состояния.
        /// </summary>
        /// <returns>Строка, текст которой будет отображаться во внутренней области состояния.</returns>
        string GetDisplayText();
    }
}
