using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace TheoryOfAutomatons.Utils.Helpers
{
    internal class DrawHelper
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
        /// Устанавливает параметры графики для улучшения качества отрисовки.
        /// Включает сглаживание для рисовки и отображения текста.
        /// </summary>
        /// <param name="g">Объект <see cref="Graphics"/>, для которого устанавливаются параметры.</param>
        public static void SetGraphicsParameters(Graphics g)
        {
            if (g.SmoothingMode != System.Drawing.Drawing2D.SmoothingMode.AntiAlias)
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            if (g.TextRenderingHint != System.Drawing.Text.TextRenderingHint.AntiAlias)
                g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        }

        /// <summary>
        /// Смешивает два цвета с учетом прозрачности <paramref name="blendColor"/>.
        /// </summary>
        /// <param name="baseColor">Исходный цвет пикселя.</param>
        /// <param name="blendColor">Цвет для смешивания (может иметь прозрачность).</param>
        /// <returns>Новый смешанный цвет.</returns>
        private static Color BlendColors(Color baseColor, Color blendColor)
        {
            double alpha = blendColor.A / 255.0;

            byte r = (byte)Clamp(baseColor.R * (1 - alpha) + blendColor.R * alpha, 0, 255);
            byte g = (byte)Clamp(baseColor.G * (1 - alpha) + blendColor.G * alpha, 0, 255);
            byte b = (byte)Clamp(baseColor.B * (1 - alpha) + blendColor.B * alpha, 0, 255);
            byte a = (byte)Clamp(baseColor.A * (1 - alpha) + blendColor.A * alpha, 0, 255);

            return Color.FromArgb(a, r, g, b);
        }

        /// <summary>
        /// Получает ограничивающий прямоугольник, содержащий указанные точки.
        /// </summary>
        /// <param name="points">Список точек, для которых необходимо получить ограничивающий прямоугольник.</param>
        /// <returns>Прямоугольник, содержащий все указанные точки.</returns>
        private static Rectangle GetBoundingRectangle(List<Point> points)
        {
            int minX = points.Min(p => p.X);
            int minY = points.Min(p => p.Y);
            int maxX = points.Max(p => p.X);
            int maxY = points.Max(p => p.Y);
            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }

        /// <summary>
        /// Рисует точки на графическом объекте <paramref name="g"/>.
        /// </summary>
        /// <param name="g">Объект <see cref="Graphics"/>, на котором будут нарисованы точки.</param>
        /// <param name="points">Список точек для рисования.</param>
        /// <param name="color">Цвет точек.</param>
        /// <param name="radius">Радиус точек. По умолчанию 1.</param>
        public static void DrawPoints(Graphics g, List<Point> points, Color color, int radius = 1)
        {
            using (var brush = new SolidBrush(color))
            {
                foreach (var point in points)
                {
                    g.FillEllipse(brush, point.X - radius, point.Y - radius, radius * 2, radius * 2);
                }
            }
        }

        /// <summary>
        /// Рисует прямоугольник на графическом объекте путём изменения цвета пикселей.
        /// </summary>
        /// <param name="g">Объект <see cref="Graphics"/>, на котором будет нарисован прямоугольник.</param>
        /// <param name="bmp">Изображение, на котором будут изменены цвета пикселей.</param>
        /// <param name="map">Массив, определяющий, нужно ли изменить цвет.</param>
        /// <param name="color">Цвет, который будет использован для изменения имеющегося цвета.</param>
        public static void DrawCustomRectangle(Graphics g, Bitmap bmp, bool[,] map, Color color)
        {
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    bool preserveColor = map[x, y];

                    if (preserveColor)
                    {
                        continue;
                    }
                    else
                    {
                        Color currentColor = bmp.GetPixel(x, y);
                        Color mixedColor = BlendColors(currentColor, color);
                        bmp.SetPixel(x, y, mixedColor);
                    }
                }
            }
            g.DrawImage(bmp, 0, 0);
        }

        /// <summary>
        /// Рисует дугу с подписью на графическом объекте <paramref name="g"/>.
        /// </summary>
        /// <param name="center">Центр дуги.</param>
        /// <param name="r">Радиус дуги.</param>
        /// <param name="info">Подпись, которую нужно отобразить рядом с дугой.</param>
        /// <param name="g">Объект <see cref="Graphics"/>, на котором будет нарисована дуга и подпись.</param>
        /// <param name="pen">Карандаш, используемый для рисования дуги и текста.</param>
        public static void DrawArcWithLabel(Point center, int r, string info, Graphics g, Pen pen)
        {
            SetGraphicsParameters(g);

            float r2 = 0.35f * r;
            float startAngle = 170;
            float endAngle = 200;

            // Рисуем дугу
            g.DrawArc(pen, center.X - r2, center.Y - r2 * 2.5F, r2 * 2, r2 * 2, startAngle, endAngle);

            // Вычисляем позицию для подписи
            float angle = (startAngle + endAngle) / 2;
            float labelX = center.X + 0;
            float labelY = center.Y - r;

            // Рисуем подпись
            using (Font font = new Font("Arial", 8))
            {
                SizeF labelSize = g.MeasureString(info, font);
                g.DrawString(info, font, pen.Brush, labelX - labelSize.Width / 2, labelY - labelSize.Height);
            }
        }
    }
}
