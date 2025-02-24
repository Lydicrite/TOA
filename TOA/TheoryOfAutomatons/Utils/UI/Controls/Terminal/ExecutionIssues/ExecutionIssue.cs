using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace TheoryOfAutomatons.Utils.UI.Controls.Terminal.ExecutionIssues
{
    [Flags]
    internal enum ExecutionIssueType
    {
        None = 0,
        Warning = 1,
        Error = 2,
        Information = 4,
        All = Warning | Error | Information
    }

    internal class ExecutionIssue
    {
        public DateTime Timestamp { get; }
        public ExecutionIssueType Type { get; }
        public string Source { get; }
        public string Message { get; }
        public Exception Exception { get; }

        public ExecutionIssue(ExecutionIssueType type, string source, string message, Exception ex = null)
        {
            Timestamp = DateTime.Now;
            Type = type;
            Source = source;
            Message = message;
            Exception = ex;
        }
    }




    internal static class ExecutionIssuesHelper
    {
        private static Size _iconSize = new Size(20, 20);
        private static Dictionary<ExecutionIssueType, Image> _iconCache = new Dictionary<ExecutionIssueType, Image>();

        public static Image GetCachedIcon(this ExecutionIssueType type)
        {
            if (!_iconCache.ContainsKey(type))
            {
                _iconCache[type] = GetIcon(type);
            }
            return _iconCache[type];
        }

        public static int ToIndex(this ExecutionIssueType type)
        {
            switch (type)
            {
                case ExecutionIssueType.Warning: return 0;
                case ExecutionIssueType.Error: return 1;
                case ExecutionIssueType.Information: return 2;
                default: return 2;
            }
        }

        public static Color ToColor(this ExecutionIssueType type)
        {
            switch (type)
            {
                case ExecutionIssueType.Warning: return Color.DarkOrange;
                case ExecutionIssueType.Error: return Color.Red;
                case ExecutionIssueType.Information: return Color.DodgerBlue;
                default: return Color.DodgerBlue;
            }
        }

        public static string GetTypeName(this ExecutionIssueType type)
        {
            switch (type)
            {
                case ExecutionIssueType.Warning: return "Предупреждения";
                case ExecutionIssueType.Error: return "Ошибки";
                case ExecutionIssueType.Information: return "Сообщения";
                default: throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private static Image GetIcon(this ExecutionIssueType type)
        {
            switch (type)
            {
                case ExecutionIssueType.Error: return GetResizedIcon(SystemIcons.Error, _iconSize);
                case ExecutionIssueType.Warning: return GetResizedIcon(SystemIcons.Warning, _iconSize);
                case ExecutionIssueType.Information: return GetResizedIcon(SystemIcons.Information, _iconSize);
                default: return null;
            }
        }

        private static Image GetResizedIcon(Icon icon, Size size)
        {
            if (icon == null) return null;
            Bitmap resized = new Bitmap(size.Width, size.Height);
            using (Graphics g = Graphics.FromImage(resized))
            {
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(icon.ToBitmap(), new Rectangle(0, 0, size.Width, size.Height));
            }
            return resized;
        }
    }

    internal static class ExceptionGenerator
    {
        private static readonly Random _random = new Random();
        private static readonly List<Func<Exception>> _exceptionFactories = new List<Func<Exception>>
        {
            () => new ArgumentException("Неверное значение аргумента"),
            () => new InvalidOperationException("Недопустимая операция"),
            () => new DivideByZeroException("Попытка деления на ноль"),
            () => new IndexOutOfRangeException("Индекс за пределами массива"),
            () => new FileNotFoundException("Файл не найден"),
            () => new TimeoutException("Превышено время ожидания"),
            () => new FormatException("Неверный формат данных"),
            () => new NotSupportedException("Данная функциональность не поддерживается"),
            () => new UnauthorizedAccessException("Отказано в доступе"),
            () => new ArgumentOutOfRangeException("Параметр вне допустимого диапазона"),
            () => new DirectoryNotFoundException("Каталог не найден"),
            () => new IOException("Ошибка ввода-вывода"),
            () => new ArithmeticException("Арифметическая ошибка")
        };

        public static void ThrowRandomException()
        {
            int index = _random.Next(_exceptionFactories.Count);
            throw _exceptionFactories[index]();
        }
    }
}