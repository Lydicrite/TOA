using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TOA.TheoryOfAutomatons.Utils.UI.Controls.Terminal.Shell;

namespace TheoryOfAutomatons.Utils.UI.Controls.Terminal.Shell
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TerminalCommandAttribute : Attribute
    {
        public string Name { get; }
        public string Description { get; }
        public string Usage { get; }

        public TerminalCommandAttribute(string name, string description, string usage = "")
        {
            Name = name;
            Description = description;
            Usage = usage;
        }
    }



    public class CommandHandler
    {
        public static readonly char CommandChar = '\\';
        public Dictionary<string, MethodInfo> Commands => _commands;
        private Dictionary<string, MethodInfo> _commands = new Dictionary<string, MethodInfo>();
        public IEnumerable<string> GetCommandNames() => _commands.Keys;

        public void RegisterFromAssembly(Assembly assembly)
        {
            foreach (var method in assembly.GetTypes()
                .SelectMany(t => t.GetMethods(BindingFlags.Public | BindingFlags.Static)))
            {
                var attr = method.GetCustomAttribute<TerminalCommandAttribute>();
                if (attr != null && method.GetParameters().Length == 2)
                {
                    _commands[attr.Name.ToLower()] = method;
                }
            }
        }

        public CommandResult Execute(string input, Terminal terminal)
        {
            var stopwatch = Stopwatch.StartNew();

            var match = Regex.Match(input, @"^(\S+)(?:\s+(.*))?$");
            if (!match.Success) return new CommandResult(false, "Неверный формат команды", TimeSpan.Zero);

            var commandName = match.Groups[1].Value.ToLower();
            var parameters = Regex.Matches(match.Groups[2].Value, @"(""[^""]*""|\S+)")
                                .Cast<Match>()
                                .Select(m => m.Value.Trim('"'))
                                .ToArray();

            if (!_commands.TryGetValue(commandName, out var method))
                return new CommandResult(false, $"Команда \"{commandName}\" не найдена", TimeSpan.Zero);

            try
            {
                var result = method.Invoke(null, new object[] { parameters, terminal });
                stopwatch.Stop();

                return result as CommandResult ?? new CommandResult(true, "Команда выполнена", stopwatch.Elapsed);
            }
            catch (Exception ex)
            {
                return new CommandResult(false, ex.InnerException?.Message ?? ex.Message, stopwatch.Elapsed);
            }
        }

        private static string GetCommandDescription(string commandName, CommandHandler commandHandler)
        {
            return commandHandler.Commands[commandName].GetCustomAttribute<TerminalCommandAttribute>()?.Description ?? "No description";
        }





        public static class TerminalCommands
        {

            [TerminalCommand("help", "Показывает список доступных команд", "help [command]")]
            public static CommandResult Help(string[] args, Terminal terminal)
            {
                if (args.Length > 1)
                    return new CommandResult(false, "Эта команда может иметь только один аргумент - команду, по которой необходима помощь", TimeSpan.Zero);

                var sb = new StringBuilder();
                var commands = terminal.CommandHandler.GetCommandNames().Distinct();

                if (args.Length == 0)
                {
                    sb.AppendLine("Доступные команды: ");
                    foreach (var cmd in commands)
                    {
                        sb.AppendLine($"    {cmd.PadRight(10)} - {GetCommandDescription(cmd, terminal.CommandHandler)}");
                    }
                }

                return new CommandResult(true, sb.ToString(), TimeSpan.Zero);
            }



            [TerminalCommand("clear", "Очищает текст в PowerShell")]
            public static CommandResult Clear(string[] args, Terminal terminal)
            {
                var sw = Stopwatch.StartNew();
                if (args.Length > 0)
                    return new CommandResult(false, "Эта команда не принимает аргументов.", TimeSpan.Zero);

                terminal.pshTerminal.Clear();
                sw.Stop();
                return new CommandResult(true, "", sw.Elapsed);
            }



            [TerminalCommand("set_input_rule", "Установить правило ввода", "\\set_input_rule [rule_name]")]
            public static CommandResult SetInputRule(string[] args, Terminal terminal)
            {
                if (args.Length == 0)
                    return new CommandResult(false, "Укажите имя правила ввода", TimeSpan.Zero);

                if (args.Length > 1)
                    return new CommandResult(false, "Эта команда может иметь только один аргумент - имя правила", TimeSpan.Zero);

                if (InputRulePresets.Rules.TryGetValue(args[0], out var rule))
                {
                    terminal.SetInputRule(rule);

                    return new CommandResult
                    (
                        true,

                        $"Активирован режим: {rule.Name}\n" +
                        $"Правила: {rule.Description}\n" +
                        $"Пример: {rule.Example}",

                        TimeSpan.Zero
                    );
                } 
                else
                {
                    return new CommandResult(false, $"Правило ввода \"{args[0]}\" не найдено", TimeSpan.Zero);
                } 

            }



            [TerminalCommand("list_rules", "Показать доступные правила")]
            public static CommandResult ListRules(string[] args, Terminal terminal)
            {
                var sb = new StringBuilder("Доступные правила: \n");
                foreach (var rule in InputRulePresets.Rules)
                {
                    sb.AppendLine($"  {rule.Key.PadRight(15)} - {rule.Value.Description}");
                }
                return new CommandResult(true, sb.ToString(), TimeSpan.Zero);
            }
        }
    }
}
