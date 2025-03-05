using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security;
using System.Text;
using System.Threading;

namespace TheoryOfAutomatons.Utils.UI.Controls.Terminal.ExecutionIssues
{
    [Flags]
    public enum ExecutionIssueType
    {
        None = 1,
        Warning = 2,
        Error = 4,
        Message = 8,
        All = Warning | Error | Message
    }

    [Flags]
    public enum ExecutionIssueCategory
    {
        IO = 1,
        UI = 2,
        Algorithmic = 4,
        Security = 8,
        Data = 16
    }

    [DataContract]
    public class ExecutionIssue
    {
        #region Свойства ошибки

        #region Базовые свойства

        [DataMember(Order = 1)]
        public Guid Id { get; } = Guid.NewGuid();

        [DataMember(Order = 2)]
        public DateTime Timestamp { get; }

        [DataMember(Order = 3)]
        public ExecutionIssueType Type { get; }

        [DataMember(Order = 4)]
        public ExecutionIssueCategory Category { get; }

        [DataMember(Order = 5)]
        public string Source { get; }

        [DataMember(Order = 6)]
        public string Message { get; }

        [DataMember(Order = 7)]
        public string StackTrace { get; }

        [DataMember(Order = 8)]
        public string TargetSite { get; }

        [DataMember(Order = 9)]
        public string HelpLink { get; }

        [DataMember(Order = 10)]
        public string ErrorCode { get; }

        [IgnoreDataMember]
        public Exception Exception { get; }

        #endregion

        #region Контекст выполнения

        [DataMember(Order = 11)]
        public string MachineName { get; } = Environment.MachineName;

        [DataMember(Order = 12)]
        public string OSVersion { get; } = Environment.OSVersion.VersionString;

        [DataMember(Order = 13)]
        public string AppVersion { get; } = Assembly.GetEntryAssembly()?.GetName().Version.ToString();

        [DataMember(Order = 14)]
        public string UserName { get; } = Environment.UserName;

        #endregion

        #region Детализация

        [DataMember(Order = 15)]
        public List<ExecutionIssue> InnerIssues { get; } = new List<ExecutionIssue>();

        [DataMember(Order = 16)]
        public string CodeSnippet { get; }

        [DataMember(Order = 17)]
        public string MethodPath { get; }

        #endregion

        #endregion

        public ExecutionIssue
        (
            ExecutionIssueType type,
            ExecutionIssueCategory category,
            string source,
            string message,
            Exception ex = null,
            string codeSnippet = null
        )
        {
            Timestamp = DateTime.UtcNow;
            Type = type;
            Category = category;
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Message = message ?? throw new ArgumentNullException(nameof(message));
            Exception = ex;
            StackTrace = ex?.StackTrace;
            MethodPath = GetMethodPath(ex);
            TargetSite = ex?.TargetSite?.ToString();
            HelpLink = ex?.HelpLink;
            ErrorCode = GenerateErrorCode(ex);
            CodeSnippet = GetCodeSnippet(ex);
        }

        private string GetCodeSnippet(Exception ex)
        {
            try
            {
                var stackFrame = new StackTrace(ex, true).GetFrame(0);
                if (stackFrame == null) return null;

                var fileName = stackFrame.GetFileName();
                var lineNumber = stackFrame.GetFileLineNumber();

                return File.Exists(fileName)
                    ? string.Join("\n", File.ReadLines(fileName)
                        .Skip(lineNumber - 15)
                        .Take(30))
                    : null;
            }
            catch { return null; }
        }

        private string GetMethodPath(Exception ex)
        {
            if (ex == null) return null;

            try
            {
                var stackFrame = new StackTrace(ex, true)
                    .GetFrames()?
                    .FirstOrDefault(f => f.GetFileLineNumber() > 0);

                return stackFrame != null
                    ? $"{stackFrame.GetMethod()?.DeclaringType?.FullName}.{stackFrame.GetMethod()?.Name}:{stackFrame.GetFileLineNumber()}"
                    : null;
            }
            catch (Exception)
            {
                return "Не удалость получить часть стека";
            }
        }

        private string GenerateErrorCode(Exception ex)
        {
            if (ex != null)
            {
                int hash = 17;
                hash = hash * 31 + ex.GetType().Name.GetHashCode(StringComparison.Ordinal);
                hash = hash * 31 + (Message?.GetHashCode(StringComparison.Ordinal) ?? 0);
                hash = hash * 31 + (Source?.GetHashCode(StringComparison.Ordinal) ?? 0);

                return $"ISS-{Math.Abs(hash):X8}";
            } 
            else
            {
                int hash = 17;
                hash = hash * 31 + Type.GetHashCode();
                hash = hash * 31 + (Message?.GetHashCode(StringComparison.Ordinal) ?? 0);
                hash = hash * 31 + (Source?.GetHashCode(StringComparison.Ordinal) ?? 0);

                return $"ISS-{Math.Abs(hash):X8}";
            }
        }

        public string GetTechnicalDetails()
        {
            const string separator = "--------------------------------------------------";
            const string indent = "\n\t";
            var details = new StringBuilder();

            void AppendSection(string title, object value)
                => details.Append($"{indent}[{title}] {value}");

            void AppendMultilineSection(string title, string content)
            {
                if (string.IsNullOrEmpty(content)) return;

                details.Append($"{indent}[{title}]{indent}{separator}")
                       .Append($"\n\n{indent}{content}")
                       .Append($"\n\n{indent}{separator}");
            }

            // Основная информация
            AppendSection("Тип", Type);
            AppendSection("Категория", Category);
            AppendSection("Время", Timestamp.ToLocalTime().ToString("G"));

            // Опциональные поля
            if (!string.IsNullOrEmpty(Source)) AppendSection("Источник", Source);
            if (!string.IsNullOrEmpty(ErrorCode)) AppendSection("Код", ErrorCode);
            if (!string.IsNullOrEmpty(Message)) AppendSection("Сообщение", Message);

            // Многострочные секции
            AppendMultilineSection("Стек вызовов", StackTrace);
            AppendMultilineSection("Фрагмент кода", CodeSnippet);

            return details.ToString();
        }

        public void AddInnerIssue(ExecutionIssue issue) => InnerIssues.Add(issue);

        public void Dispose() => InnerIssues.Clear();
    }




    public static class ExecutionIssuesHelper
    {
        private static readonly Size _iconSize = new Size(20, 20);
        private static readonly ReaderWriterLockSlim _cacheLock = new ReaderWriterLockSlim();
        private static readonly Dictionary<ExecutionIssueType, Image> _iconCache = new Dictionary<ExecutionIssueType, Image>();

        public static Image GetCachedIcon(this ExecutionIssueType type)
        {
            _cacheLock.EnterReadLock();
            try
            {
                if (_iconCache.TryGetValue(type, out var cachedIcon))
                    return cachedIcon;
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }

            _cacheLock.EnterWriteLock();
            try
            {
                var icon = GetIcon(type);
                _iconCache[type] = icon;
                return icon;
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        public static int ToIndex(this ExecutionIssueType type) => type switch
        {
            ExecutionIssueType.Warning => 0,
            ExecutionIssueType.Error => 1,
            ExecutionIssueType.Message => 2,
            _ => 2
        };

        public static Color ToColor(this ExecutionIssueType type) => type switch
        {
            ExecutionIssueType.Warning => Color.DarkOrange,
            ExecutionIssueType.Error => Color.Red,
            ExecutionIssueType.Message => Color.DodgerBlue,
            _ => Color.DodgerBlue
        };

        public static string GetTypeName(this ExecutionIssueType type) => type switch
        {
            ExecutionIssueType.Warning => "Предупреждения",
            ExecutionIssueType.Error => "Ошибки",
            ExecutionIssueType.Message => "Сообщения",
            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };

        private static Image GetIcon(ExecutionIssueType type)
        {
            Icon icon = type switch
            {
                ExecutionIssueType.Error => SystemIcons.Error,
                ExecutionIssueType.Warning => SystemIcons.Warning,
                ExecutionIssueType.Message => SystemIcons.Information,
                _ => null
            };

            return icon != null ? GetResizedIcon(icon, _iconSize) : null;
        }

        private static Image GetResizedIcon(Icon icon, Size size)
        {
            if (icon == null) return null;

            using var srcBitmap = icon.ToBitmap();
            var destBitmap = new Bitmap(size.Width, size.Height);

            using (var g = Graphics.FromImage(destBitmap))
            {
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                g.DrawImage(srcBitmap, new Rectangle(0, 0, size.Width, size.Height));
            }
            return destBitmap;
        }

        public static void ExportToFile(IEnumerable<ExecutionIssue> issues, string path, bool includeTechnicalDetails = true)
        {
            var content = includeTechnicalDetails
                ? string.Join("\n\n", issues.Select(i => i.GetTechnicalDetails()))
                : string.Join("\n", issues.Select(i => $"[{i.Timestamp:HH:mm:ss}] {i.Type}: {i.Message}"));

            File.WriteAllText(path, content, Encoding.UTF8);
        }

        public static string ToHtmlReport(this IEnumerable<ExecutionIssue> issues, bool includeStackTrace = false)
        {
            var sb = new StringBuilder(@"<style>
                .error { color: #dc3545; } 
                .warning { color: #ffc107; } 
                .info { color: #17a2b8; }
                table { border-collapse: collapse; width: 100%; }
                td, th { border: 1px solid #ddd; padding: 8px; }
                </style>
                <table>
                <tr><th>Time</th><th>Type</th><th>Message</th></tr>");

            foreach (var issue in issues)
            {
                sb.Append($@"<tr class='{issue.Type.ToString().ToLower()}'>
                    <td>{issue.Timestamp:HH:mm:ss.fff}</td>
                    <td>{issue.Type}</td>
                    <td>{SecurityElement.Escape(issue.Message)}" +
                    (includeStackTrace ? $"<pre>{SecurityElement.Escape(issue.StackTrace)}</pre>" : "") +
                    "</td></tr>");
            }
            return sb.Append("</table>").ToString();
        }


        public static string ToFriendlyString(this ExecutionIssueType type)
        {
            return type switch
            {
                ExecutionIssueType.Error => "Ошибка",
                ExecutionIssueType.Warning => "Предупреждение",
                ExecutionIssueType.Message => "Информация",
                _ => "Неизвестно"
            };
        }

        public static string ToFriendlyString(this ExecutionIssueCategory category)
        {
            return category switch
            {
                ExecutionIssueCategory.IO => "Ввод/вывод",
                ExecutionIssueCategory.UI => "Интерфейс",
                ExecutionIssueCategory.Algorithmic => "Алгоритм",
                ExecutionIssueCategory.Security => "Безопасность",
                ExecutionIssueCategory.Data => "Данные",
                _ => "Другое"
            };
        }
    }


    public static class ExceptionGenerator
    {
        private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() =>
             new Random(Interlocked.Increment(ref _seed)));

        private static int _seed = Environment.TickCount;
        private static readonly List<Func<Exception>> _exceptionFactories = new List<Func<Exception>>
        {
            () => new ArgumentException("Invalid argument value"),
            () => new InvalidOperationException("Invalid operation"),
            () => new DivideByZeroException("Division by zero"),
            () => new IndexOutOfRangeException("Index out of range"),
            () => new FileNotFoundException("File not found"),
            () => new TimeoutException("Operation timeout"),
            () => new FormatException("Invalid data format"),
            () => new NotSupportedException("Feature not supported"),
            () => new UnauthorizedAccessException("Access denied"),
            () => new ArgumentOutOfRangeException("Parameter out of range"),
            () => new DirectoryNotFoundException("Directory not found"),
            () => new IOException("I/O error"),
            () => new SecurityException("Security violation"),
            () => new ArithmeticException("Arithmetic error")
        };

        public static void ThrowRandomException(int? seed = null)
        {
            var random = seed.HasValue
                ? new Random(seed.Value)
                : _random.Value;

            int index = random.Next(_exceptionFactories.Count);
            throw _exceptionFactories[index]();
        }

        public static void AddCustomException(Func<Exception> exceptionFactory)
        {
            if (exceptionFactory == null)
                throw new ArgumentNullException(nameof(exceptionFactory));

            _exceptionFactories.Add(exceptionFactory);
        }
    }
}