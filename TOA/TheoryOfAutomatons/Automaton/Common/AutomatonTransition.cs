using System;
using System.Collections.Generic;
using System.Drawing;
using TheoryOfAutomatons.Automaton.MealyAutomaton;
using TheoryOfAutomatons.Automaton.MooreAutomaton;
using TheoryOfAutomatons.Utils;
using TheoryOfAutomatons.Utils.Helpers;
using TOA.TheoryOfAutomatons.Automaton.Common;

namespace TheoryOfAutomatons.Automaton.Common
{
    /// <summary>
    /// Абстрактный класс, представляющий переход автомата.
    /// </summary>
    /// <typeparam name="TState">Тип состояния автомата.</typeparam>
    internal abstract class AutomatonTransition : IAutomatonTransition
    {
        /// <summary>
        /// Состояние, из которого происходит переход.
        /// </summary>
        public IAutomatonState From { get; protected set; }

        /// <summary>
        /// Состояние, в которое происходит переход.
        /// </summary>
        public IAutomatonState To { get; protected set; }

        /// <summary>
        /// Аннотация, связанная с переходом.
        /// </summary>
        public string Annotation { get; protected set; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="AutomatonTransition{TState}"/>.
        /// </summary>
        /// <param name="from">Начальное состояние перехода.</param>
        /// <param name="to">Конечное состояние перехода.</param>
        protected AutomatonTransition(IAutomatonState from, IAutomatonState to)
        {
            From = from;
            To = to;
        }

        /// <summary>
        /// Создает аннотацию для перехода.
        /// </summary>
        public abstract void CreateAnnotation();

        /// <summary>
        /// Отрисовывает переход.
        /// </summary>
        /// <param name="p">Карандаш для рисования.</param>
        /// <param name="g">Графический контекст для рисования.</param>
        /// <param name="space">Двумерный массив, представляющий карту пространства для отрисовки пути перехода.</param>
        /// <param name="needRecalculate">Указывает, нужно ли пересчитать путь перехода перед отрисовкой.</param>
        public virtual void DrawTransition(Pen p, Graphics g, bool[,] space, bool needRecalculate)
        {
            DrawHelper.SetGraphicsParameters(g);
            CreateAnnotation();
        }

        /// <summary>
        /// Создает новый переход между двумя состояниями.
        /// </summary>
        /// <param name="from">Начальное состояние.</param>
        /// <param name="to">Конечное состояние.</param>
        /// <param name="inputSymbol">Входной символ, связанный с переходом.</param>
        /// <returns>Созданный переход.</returns>
        public static AutomatonTransition CreateTransition(IAutomatonState from, IAutomatonState to, char inputSymbol)
        {
            if (from == to)
            {
                return CreateSelfTransition(from);
            }
            else
            {
                return CreateFromToTransition(from, to, inputSymbol);
            }
        }

        /// <summary>
        /// Создает переход от одного состояния к другому с использованием входного символа.
        /// </summary>
        /// <param name="from">Начальное состояние.</param>
        /// <param name="to">Конечное состояние.</param>
        /// <param name="inputSymbol">Входной символ.</param>
        /// <returns>Созданный переход между состояниями.</returns>
        protected static AutomatonTransition CreateFromToTransition(IAutomatonState from, IAutomatonState to, char inputSymbol)
        {
            if (typeof(TState) == typeof(MealyAutomatonState))
            {
                return new MealyFromToTransition(from as MealyAutomatonState, to as MealyAutomatonState, inputSymbol) as AutomatonTransition;
            }
            else if (typeof(TState) == typeof(MooreAutomatonState))
            {
                return new MooreFromToTransition(from as MooreAutomatonState, to as MooreAutomatonState, inputSymbol) as AutomatonTransition;
            }
            else
            {
                throw new NotSupportedException("Неподдерживаемый тип состояния Автомата TState.");
            }
        }

        /// <summary>
        /// Создает самопереход для указанного состояния.
        /// </summary>
        /// <param name="state">Состояние, для которого нужно создать самопереход.</param>
        /// <returns>Созданный самопереход.</returns>
        protected static AutomatonTransition CreateSelfTransition(IAutomatonState state)
        {
            if (typeof(TState) == typeof(MealyAutomatonState))
            {
                return new MealySelfTransition(state as MealyAutomatonState) as AutomatonTransition;
            }
            else if (typeof(TState) == typeof(MooreAutomatonState))
            {
                return new MooreSelfTransition(state as MooreAutomatonState) as AutomatonTransition;
            }
            else
            {
                throw new NotSupportedException("Неподдерживаемый тип состояния Автомата TState.");
            }
        }

        /// <summary>
        /// Вычисляет новое уменьшенное количество точек для пути.
        /// </summary>
        /// <param name="pointsCount">Количество точек в изначальном пути.</param>
        /// <returns>Новое количество точек.</returns>
        protected int CalculateNewPointCount(int pointsCount)
        {
            if (pointsCount <= 4) return pointsCount;
            if (pointsCount <= 6) return 4;
            if (pointsCount <= 9) return 7;
            if (pointsCount <= 12) return 10;
            if (pointsCount <= 15) return 13;
            if (pointsCount <= 18) return 16;
            if (pointsCount <= 21) return 19;
            if (pointsCount <= 24) return 22;
            if (pointsCount <= 27) return 25;
            if (pointsCount <= 30) return 28;
            if (pointsCount <= 33) return 31;
            return 34;
        }
    }





    /// <summary>
    /// Утилитарный класс для работы с переходами автомата.
    /// </summary>
    internal static class TransitionHelper
    {
        /// <summary>
        /// Определяет начальные и конечные точки перехода на карте.
        /// </summary>
        /// <typeparam name="TState">Тип состояния автомата.</typeparam>
        /// <param name="transition">Переход, для которого нужно определить точки.</param>
        /// <param name="map">Двумерный массив, представляющий карту пространства.</param>
        /// <param name="getTransitionStartPoints">Функция, которая возвращает список начальных точек перехода для указанного состояния.</param>
        /// <exception cref="ArgumentNullException">Выбрасывается, если передан null переход.</exception>
        /// <exception cref="InvalidOperationException">Выбрасывается, если не были инициализированы начальные или конечные точки перехода.</exception>
        public static void DetermineStartAndEnd(AutomatonTransition transition, bool[,] map, Func<IAutomatonState, List<Point>> getTransitionStartPoints) 
        {
            if (transition == null)
                throw new ArgumentNullException(nameof(transition));

            var fromPoints = getTransitionStartPoints(transition.From);
            var toPoints = getTransitionStartPoints(transition.To);

            if (fromPoints == null || toPoints == null)
                throw new InvalidOperationException("Поле \"TransitionStartPoints\" у From или To не инициализировано.");

            double minDistance = double.MaxValue;
            Point optimalStart = new Point();
            Point optimalEnd = new Point();
            bool found = false;

            foreach (var startPoint in fromPoints)
            {
                if (!IsPointValid(startPoint, map))
                    continue;

                foreach (var endPoint in toPoints)
                {
                    if (!IsPointValid(endPoint, map))
                        continue;

                    double distance = CalculateEuclideanDistance(startPoint, endPoint);

                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        optimalStart = startPoint;
                        optimalEnd = endPoint;
                        found = true;
                    }
                }
            }

            if (found)
            {
                transition.SetStartAndEnd(optimalStart, optimalEnd);
            }
            else
            {
                throw new InvalidOperationException("Не найдено подходящих точек, соответствующих карте.");
            }
        }

        /// <summary>
        /// Проверяет, является ли указанная точка допустимой на карте.
        /// </summary>
        /// <param name="p">Точка для проверки.</param>
        /// <param name="map">Карта пространства.</param>
        /// <returns>true, если точка допустима, иначе - false.</returns>
        private static bool IsPointValid(Point p, bool[,] map)
        {
            int width = map.GetLength(0);
            int height = map.GetLength(1);

            if (p.X < 0 || p.X >= width || p.Y < 0 || p.Y >= height)
                return false;

            return map[p.X, p.Y];
        }

        /// <summary>
        /// Вычисляет евклидово расстояние между двумя точками.
        /// </summary>
        /// <param name="a">Первая точка.</param>
        /// <param name="b">Вторая точка.</param>
        /// <returns>Евклидово расстояние между двумя точками.</returns>
        private static double CalculateEuclideanDistance(Point a, Point b)
        {
            int deltaX = a.X - b.X;
            int deltaY = a.Y - b.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        /// <summary>
        /// Равномерно уменьшает количество точек в начальном пути до указанного количества.
        /// </summary>
        /// <param name="initialPath">Начальный список точек, который необходимо уменьшить.</param>
        /// <param name="n">Желаемое количество точек после уменьшения.</param>
        public static void ReducePath(List<Point> initialPath, int n)
        {
            if (initialPath.Count > n)
            {
                if (n <= 0)
                {
                    initialPath.Clear();
                    return;
                }

                int m = initialPath.Count;
                double step = (double)(m - 1) / (n - 1);
                List<Point> reducedPath = new List<Point>(n);

                for (int i = 0; i < n; i++)
                {
                    double currentIndex = i * step;
                    int index = (int)Math.Round(currentIndex);
                    index = Math.Min(index, m - 1);
                    reducedPath.Add(initialPath[index]);
                }

                initialPath.Clear();
                initialPath.AddRange(reducedPath);
            }
        }
    }





    /// <summary>
    /// Расширения для работы с переходами автомата.
    /// </summary>
    internal static class TransitionExtensions
    {
        /// <summary>
        /// Устанавливает начальную и конечную точки для перехода.
        /// </summary>
        /// <typeparam name="TState">Тип состояния автомата.</typeparam>
        /// <param name="transition">Переход, для которого нужно установить точки.</param>
        /// <param name="start">Начальная точка.</param>
        /// <param name="end">Конечная точка.</param>
        /// <exception cref="NotSupportedException">Выбрасывается, если тип перехода не поддерживается.</exception>
        public static void SetStartAndEnd(this AutomatonTransition transition, Point start, Point end)
        {
            if (transition is MealyFromToTransition fromToMealy)
            {
                fromToMealy.Start = start;
                fromToMealy.End = end;
            }
            else if (transition is MooreFromToTransition fromToMoore)
            {
                fromToMoore.Start = start;
                fromToMoore.End = end;
            }
            else
            {
                throw new NotSupportedException("Неподдерживаемый тип перехода.");
            }
        }
    }
}