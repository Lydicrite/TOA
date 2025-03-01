using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheoryOfAutomatons.Automaton.Common;
using TheoryOfAutomatons.Utils.Containers;

namespace TheoryOfAutomatons.Utils.Helpers
{
    internal class GeometryHelper
    {
        /// <summary>
        /// Ограничивает переданное значение, возвращая минимальное, если оно меньше, или максимальное, если оно больше, в противном случае возвращает само значение.
        /// </summary>
        /// <typeparam name="T">Тип значения, который должен реализовывать интерфейс <see cref="IComparable{T}"/>.</typeparam>
        /// <param name="value">Значение, которое нужно ограничить.</param>
        /// <param name="min">Минимально допустимое значение.</param>
        /// <param name="max">Максимально допустимое значение.</param>
        /// <returns>Ограниченное значение.</returns>
        public static T Clamp<T>(T value, T min, T max) where T : IComparable<T>
        {
            if (value.CompareTo(min) < 0)
            {
                return min;
            }
            else if (value.CompareTo(max) > 0)
            {
                return max;
            }
            return value;
        }

        /// <summary>
        /// Получает список точек, находящихся за пределами заданного радиуса окружности, описанной вокруг центра.
        /// </summary>
        /// <param name="center">Центр окружности, вокруг которой будут генерироваться точки.</param>
        /// <param name="r">Радиус окружности.</param>
        /// <param name="n">Количество точек, которые будут сгенерированы (по умолчанию 8).</param>
        /// <param name="abroadPathLength">Длина пути за пределами, добавляемая к каждой точке (по умолчанию 1).</param>
        /// <returns>Массив списков точек; первый список содержит точки за пределами, а второй список - границы пространства.</returns>
        public static List<Point>[] GetAbroadSpacePoints(Point center, int r, int n = 8, int abroadPathLength = 1)
        {
            List<Point> abroadPoints = GetAbroadPoints(center, r, n, abroadPathLength);
            List<Point> abroadSpacePoints = new List<Point>();

            if (abroadPoints == null || abroadPoints.Count == 0)
                return new[] { new List<Point>(), new List<Point>() };

            int minX = abroadPoints.Min(p => p.X);
            int maxX = abroadPoints.Max(p => p.X);
            int minY = abroadPoints.Min(p => p.Y);
            int maxY = abroadPoints.Max(p => p.Y);

            abroadSpacePoints.Add(new Point(minX, minY));
            abroadSpacePoints.Add(new Point(maxX, minY));
            abroadSpacePoints.Add(new Point(maxX, maxY));
            abroadSpacePoints.Add(new Point(minX, maxY));

            return new[] { abroadPoints, abroadSpacePoints };
        }

        /// <summary>
        /// Генерирует список точек, расположенных на круге заданного радиуса вокруг указанного центра.
        /// </summary>
        /// <param name="center">Центр окружности, вокруг которой будут генерироваться точки.</param>
        /// <param name="r">Радиус окружности.</param>
        /// <param name="n">Количество точек, которые будут сгенерированы (по умолчанию 8).</param>
        /// <param name="delta">Смещение, добавляемое к каждой точке (по умолчанию 1).</param>
        /// <returns>Список точек, расположенных на окружности заданного радиуса.</returns>
        public static List<Point> GetAbroadPoints(Point center, int r, int n = 8, int delta = 1)
        {
            List<Point> points = new List<Point>();
            double angleStep = Math.PI / (n / 2);

            for (int i = 0; i < n; i++)
            {
                double angle = i * angleStep;
                int x = center.X + (int)Math.Round(r * Math.Cos(angle));
                int y = center.Y + (int)Math.Round(r * Math.Sin(angle));

                var qAddition = GetQuadrantAddition(center, new Point(x, y), delta);
                points.Add(new Point(x + qAddition[0], y + qAddition[1]));
            }

            return points;
        }

        /// <summary>
        /// Вычисляет смещения по координатам x и y для точек, находящихся в разных квадрантах относительно центра.
        /// </summary>
        /// <param name="a">Первая точка (центр).</param>
        /// <param name="b">Вторая точка.</param>
        /// <param name="addition">Смещение, которое будет применяться к координатам.</param>
        /// <param name="zeroXAdd">Смещение по координате X, когда разница между координатами X точек a и b равна нулю (по умолчанию 0).</param>
        /// <param name="zeroYAdd">Смещение по координате Y, когда разница между координатами Y точек a и b равна нулю (по умолчанию 0).</param>
        /// <returns>Массив, содержащий добавление по x и y.</returns>
        public static int[] GetQuadrantAddition(Point a, Point b, int addition, int zeroXAdd = 0, int zeroYAdd = 0)
        {
            int[] q = new int[2];
            int deltaX = b.X - a.X;
            int deltaY = b.Y - a.Y;

            if (deltaX > 0 && deltaY > 0)
            {
                q[0] = addition;
                q[1] = addition;
            }
            else if (deltaX < 0 && deltaY > 0)
            {
                q[0] = -addition;
                q[1] = addition;
            }
            else if (deltaX < 0 && deltaY < 0)
            {
                q[0] = -addition;
                q[1] = -addition;
            }
            else if (deltaX > 0 && deltaY < 0)
            {
                q[0] = addition;
                q[1] = -addition;
            }

            else if (deltaX > 0 && deltaY == 0)
            {
                q[0] = addition;
                q[1] = zeroYAdd;
            }
            else if (deltaX < 0 && deltaY == 0)
            {
                q[0] = -addition;
                q[1] = zeroYAdd;
            }
            else if (deltaX == 0 && deltaY < 0)
            {
                q[0] = zeroXAdd;
                q[1] = -addition;
            }
            else if (deltaX == 0 && deltaY > 0)
            {
                q[0] = zeroXAdd;
                q[1] = addition;
            }

            return q;
        }

        /// <summary>
        /// Вычисляет смещения по координатам x и y для точек, находящихся в разных квадрантах относительно центра.
        /// </summary>
        /// <param name="a">Первая точка (центр).</param>
        /// <param name="b">Вторая точка.</param>
        /// <param name="xAddition">Смещение по координате X.</param>
        /// <param name="yAddition">Смещение по координате Y.</param>
        /// <param name="zeroXAdd">Смещение по координате X, когда разница между координатами X точек a и b равна нулю (по умолчанию 0).</param>
        /// <param name="zeroYAdd">Смещение по координате Y, когда разница между координатами Y точек a и b равна нулю (по умолчанию 0).</param>
        /// <returns>Массив, содержащий добавление по x и y.</returns>
        public static int[] GetQuadrantAddition(Point a, Point b, int xAddition, int yAddition, int zeroXAdd = 0, int zeroYAdd = 0)
        {
            int[] q = new int[2];
            int deltaX = b.X - a.X;
            int deltaY = b.Y - a.Y;

            if (deltaX > 0 && deltaY > 0)
            {
                q[0] = xAddition;
                q[1] = yAddition;
            }
            else if (deltaX < 0 && deltaY > 0)
            {
                q[0] = -xAddition;
                q[1] = yAddition;
            }
            else if (deltaX < 0 && deltaY < 0)
            {
                q[0] = -xAddition;
                q[1] = -yAddition;
            }
            else if (deltaX > 0 && deltaY < 0)
            {
                q[0] = xAddition;
                q[1] = -yAddition;
            }

            else if (deltaX > 0 && deltaY == 0)
            {
                q[0] = xAddition;
                q[1] = zeroYAdd;
            }
            else if (deltaX < 0 && deltaY == 0)
            {
                q[0] = -xAddition;
                q[1] = zeroYAdd;
            }
            else if (deltaX == 0 && deltaY < 0)
            {
                q[0] = zeroXAdd;
                q[1] = -yAddition;
            }
            else if (deltaX == 0 && deltaY > 0)
            {
                q[0] = zeroXAdd;
                q[1] = yAddition;
            }

            return q;
        }

        /// <summary>
        /// Адаптирует размеры формы так, чтобы она могла отрисовать Автомат.
        /// </summary>
        /// <param name="automaton">Автомат.</param>
        /// <param name="form">Форма.</param>
        public static void AdaptContainerSize<TState>(DFAutomaton<TState> automaton, Form form) where TState : class
        {
            if (form == null)
                return;

            form.MinimumSize = new Size(880, 1080);
            form.Size = form.MinimumSize;

            int formWidth = form.Size.Width;
            int formHeight = form.Size.Height;
            form.AutoSizeMode = AutoSizeMode.GrowAndShrink;

            int maxX = automaton.StatesAlphabet.Max(state => ((dynamic)state).StateCenter.X);
            var maxY = automaton.StatesAlphabet.Max(state => ((dynamic)state).StateCenter.Y);
            int 
                width = (int)Math.Max((double)((formWidth - 555) + maxX + 2 * automaton.CircleDiameter), (double)formWidth), 
                height = (int)Math.Max((double)((formHeight - 663) + maxY + 2 * automaton.CircleDiameter), (double)formHeight);

            form.MinimumSize = new Size(width, height);
            form.Size = form.MinimumSize;
            automaton.FillMaps();
            automaton.OnTransitionsChanged();

            form.AutoSizeMode = AutoSizeMode.GrowOnly;
        }





        #region Алгоритм поиска пути A*

        /// <summary>
        /// Находит кратчайший путь от начальной точки до конечной точки с использованием алгоритма A*.
        /// </summary>
        /// <param name="start">Точка начала поиска.</param>
        /// <param name="end">Точка окончания поиска.</param>
        /// <param name="freeSpace">Массив, представляющий свободные и занятые ячейки пространства.
        /// Значение <c>true</c> указывает, что ячейка свободна, <c>false</c> - занята.</param>
        /// <returns>Список точек, представляющих найденный путь от <paramref name="start"/> до <paramref name="end"/>.
        /// Если путь не найден, возвращается пустой список.</returns>
        public static List<Point> AStarPathfinding(Point start, Point end, bool[,] freeSpace)
        {
            int width = freeSpace.GetLength(0);
            int height = freeSpace.GetLength(1);

            var openSet = new Containers.PriorityQueue<Point, double>();
            var openSetHash = new HashSet<Point>();
            openSet.Enqueue(start, 0);
            openSetHash.Add(start);

            var cameFrom = new Dictionary<Point, Point>();

            var gScore = new double[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    gScore[x, y] = double.PositiveInfinity;
            gScore[start.X, start.Y] = 0;

            var fScore = new double[width, height];
            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                    fScore[x, y] = double.PositiveInfinity;
            fScore[start.X, start.Y] = Heuristic(start, end);

            int[] dx = { -1, 0, 1, -1, 1, -1, 0, 1 };
            int[] dy = { -1, -1, -1, 0, 0, 1, 1, 1 };

            var closedSet = new HashSet<Point>();

            while (openSet.Count > 0)
            {
                Point current = openSet.Dequeue();
                openSetHash.Remove(current);

                if (current.Equals(end))
                {
                    return ReconstructPath(cameFrom, current);
                }

                closedSet.Add(current);

                for (int i = 0; i < dx.Length; i++)
                {
                    int neighborX = current.X + dx[i];
                    int neighborY = current.Y + dy[i];
                    Point neighbor = new Point(neighborX, neighborY);

                    if (neighborX < 0 || neighborX >= width || neighborY < 0 || neighborY >= height)
                        continue;
                    if (!freeSpace[neighborX, neighborY])
                        continue;
                    if (closedSet.Contains(neighbor))
                        continue;

                    double tentative_gScore = gScore[current.X, current.Y] + Distance(current, neighbor);

                    if (tentative_gScore < gScore[neighbor.X, neighbor.Y])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor.X, neighbor.Y] = tentative_gScore;
                        fScore[neighbor.X, neighbor.Y] = tentative_gScore + Heuristic(neighbor, end);

                        if (!openSetHash.Contains(neighbor))
                        {
                            openSet.Enqueue(neighbor, fScore[neighbor.X, neighbor.Y]);
                            openSetHash.Add(neighbor);
                        }
                    }
                }
            }
            return new List<Point>();
        }

        /// <summary>
        /// Восстанавливает путь от конечной точки к начальной на основе словаря предшествующих точек.
        /// </summary>
        /// <param name="cameFrom">Словарь, в котором хранятся предшествующие точки.
        /// Ключ - текущее местоположение, значение - предшествующая точка.</param>
        /// <param name="current">Текущая точка для восстановления пути.</param>
        /// <returns>Список точек, представляющий восстановленный путь.</returns>
        private static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point current)
        {
            var totalPath = new List<Point> { current };
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Insert(0, current);
            }
            return totalPath;
        }

        /// <summary>
        /// Вычисляет эвристическое расстояние между двумя точками.
        /// Используется для оценки стоимости пути в алгоритме A*.
        /// </summary>
        /// <param name="a">Первая точка.</param>
        /// <param name="b">Вторая точка.</param>
        /// <returns>Эвристическое расстояние между точками <paramref name="a"/> и <paramref name="b"/>.</returns>
        private static double Heuristic(Point a, Point b)
        {
            return Math.Max(Math.Abs(a.X - b.X), Math.Abs(a.Y - b.Y));
        }

        /// <summary>
        /// Вычисляет расстояние между двумя точками, с учетом диагонального перемещения.
        /// </summary>
        /// <param name="a">Первая точка.</param>
        /// <param name="b">Вторая точка.</param>
        /// <returns>Расстояние между точками <paramref name="a"/> и <paramref name="b"/>.</returns>
        private static double Distance(Point a, Point b)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            if (dx == 1 && dy == 1)
                return Math.Sqrt(2); // Диагональное движение
            else
                return 1; // Вертикальное или горизонтальное движение
        }

        #endregion
    }
}
