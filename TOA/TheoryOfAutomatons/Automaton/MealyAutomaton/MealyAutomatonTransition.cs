using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using TheoryOfAutomatons.Automaton.Common;
using TheoryOfAutomatons.Utils.Helpers;

namespace TheoryOfAutomatons.Automaton.MealyAutomaton
{
    /// <summary>
    /// Класс, представляющий переход между различными состояниями Автомата Мили.
    /// </summary>
    internal class MealyFromToTransition : AutomatonTransition
    {
        /// <summary>
        /// Начальная точка перехода.
        /// </summary>
        public Point Start { get; set; }
        /// <summary>
        /// Конечная точка перехода.
        /// </summary>
        public Point End { get; set; }
        /// <summary>
        /// Путь для отрисовки перехода.
        /// </summary>
        public List<Point> Path { get; set; }
        /// <summary>
        /// Входной символ, по которому осуществляется переход.
        /// </summary>
        private readonly char InputSymbol;

        /// <summary>
        /// Представляет переход между двумя различными состояниями Автомата Мили.
        /// </summary>
        /// <param name="from">Состояние, от которого происходит переход.</param>
        /// <param name="to">Состояние, в которое происходит переход.</param>
        /// <param name="inputSymbol">Входной символ, по которому происходит переход.</param>
        /// <exception cref="ArgumentException">Выбрасывается в случае некорректного определения состояний From и To.</exception>
        public MealyFromToTransition(MealyAutomatonState from, MealyAutomatonState to, char inputSymbol)
            : base(from, to)
        {
            if (from == to)
                throw new ArgumentException("Состояния From и To должны быть различны для перехода FromToTransition.");

            InputSymbol = inputSymbol;
            Path = new List<Point>();
        }

        /// <summary>
        /// Создаёт аннотацию для перехода.
        /// </summary>
        public override void CreateAnnotation()
        {
            char output = From.Outputs.TryGetValue(InputSymbol, out char outVal) ? outVal : '\0';
            Annotation = output != '\0'
                ? $"{{{InputSymbol} / {output}}}"
                : $"{{{InputSymbol} / _}}";
        }

        /// <summary>
        /// Отрисовывает переход.
        /// </summary>
        /// <param name="p">Карандаш для рисования.</param>
        /// <param name="g">Графический контекст для рисования.</param>
        /// <param name="space">Двумерный массив, представляющий карту пространства для отрисовки пути перехода.</param>
        /// <param name="needRecalculate">Указывает, нужно ли пересчитать путь перехода перед отрисовкой.</param>
        public override void DrawTransition(Pen p, Graphics g, bool[,] space, bool needRecalculate)
        {
            base.DrawTransition(p, g, space, needRecalculate);
            DrawHelper.SetGraphicsParameters(g);
            if (needRecalculate)
            {
                TransitionHelper.DetermineStartAndEnd(this, space, state => state.TransitionStartPoints);
                CreatePath(space);
            }

            if (Path.Count > 0)
            {
                g.DrawBeziers(p, Path.ToArray());

                Point start = Path.First();
                var qa = GeometryHelper.GetQuadrantAddition(From.StateCenter, start, 20, 15, 0, 3);
                Point atp = new Point(start.X + qa[0], start.Y + qa[1]);

                using (Font f = new Font("Arial", 8f))
                {
                    SizeF size = g.MeasureString(Annotation, f);
                    g.DrawString(Annotation, f, p.Brush, atp.X, atp.Y);
                }
            }
        }

        /// <summary>
        /// Создаёт путь, по которому будет отрисовываться переход.
        /// </summary>
        /// <param name="space">Двумерный массив, представляющий карту пространства для отрисовки пути перехода.</param>
        public void CreatePath(bool[,] space)
        {
            if (space != null && From != To)
            {
                Path = GeometryHelper.AStarPathfinding(Start, End, space);

                if (From.Automaton.ProhibitIntersectingTransitions)
                    foreach (var p in Path)
                    {
                        space[p.X, p.Y] = false;
                    }

                TransitionHelper.ReducePath(Path, CalculateNewPointCount(Path.Count));
            }
        }
    }





    /// <summary>
    /// Класс, представляющий самопереход состояния Автомата Мили.
    /// </summary>
    internal class MealySelfTransition : AutomatonTransition
    {
        /// <summary>
        /// Список входных символов состояния.
        /// </summary>
        private List<char> InputsList;

        /// <summary>
        /// Представляет самопереход состояния Автомата Мура.
        /// </summary>
        /// <param name="state">Состояние с самопереходом.</param>
        /// <exception cref="ArgumentException">Выбрасывается в случае некорректного определения состояний From и To.</exception>
        public MealySelfTransition(MealyAutomatonState state)
            : base(state, state)
        {
            if (From != To)
                throw new ArgumentException("From and To states must be same for SelfTransition.");

            InputsList = new List<char>(From.Outputs.Keys);
        }

        /// <summary>
        /// Добавляет входной символ в список входных символов.
        /// </summary>
        /// <param name="input">Символ для добавления.</param>
        public void AppendInput(char input)
        {
            if (!InputsList.Contains(input))
                InputsList.Add(input);
        }

        /// <summary>
        /// Удаляет символ из списка входных символов.
        /// </summary>
        /// <param name="input">Символ для удаления.</param>
        public void RemoveInput(char input)
        {
            InputsList.Remove(input);
        }

        /// <summary>
        /// Создаёт аннотацию для перехода.
        /// </summary>
        public override void CreateAnnotation()
        {
            var annotations = InputsList.Select(input =>
            {
                if (From.Outputs.TryGetValue(input, out char output))
                {
                    return output != '\0'
                        ? $"{{{input} / {output}}}"
                        : $"{{{input} / _}}";
                }
                return $"{{{input}/}}";
            });

            Annotation = string.Join(", ", annotations);
        }

        /// <summary>
        /// Отрисовывает переход.
        /// </summary>
        /// <param name="p">Карандаш для рисования.</param>
        /// <param name="g">Графический контекст для рисования.</param>
        /// <param name="space">Двумерный массив, представляющий карту пространства для отрисовки пути перехода.</param>
        /// <param name="needRecalculate">Указывает, нужно ли пересчитать путь перехода перед отрисовкой.</param>
        public override void DrawTransition(Pen p, Graphics g, bool[,] space, bool needRecalculate)
        {
            base.DrawTransition(p, g, space, needRecalculate);
            if (Annotation.Length >= 5)
                DrawHelper.DrawArcWithLabel(From.StateCenter, From.Automaton.CircleDiameter, Annotation, g, p);
        }
    }
}