using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TheoryOfAutomatons.Automaton.Common;
using TheoryOfAutomatons.Automaton.MealyAutomaton;
using TheoryOfAutomatons.Automaton.MooreAutomaton;
using TheoryOfAutomatons.Utils.Helpers;

namespace TOA.TheoryOfAutomatons.Automaton.Common
{
    internal interface IAutomatonTransition
    {
        /// <summary>
        /// Состояние, из которого происходит переход.
        /// </summary>
        IAutomatonState From { get; }
        /// <summary>
        /// Состояние, в которое происходит переход.
        /// </summary>
        IAutomatonState To { get; }
        /// <summary>
        /// Аннотация, связанная с переходом.
        /// </summary>
        string Annotation { get; }
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
        /// Создает аннотацию для перехода.
        /// </summary>
        void CreateAnnotation();

        /// <summary>
        /// Отрисовывает переход.
        /// </summary>
        /// <param name="p">Карандаш для рисования.</param>
        /// <param name="g">Графический контекст для рисования.</param>
        /// <param name="space">Двумерный массив, представляющий карту пространства для отрисовки пути перехода.</param>
        /// <param name="needRecalculate">Указывает, нужно ли пересчитать путь перехода перед отрисовкой.</param>
        void DrawTransition(Pen p, Graphics g, bool[,] space, bool needRecalculate);

        /// <summary>
        /// Вычисляет новое уменьшенное количество точек для пути.
        /// </summary>
        /// <param name="pointsCount">Количество точек в изначальном пути.</param>
        /// <returns>Новое количество точек.</returns>
        int CalculateNewPointCount(int pointsCount);
    }
}
