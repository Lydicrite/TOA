using System;
using System.Collections.Generic;
using System.Drawing;

using TheoryOfAutomatons.Automaton.MealyAutomaton;
using TheoryOfAutomatons.Automaton.MooreAutomaton;
using TheoryOfAutomatons.Utils.Helpers;

namespace TheoryOfAutomatons.Automaton.Common
{
    /// <summary>
    /// Реализует абстрактный класс состояния Автомата, содержащий общие для состояний Автоматов Мили и Мура поля, члены, методы и функции.
    /// </summary>
    /// <typeparam name="TAutomaton">Тип Автомата</typeparam>
    /// <typeparam name="TSelfTransition">Тип самоперехода</typeparam>
    internal abstract class AutomatonState<TAutomaton, TSelfTransition>
        where TAutomaton : class
        where TSelfTransition : class
    {
        // Общие параметры для работы и визуализации
        /// <summary>
        /// Индекс состояния.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// Имя состояния
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Определённый пользователем текст - смысл того, что представляет собой это состояние.
        /// </summary>
        public string UserDefinedText { get; protected set; }

        /// <summary>
        /// Автомат, которому принадлежит это состояние.
        /// </summary>
        public TAutomaton Automaton { get; protected set; }

        /// <summary>
        /// Перемещается ли в текущий момент это состояние.
        /// </summary>
        public bool IsMoving { get; set; }

        /// <summary>
        /// Является ли состояние цикличным (все ли переходы являются самопереходами).
        /// </summary>
        public bool IsCyclic { get; set; }

        /// <summary>
        /// Является ли состояние входным.
        /// </summary>
        public bool IsInput { get; set; }

        /// <summary>
        /// Центр области состояния.
        /// </summary>
        public Point StateCenter { get; protected set; }

        /// <summary>
        /// Все возможные точки для начала отрисовки переходов из этого состояния.
        /// </summary>
        public List<Point> TransitionStartPoints { get; protected set; }

        /// <summary>
        /// Самопереход состояния.
        /// </summary>
        public TSelfTransition SelfTransition { get; protected set; }

        /// <summary>
        /// Представляет базовый абстрактный класс состояния Автомата.
        /// </summary>
        /// <param name="automaton">Автомат, которому принадлежит это состояние.</param>
        /// <param name="index">Индекс состояния.</param>
        /// <param name="userDefinedText">Смысл того, что представляет собой это состояние.</param>
        /// <param name="initialPosition">Начальная позиция центра состояния.</param>
        /// <exception cref="NotSupportedException">Выбрасывается в случае неподдерживаемого типа Автомата, типа самоперехода или неподдерживаемого их сочетания.</exception>
        protected AutomatonState(TAutomaton automaton, int index, string userDefinedText, Point initialPosition)
        {
            // Проверка на допустимые классы для Автомата
            if (!(typeof(TAutomaton) == typeof(DFMealyAutomaton) || typeof(TAutomaton) == typeof(DFMooreAutomaton)))
                throw new NotSupportedException("Неподдерживаемый тип Автомата.");

            // Проверка на допустимые классы для Автомата
            if (!(typeof(TSelfTransition) == typeof(MealySelfTransition) || typeof(TSelfTransition) == typeof(MooreSelfTransition)))
                throw new NotSupportedException("Неподдерживаемый тип самоперехода.");

            // Проверка на совместимость класса Автомата и класса самоперехода
            if
            (!((typeof(TAutomaton) == typeof(DFMealyAutomaton) && typeof(TSelfTransition) == typeof(MealySelfTransition)) ||
              (typeof(TAutomaton) == typeof(DFMooreAutomaton) && typeof(TSelfTransition) == typeof(MooreSelfTransition)))
            )
                throw new NotSupportedException("Неподдерживаемое сочетание типов Автомата и самоперехода.");

            Automaton = automaton;
            Name = $"S{index}";
            Index = index;
            UserDefinedText = userDefinedText;
            StateCenter = initialPosition.IsEmpty ? Point.Empty : initialPosition;

            TransitionStartPoints = new List<Point>();
            IsMoving = false;
            IsCyclic = false;
            IsInput = false;

            CreateTransitionStartPoints();
        }





        #region Отрисовки

        /// <summary>
        /// Отрисовывает состояние со всеми необходимыми обозначениями.
        /// </summary>
        /// <param name="g">Объект Graphics для отрисовки.</param>
        /// <param name="highlightBoundary">Определяет, нужно ли подсвечивать граничную область состояния.</param>
        /// <param name="highlightInner">Определяет, нужно ли подсвечивать внутреннюю область состояния.</param>
        public virtual void Draw(Graphics g, bool highlightBoundary = false, bool highlightInner = false)
        {
            // Настройка графических параметров
            DrawHelper.SetGraphicsParameters(g);

            // Получение размеров круга и толщины границы
            int circleDiameter = AutomatonStatesHelper.GetCircleDiameter(Automaton);
            int boundaryThickness = AutomatonStatesHelper.GetCircleBound(Automaton);

            // Определение центра состояния
            Point stateCenter = StateCenter;
            int halfDiameter = circleDiameter / 2;

            // Проверка, нужно ли выделять внутреннюю область
            bool isInnerHighlighted = highlightInner && ((dynamic)Automaton).Container.Enabled;

            // Очистка внутренней области
            g.FillEllipse(
                new SolidBrush(((dynamic)Automaton).InnerStateColor),
                stateCenter.X - halfDiameter,
                stateCenter.Y - halfDiameter,
                circleDiameter,
                circleDiameter
            );

            // Определение цвета для границы
            Brush boundaryBrush = GetBoundaryBrush(isInnerHighlighted);

            using (Pen boundaryPen = new Pen(boundaryBrush, boundaryThickness))
            {
                // Отрисовка главной границы
                g.DrawEllipse(
                    boundaryPen,
                    stateCenter.X - halfDiameter,
                    stateCenter.Y - halfDiameter,
                    circleDiameter,
                    circleDiameter
                );

                // Отрисовка второго круга для циклического состояния
                if (IsCyclic)
                {
                    Brush cyclicBrush = GetBoundaryBrush(isInnerHighlighted); // Можно использовать тот же метод
                    using (Pen cyclicPen = new Pen(cyclicBrush, 2))
                    {
                        int offset = 9;
                        int cyclicDiameter = circleDiameter - 18;
                        g.DrawEllipse(
                            cyclicPen,
                            stateCenter.X - halfDiameter + offset,
                            stateCenter.Y - halfDiameter + offset,
                            cyclicDiameter,
                            cyclicDiameter
                        );
                    }
                }
            }

            // Подсветка границы, если требуется
            if (highlightBoundary && ((dynamic)Automaton).Container.Enabled)
            {
                using (Pen highlightPen = new Pen(Color.Blue, 1))
                {
                    int highlightOffset = 5;
                    g.DrawEllipse(
                        highlightPen,
                        stateCenter.X - halfDiameter - highlightOffset,
                        stateCenter.Y - halfDiameter - highlightOffset,
                        circleDiameter + 2 * highlightOffset,
                        circleDiameter + 2 * highlightOffset
                    );

                    int innerHighlightOffset = 5;
                    g.DrawEllipse(
                        highlightPen,
                        stateCenter.X - halfDiameter + innerHighlightOffset,
                        stateCenter.Y - halfDiameter + innerHighlightOffset,
                        circleDiameter - 2 * innerHighlightOffset,
                        circleDiameter - 2 * innerHighlightOffset
                    );
                }
            }

            // Отрисовка текста внутри состояния
            using (Font font = new Font("Arial Black", 8f))
            {
                string text = GetDisplayText();
                SizeF textSize = g.MeasureString(text, font);

                // Определение позиции текста для центрирования
                float textX = stateCenter.X - textSize.Width / 2;
                float textY = stateCenter.Y - textSize.Height / 2;

                // Выбор кисти для текста
                Brush textBrush = Brushes.Black;

                g.DrawString(text, font, textBrush, textX, textY);
            }
        }

        /// <summary>
        /// Возвращает кисть для границы в зависимости от состояния.
        /// </summary>
        /// <param name="isInnerHighlighted">Состояние подсветки внутренней области.</param>
        /// <returns>Выбранная кисть.</returns>
        private Brush GetBoundaryBrush(bool isInnerHighlighted)
        {
            if (IsInput)
            {
                return isInnerHighlighted ? new SolidBrush(((dynamic)Automaton).ActiveBorderColor) : new SolidBrush(((dynamic)Automaton).ActiveBorderColor);
            }
            else
            {
                return isInnerHighlighted ? new SolidBrush(((dynamic)Automaton).HighlightedBorderColor) : new SolidBrush(((dynamic)Automaton).InactiveBorderColor);
            }
        }

        /// <summary>
        /// Отрисовывает круглую "рамку", обозначающую то, что данное состояние находится в режиме перемещения
        /// </summary>
        /// <param name="g"></param>
        public void DrawMovingFrame(Graphics g)
        {
            if (!IsMoving) return;

            DrawHelper.SetGraphicsParameters(g);

            int circleDiameter = AutomatonStatesHelper.GetCircleDiameter(Automaton);

            // Рисуем круг
            using (Pen p = new Pen(Brushes.DeepSkyBlue, 5))
                g.DrawEllipse(p, StateCenter.X - circleDiameter / 2 - 15, StateCenter.Y - circleDiameter / 2 - 15,
                    circleDiameter + 30, circleDiameter + 30);

            using (Font font = new Font("Arial", 9, FontStyle.Bold))
            {
                string label = "Перемещение...";
                SizeF labelSize = g.MeasureString(label, font);
                g.DrawString(label, font, Brushes.DeepSkyBlue, StateCenter.X - labelSize.Width / 2, StateCenter.Y - circleDiameter / 2 - 30 - labelSize.Height / 2);
            }
        }

        /// <summary>
        /// Определяет все возможные точки для начала отрисовки переходов из этого состояния.
        /// </summary>
        public void CreateTransitionStartPoints()
        {
            TransitionStartPoints = GeometryHelper.GetAbroadPoints(StateCenter, AutomatonStatesHelper.GetCircleDiameter(Automaton) / 2, 32, 15);
        }

        /// <summary>
        /// Проверяет, находится ли переданная точка во внутренней области состояния.
        /// </summary>
        /// <param name="point">Точка для проверки.</param>
        /// <returns>True, если переданная точка находится во внутренней области состояния; иначе - false.</returns>
        public bool IsInInnerArea(Point point)
        {
            double dx = point.X - StateCenter.X;
            double dy = point.Y - StateCenter.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            return distance <= AutomatonStatesHelper.GetCircleDiameter(Automaton) / 2 - 6;
        }

        /// <summary>
        /// Проверяет, находится ли переданная точка во внешней области состояния.
        /// </summary>
        /// <param name="point">Точка для проверки.</param>
        /// <returns>True, если переданная точка находится во внешней области состояния; иначе - false.</returns>
        public bool IsInBoundaryArea(Point point)
        {
            int circleDiameter = AutomatonStatesHelper.GetCircleDiameter(Automaton);

            double dx = point.X - StateCenter.X;
            double dy = point.Y - StateCenter.Y;
            double distance = Math.Sqrt(dx * dx + dy * dy);
            double innerRadius = circleDiameter / 2 - 5;
            double outerRadius = circleDiameter / 2 + 5;
            return distance >= innerRadius && distance <= outerRadius;
        }

        /// <summary>
        /// Устанавливает новую центральную точку состояния с учётом ограничений.
        /// </summary>
        /// <param name="newPosition">Новая центральная точка.</param>
        public void SetPosition(Point newPosition)
        {
            int containerWidth = AutomatonStatesHelper.Width(Automaton);
            int containerHeight = AutomatonStatesHelper.Height(Automaton);

            Point newPos = newPosition;

            // Ограничиваем новую позицию границами контейнера
            int halfDiameter = AutomatonStatesHelper.GetCircleDiameter(Automaton) / 2;
            newPos.X = Math.Max(halfDiameter + 50, Math.Min(containerWidth - halfDiameter - 50, newPosition.X));
            newPos.Y = Math.Max(halfDiameter + 50, Math.Min(containerHeight - halfDiameter - 50, newPosition.Y));

            StateCenter = newPos;
            CreateTransitionStartPoints();
        }

        #endregion





        #region Контекстное меню

        /// <summary>
        /// Заполняет и показывает контекстное меню этого состояния.
        /// </summary>
        /// <param name="location">Точка для отображения контекстного меню.</param>
        public abstract void ShowContextMenu(Point location);

        /// <summary>
        /// Добавляет новый переход.
        /// </summary>
        /// <param name="input">Входной символ, по которому совершается переход.</param>
        /// <param name="state">Состояние, в которое совершается переход.</param>
        public abstract void AddTransition(char input, AutomatonState<TAutomaton, TSelfTransition> state);

        /// <summary>
        /// Удаляет существующий переход.
        /// </summary>
        /// <param name="input">Входной символ, по которому осуществляется поиск удаляемого перехода.</param>
        protected abstract void RemoveTransition(char input);

        #endregion





        /// <summary>
        /// Получает текст, который будет отображаться во внутренней области состояния.
        /// </summary>
        /// <returns>Строка, текст которой будет отображаться во внутренней области состояния.</returns>
        protected abstract string GetDisplayText();
    }



    /// <summary>
    /// Статический класс, предоставляющий различные утилиты для работы с состояниями автомата.
    /// </summary>
    internal static class AutomatonStatesHelper
    {
        /// <summary>
        /// Получает диаметр круга для отрисовки состояния Автомата.
        /// </summary>
        /// <typeparam name="TAutomaton">Тип Автомата.</typeparam>
        /// <param name="automaton">Автомат.</param>
        /// <returns>Диаметр круга для отрисовки состояния Автомата.</returns>
        /// <exception cref="NotSupportedException">Выбрасывается в случае неподдерживаемого типа Автомата.</exception>
        public static int GetCircleDiameter<TAutomaton>(TAutomaton automaton)
        {
            if (typeof(TAutomaton) == typeof(DFMealyAutomaton))
            {
                return (automaton as DFMealyAutomaton).CircleDiameter;
            }
            else if (typeof(TAutomaton) == typeof(DFMooreAutomaton))
            {
                return (automaton as DFMooreAutomaton).CircleDiameter;
            }
            else
            {
                throw new NotSupportedException("Неподдерживаемый тип Автомата.");
            }
        }

        /// <summary>
        /// Получает ширину границы круга для отрисовки состояния Автомата.
        /// </summary>
        /// <typeparam name="TAutomaton">Тип Автомата.</typeparam>
        /// <param name="automaton">Автомат.</param>
        /// <returns>Ширина границы круга для отрисовки состояния Автомата.</returns>
        /// <exception cref="NotSupportedException">Выбрасывается в случае неподдерживаемого типа Автомата.</exception>
        public static int GetCircleBound<TAutomaton>(TAutomaton automaton)
        {
            if (typeof(TAutomaton) == typeof(DFMealyAutomaton))
            {
                return (automaton as DFMealyAutomaton).BorderWidth;
            }
            else if (typeof(TAutomaton) == typeof(DFMooreAutomaton))
            {
                return (automaton as DFMooreAutomaton).BorderWidth;
            }
            else
            {
                throw new NotSupportedException("Unsupported Automaton type.");
            }
        }

        /// <summary>
        /// Получает ширину поля для отрисовки состояния Автомата.
        /// </summary>
        /// <typeparam name="TAutomaton">Тип Автомата.</typeparam>
        /// <param name="automaton">Автомат.</param>
        /// <returns>Ширина поля отрисовки состояния Автомата.</returns>
        /// <exception cref="NotSupportedException">Выбрасывается в случае неподдерживаемого типа Автомата.</exception>
        public static int Width<TAutomaton>(TAutomaton automaton)
        {
            if (typeof(TAutomaton) == typeof(DFMealyAutomaton))
            {
                return (automaton as DFMealyAutomaton).Container.Width;
            }
            else if (typeof(TAutomaton) == typeof(DFMooreAutomaton))
            {
                return (automaton as DFMooreAutomaton).Container.Width;
            }
            else
            {
                throw new NotSupportedException("Unsupported Automaton type.");
            }
        }

        /// <summary>
        /// Получает высоту поля для отрисовки состояния Автомата.
        /// </summary>
        /// <typeparam name="TAutomaton">Тип Автомата.</typeparam>
        /// <param name="automaton">Автомат.</param>
        /// <returns>Высота поля отрисовки состояния Автомата.</returns>
        /// <exception cref="NotSupportedException">Выбрасывается в случае неподдерживаемого типа Автомата.</exception>
        public static int Height<TAutomaton>(TAutomaton automaton)
        {
            if (typeof(TAutomaton) == typeof(DFMealyAutomaton))
            {
                return (automaton as DFMealyAutomaton).Container.Height;
            }
            else if (typeof(TAutomaton) == typeof(DFMooreAutomaton))
            {
                return (automaton as DFMooreAutomaton).Container.Height;
            }
            else
            {
                throw new NotSupportedException("Unsupported Automaton type.");
            }
        }

        public static void OnTransitionsChanged<TAutomaton>(TAutomaton automaton) where TAutomaton : class
        {
            if (typeof(TAutomaton) == typeof(DFMealyAutomaton))
            {
                (automaton as DFMealyAutomaton).OnTransitionsChanged();
            }
            else if (typeof(TAutomaton) == typeof(DFMooreAutomaton))
            {
                (automaton as DFMooreAutomaton).OnTransitionsChanged();
            }
            else
            {
                throw new NotSupportedException("Unsupported Automaton type.");
            }
        }
    }
}