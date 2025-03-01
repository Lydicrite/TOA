using System;

namespace TheoryOfAutomatons.Utils.UI.Controls.Terminal.Shell
{
    public class Command
    {
        public string Name { get; }
        public string Description { get; }
        public Action<CommandArgs> Action { get; }

        public Command(string name, string description, Action<CommandArgs> action)
        {
            Name = name;
            Description = description;
            Action = action;
        }
    }

    public class CommandResult
    {
        public bool Success { get; }
        public string Message { get; }
        public TimeSpan ExecutionTime { get; }

        public CommandResult(bool success, string message, TimeSpan executionTime)
        {
            Success = success;
            Message = message;
            ExecutionTime = executionTime;
        }

        public override string ToString() => $"{Message}";

        public string ExecutionTimeToString()
        {
            return $"[execution time: {FormatExecutionTime(ExecutionTime)}]";
        }

        private static string FormatExecutionTime(TimeSpan duration)
        {
            double totalNanoseconds = duration.TotalMilliseconds * 1_000_000;

            if (totalNanoseconds < 1_000)
            {
                return $"{totalNanoseconds:F0} ns";
            }
            else if (totalNanoseconds < 1_000_000)
            {
                return $"{(totalNanoseconds / 1_000):F2} μs";
            }
            else if (totalNanoseconds < 1_000_000_000)
            {
                return $"{duration.TotalMilliseconds:F2} ms";
            }
            else if (totalNanoseconds < 60_000_000_000)
            {
                return $"{duration.TotalSeconds:F2} s";
            }
            else
            {
                return $"{duration.TotalMinutes:F2} min";
            }
        }
    }
}
