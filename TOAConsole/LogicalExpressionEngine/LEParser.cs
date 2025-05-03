using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TOAConsole.LogicalExpressionEngine.Utils;

namespace TOAConsole.LogicalExpressionEngine
{
    /// <summary>
    /// Представляет класс парсера логических выражений.
    /// </summary>
    internal static class LEParser
    {
        /// <summary>
        /// Словарь, определяющий очерёдность обработки логических операторов.
        /// </summary>
        private static readonly Dictionary<string, int> _operatorPrecedence = new(StringComparer.OrdinalIgnoreCase)
        {
            { "~", 5 }, { "&", 4 }, { "!&", 4 }, { "^", 3 }, { "|", 2 }, { "!|", 2 }, { "=>", 1 }, { "<=>", 0 }
        };

        /// <summary>
        /// Словарь, определяющий функции преобразования для унарных операторов.
        /// </summary>
        private static readonly Dictionary<string, Func<LENode, LENode>> _unaryOperators = new(StringComparer.OrdinalIgnoreCase)
        {
            { "~", operand => new UnaryNode("~", operand) }
        };

        /// <summary>
        /// Словарь, определяющий функции преобразования для бинарных операторов.
        /// </summary>
        private static readonly Dictionary<string, Func<LENode, LENode, LENode>> _binaryOperators = new(StringComparer.OrdinalIgnoreCase)
        {
                { "&", (l, r) =>   new BinaryNode("&", l, r) },
                { "|", (l, r) =>   new BinaryNode("|", l, r) },
                { "^", (l, r) =>   new BinaryNode("^", l, r) },
                { "=>", (l, r) =>  new BinaryNode("=>", l, r) },
                { "<=>", (l, r) => new BinaryNode("<=>", l, r) },
                { "!&", (l, r) =>  new BinaryNode("!&", l, r) },
                { "!|", (l, r) =>  new BinaryNode("!|", l, r) }
        };



        #region Словари союзных токенов

        /// <summary>
        /// Словарь, определяющий "союзные" строковые представления для стандартных представлений всех операторов.
        /// </summary>
        private static readonly Dictionary<string, string> _operatorAliases = new(StringComparer.OrdinalIgnoreCase)
        {
            // Бинарные операторы
            { "&", "&" }, { "AND", "&" }, { "˄", "&" }, { "∧", "&" },
            { "|", "|" }, { "OR", "|" }, { "˅", "|" }, { "∨", "|" },
            { "^", "^" }, { "XOR", "^" }, { "⊕", "^" },
            { "=>", "=>" }, { "IMPLIES", "=>" }, { "→", "=>" }, { "->", "=>" },
            { "<=>", "<=>" }, { "IFF", "<=>" }, { "≡", "<=>" }, { "⇔", "<=>" },
            { "!&", "!&" }, { "NAND", "!&" }, { "/", "!&" }, { "⊼", "!&" },
            { "!|", "!|" }, { "NOR", "!|" }, {"↓", "!|"}, {"⊽", "!|"},
            
            // Унарные операторы
            { "!", "~" }, { "NOT", "~" }, { "~", "~" }, { "¬", "~" }
        };

        /// <summary>
        /// Словарь, определяющий "союзные" строковые представления для стандартных представлений констант.
        /// </summary>
        private static readonly Dictionary<string, string> _constantAliases = new(StringComparer.OrdinalIgnoreCase)
        {
            { "0", "0" }, { "false", "0" },
            { "1", "1" }, { "true", "1" }
        };

        #endregion



        #region Кэш

        /// <summary>
        /// Максимальное количество элементов в кэше.
        /// </summary>
        private const int MaxCacheSize = 1024;
        /// <summary>
        /// Объект, хранящий и управляющий кэшем всех логических выражений, обработанных парсером.
        /// </summary>
        private static readonly MemoryCache _logicalExpressionsCache = new MemoryCache("LogicalExpressionsCache");
        /// <summary>
        /// Определяет политику работы с кэшем.
        /// </summary>
        private static readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy()
        {
            SlidingExpiration = TimeSpan.FromMinutes(5)
        };

        #endregion



        /// <summary>
        /// Преобразует строку в вычислимое логическое выражение.
        /// </summary>
        /// <param name="expression">Строка, содержащая текстовое представление логического выражения.</param>
        /// <returns>Корневой (начальный) узел логического выражения.</returns>
        public static LENode Parse(string expression)
        {
            if (_logicalExpressionsCache.Contains(expression))
                return (LENode)_logicalExpressionsCache.Get(expression);

            ReplaceConstantAliases(ref expression);
            ReplaceOperatorAliases(ref expression);
            var tokens = Tokenize(expression);
            ValidateTokenSequence(tokens);
            var postfix = ConvertToPostfix(tokens);
            var ast = BuildAST(postfix);

            // Очистка кэша
            if (_logicalExpressionsCache.GetCount() >= MaxCacheSize)
                _logicalExpressionsCache.Trim(10);

            _logicalExpressionsCache.Add(expression, ast, _cachePolicy);
            return ast;
        }

        /// <summary>
        /// Заменяет все символы констант на их стандартные варианты.
        /// </summary>
        /// <param name="expression">Ссылка на строку, в которой производится замена.</param>
        private static void ReplaceConstantAliases(ref string expression)
        {
            var pattern = @"\b(0|1)\b";
            expression = Regex.Replace(
                expression,
                pattern,
                m => _constantAliases[m.Value.ToLower()]
            );
        }

        /// <summary>
        /// Заменяет все символы операторов на их стандартные варианты.
        /// </summary>
        /// <param name="expression">Ссылка на строку, в которой производится замена.</param>
        private static void ReplaceOperatorAliases(ref string expression)
        {
            var sortedKeys = _operatorAliases.Keys
                .OrderByDescending(k => k.Length)
                .ThenBy(k => k, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var pattern = string.Join("|", sortedKeys.Select(k => Regex.Escape(k)));
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            expression = regex.Replace(expression, match => _operatorAliases[match.Value]);
        }

        /// <summary>
        /// Выполняет токенизацию входной строки.
        /// </summary>
        /// <param name="expression">Строка, представляющая логическое выражение.</param>
        /// <returns>Список токенов выражения.</returns>
        private static List<string> Tokenize(string expression)
        {
            var tokens = new List<string>();
            int i = 0;
            var varNameRegex = new Regex(@"^[a-zA-Z_][a-zA-Z0-9_]*$");

            while (i < expression.Length)
            {
                if (char.IsWhiteSpace(expression[i]))
                { 
                    i++;
                    continue;
                }
                else if (char.IsLetter(expression[i]))
                {
                    int start = i;
                    while (i < expression.Length && (char.IsLetterOrDigit(expression[i]) || expression[i] == '_'))
                        i++;

                    string varName = expression.Substring(start, i - start);

                    if (!varNameRegex.IsMatch(varName))
                        ThrowError($"Недопустимое имя переменной: '{varName}'", start);

                    tokens.Add(varName);
                }
                else if (expression[i] == '(' || expression[i] == ')')
                {
                    tokens.Add(expression[i].ToString());
                    i++;
                }
                else if (expression[i] == '0' || expression[i] == '1')
                {
                    tokens.Add(expression[i].ToString());
                    i++;
                }
                else
                {
                    string matchedOp = null;
                    foreach (var op in _operatorPrecedence.Keys)
                    {
                        if (i + op.Length <= expression.Length &&
                            expression.Substring(i, op.Length).Equals(op, StringComparison.OrdinalIgnoreCase))
                        {
                            matchedOp = op;
                            break;
                        }
                    }

                    if (matchedOp != null)
                    {
                        tokens.Add(matchedOp);
                        i += matchedOp.Length;
                    }
                    else
                    {
                        ThrowError($"Недопустимый символ '{expression[i]}'", i);
                    }
                }
            }
            return tokens;
        }

        /// <summary>
        /// Реализует алгоритм сортировочной станции для учёта приоритетов операторов.
        /// </summary>
        /// <param name="tokens">Список токенов выражения.</param>
        /// <returns>Список токенов в постфиксной записи.</returns>
        private static List<string> ConvertToPostfix(List<string> tokens)
        {
            var output = new List<string>();
            var stack = new Stack<string>();

            foreach (var token in tokens)
            {
                if (token == "(")
                {
                    stack.Push(token);
                }
                else if (token == ")")
                {
                    while (stack.Peek() != "(")
                        output.Add(stack.Pop());
                    stack.Pop();
                }
                else if (_operatorPrecedence.TryGetValue(token, out var precedence))
                {
                    while (stack.Count > 0 && stack.Peek() != "(" &&
                           _operatorPrecedence[stack.Peek()] >= precedence)
                        output.Add(stack.Pop());
                    stack.Push(token);
                }
                else
                {
                    output.Add(token);
                }
            }

            while (stack.Count > 0)
                output.Add(stack.Pop());

            return output;
        }

        /// <summary>
        /// Строит абстрактное синтаксическое дерево на основе списка токенов в постфиксной записи.
        /// </summary>
        /// <param name="postfix">Список токенов в постфиксной записи.</param>
        /// <returns>Корневой (начальный) узел синтаксического дерева выражения</returns>
        private static LENode BuildAST(List<string> postfix)
        {
            var stack = new Stack<LENode>();

            foreach (var token in postfix)
            {
                if (_constantAliases.TryGetValue(token, out var alias))
                {
                    stack.Push(new ConstantNode(alias == "1"));
                }
                else if (_unaryOperators.TryGetValue(token, out var unaryFactory))
                {
                    stack.Push(unaryFactory(stack.Pop()));
                }
                else if (_binaryOperators.TryGetValue(token, out var binaryFactory))
                {
                    var right = stack.Pop();
                    var left = stack.Pop();
                    stack.Push(binaryFactory(left, right));
                }
                else
                {
                    stack.Push(new VariableNode(token));
                }
            }

            return stack.Pop();
        }



        /// <summary>
        /// Проверяет на корректность последовательность токенов.
        /// </summary>
        /// <param name="tokens">Список токенов.</param>
        private static void ValidateTokenSequence(List<string> tokens)
        {
            int bracketBalance = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];

                // Проверка скобок
                if (token == "(")
                {
                    bracketBalance++;
                    if (i > 0 && !IsValidBeforeOpenBracket(tokens[i - 1]))
                        ThrowError($"Скобка '(' не может находиться после операнда \"{tokens[i - 1]}\"", i);
                }
                else if (token == ")")
                {
                    bracketBalance--;
                    if (i < tokens.Count - 1 && !IsValidAfterCloseBracket(tokens[i + 1]))
                        ThrowError($"Скобка ')' не может находиться перед операндом \"{tokens[i + 1]}\"", i);
                }

                // Проверка унарных операторов
                if (_unaryOperators.ContainsKey(token))
                {
                    if (i == tokens.Count - 1 || !IsValidAfterUnaryOperator(tokens[i + 1]))
                        ThrowError($"Унарный оператор \"{token}\" требует операнд", i);
                }

                // Проверка бинарных операторов
                if (_binaryOperators.ContainsKey(token))
                {
                    if (i == 0 || i == tokens.Count - 1)
                        ThrowError($"Бинарный оператор '{token}' не может быть в начале/конце", i);

                    if (!IsValidBinaryOperatorContext(tokens[i - 1], tokens[i + 1]))
                        ThrowError($"Некорректное использование оператора '{token}'", i);
                }

                if (bracketBalance < 0)
                    ThrowError("Неспаренная закрывающая скобка", i);
            }

            if (bracketBalance != 0)
                ThrowError("Неспаренные скобки", tokens.Count - 1);
        }

        /// <summary>
        /// Проверяет, может ли предыдущий токен стоять перед открывающей скобкой.
        /// </summary>
        /// <param name="prevToken">Предыдущий токен.</param>
        /// <returns><see langword="true"/>, если <paramref name="prevToken"/> может стоять перед открывающией скобкой, иначе <see langword="false"/>.</returns>
        private static bool IsValidBeforeOpenBracket(string prevToken) =>
            _binaryOperators.ContainsKey(prevToken) || prevToken == "(" || _unaryOperators.ContainsKey(prevToken);

        /// <summary>
        /// Проверяет, может ли следующий токен стоять после закрывающей скобки.
        /// </summary>
        /// <param name="nextToken">Следующий токен.</param>
        /// <returns><see langword="true"/>, если <paramref name="nextToken"/> может стоять после закрывающей скобки, иначе <see langword="false"/>.</returns>
        private static bool IsValidAfterCloseBracket(string nextToken) =>
            _binaryOperators.ContainsKey(nextToken) || nextToken == ")";

        /// <summary>
        /// Проверяет, может ли следующий токен стоять после унарного оператора.
        /// </summary>
        /// <param name="nextToken">Следующий токен.</param>
        /// <returns><see langword="true"/>, если <paramref name="nextToken"/> может стоять после унарного оператора, иначе <see langword="false"/>.</returns>
        private static bool IsValidAfterUnaryOperator(string nextToken) =>
            nextToken == "(" || IsVariableOrConstant(nextToken) || nextToken == "~";

        /// <summary>
        /// Проверяет, в верном ли контексте использован бинарный оператор.
        /// </summary>
        /// <param name="left">Токен, расположенный слева от оператора.</param>
        /// <param name="right">Токен, расположенный справа от оператора.</param>
        /// <returns><see langword="true"/>, если бинарный оператор использован в верном контексте, иначе <see langword="false"/>.</returns>
        private static bool IsValidBinaryOperatorContext(string left, string right) =>
            (IsVariableOrConstant(left) || left == ")" || _unaryOperators.ContainsKey(left)) && 
            (IsVariableOrConstant(right) || right == "(" || _unaryOperators.ContainsKey(right));

        /// <summary>
        /// Проверяет, представляет ли токен переменную или константу.
        /// </summary>
        /// <param name="token">Проверяемый токен.</param>
        /// <returns><see langword="true"/>, если токен представляет переменную или константу, иначе <see langword="false"/>.</returns>
        private static bool IsVariableOrConstant(string token) =>
            !_operatorPrecedence.ContainsKey(token) && token != "(" && token != ")";

        /// <summary>
        /// Выбрасывает исключение <see cref="LEParseException"/>.
        /// </summary>
        /// <param name="message">Сообщение ошибки.</param>
        /// <param name="position">Позиция в списке токенов, в которой найдена ошибка.</param>
        /// <exception cref="LEParseException"></exception>
        private static void ThrowError(string message, int position) 
            => throw new LEParseException(message, position);
    }
}