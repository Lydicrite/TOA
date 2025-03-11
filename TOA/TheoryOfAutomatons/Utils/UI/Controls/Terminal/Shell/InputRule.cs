using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.Terminal.Shell
{
    public enum InputRuleType
    {
        Commands = 0,
        BooleanLogic = 1,
        LogicalScheme = 2,
        SingleDigit = 3
    }

    public class InputRule
    {
        public InputRuleType Type { get; }
        public string Name { get; }
        public string Description { get; }
        public Regex ValidationRegex { get; }
        public string Example { get; }

        public InputRule(InputRuleType type, string name, string pattern, string description, string example)
        {
            Type = type;
            Name = name;
            Description = description;
            ValidationRegex = new Regex(pattern, RegexOptions.Compiled);
            Example = example;
        }

        public bool ValidateStrong(string input) => ValidationRegex.IsMatch(input);

        public bool ValidateFuture(string input) => ValidationRegex.IsMatch(input) || ValidationRegex.IsMatch(input + 'h');
    }

    public static class InputRulePresets
    {
        public static readonly Dictionary<string, InputRule> Rules = new()
        {
            {
                "commands",
                new InputRule
                (
                    InputRuleType.Commands,
                    "Командный режим",
                    @"^\\[a-zA-Z0-9_]+(?:\s+[a-zA-Z0-9_]+)*\s*$",
                    "Ввод команд вида: \\команда [аргументы]",
                    "\\help или \\clear или \\set_input_rule boolean_logic"
                )
            },

            {
                "boolean_logic",
                new InputRule
                (
                    InputRuleType.BooleanLogic,
                    "Логические выражения",
                    @"^(?!\s*$)(?:\s*(\b(true|false)\b|[A-Za-z]+|[01]|<=>|=>|&|\||\^|!|\(|\))\s*)+$",
                    "Допустимы: true, false, переменные (a-z, A-Z), 0, 1, операторы (&, |, ^, =>, <=>, !), скобки и пробелы.",
                    "(true & A) | (B ^ false)"
                )
            },

            {
                "single_digit",
                new InputRule
                (
                    InputRuleType.SingleDigit,
                    "Одиночные цифры",
                    @"^[0-9]$",
                    "Ввод одной цифры от 0 до 9",
                    "3 или 7"
                )
            }
        };
    }
}
