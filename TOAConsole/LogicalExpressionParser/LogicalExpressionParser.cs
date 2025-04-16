using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TOAConsole.LogicalExpressionParser.Utils;

namespace TOAConsole.LogicalExpressionParser
{
    internal class LogicalExpressionParser
    {
        private readonly Dictionary<string, int> _operatorPrecedence;
        private readonly Dictionary<string, Func<LENode, LENode>> _unaryOperators;
        private readonly Dictionary<string, Func<LENode, LENode, LENode>> _binaryOperators;
        private readonly Dictionary<string, string> _operatorAliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            // Бинарные операторы
            { "&", "&" }, { "AND", "&" }, { "∧", "&" },
            { "|", "|" }, { "OR", "|" }, { "∨", "|" },
            { "^", "^" }, { "XOR", "^" }, { "⊕", "^" },
            { "=>", "=>" }, { "IMPLIES", "=>" }, { "→", "=>" }, { "->", "=>" },
            { "<=>", "<=>" }, { "IFF", "<=>" }, { "≡", "<=>" }, { "⇔", "<=>" },
            { "!&", "!&" }, { "NAND", "!&" },
            { "!|", "!|" }, { "NOR", "!|" },
            
            // Унарные операторы
            { "!", "!" }, { "NOT", "!" }, { "~", "!" }, { "¬", "!" }
        };
        private static readonly HashSet<string> _reservedKeywords = new HashSet<string> { "true", "false" };
        private const int MaxCacheSize = 1024;
        private static readonly MemoryCache _logicalExpressionsCache = new MemoryCache("LogicalExpressionsCache");
        private static readonly CacheItemPolicy _cachePolicy = new CacheItemPolicy
        {
            RemovedCallback = args =>
            {
                Console.WriteLine($"[КЭШ] Удален элемент: {args.CacheItem.Key} | Причина: {args.RemovedReason}");
            },

            SlidingExpiration = TimeSpan.FromMinutes(5)
        };

        public LogicalExpressionParser()
        {
            _operatorPrecedence = new Dictionary<string, int>
            {
                { "!", 5 }, { "&", 4 }, { "!&", 4 }, { "^", 3 }, { "|", 2 }, { "!|", 2 }, { "=>", 1 }, { "<=>", 0 }
            };

            _unaryOperators = new Dictionary<string, Func<LENode, LENode>>
            {
                { "!", operand => new UnaryNode("!", operand) }
            };

            _binaryOperators = new Dictionary<string, Func<LENode, LENode, LENode>>
            {
                { "&", (l, r) =>   new BinaryNode("&", l, r) },
                { "|", (l, r) =>   new BinaryNode("|", l, r) },
                { "^", (l, r) =>   new BinaryNode("^", l, r) },
                { "=>", (l, r) =>  new BinaryNode("=>", l, r) },
                { "<=>", (l, r) => new BinaryNode("<=>", l, r) },
                { "!&", (l, r) =>  new BinaryNode("!&", l, r) },
                { "!|", (l, r) =>  new BinaryNode("!|", l, r) }
            };
        }

        public LENode Parse(string expression)
        {
            if (_logicalExpressionsCache.Contains(expression))
                return (LENode)_logicalExpressionsCache.Get(expression);

            ReplaceOperatorAliases(ref expression);
            var tokens = Tokenize(expression);
            ValidateTokenSequence(tokens);
            var postfix = ConvertToPostfix(tokens);
            var ast = BuildAST(postfix);

            if (_logicalExpressionsCache.GetCount() >= MaxCacheSize)
            {
                Console.WriteLine($"[КЭШ] Достигнут максимальный размер ({MaxCacheSize}), выполняем очистку 10%");
                _logicalExpressionsCache.Trim(10);
            }

            _logicalExpressionsCache.Add(expression, ast, _cachePolicy);
            return ast;
        }

        private void ReplaceOperatorAliases(ref string expression)
        {
            var sortedKeys = _operatorAliases.Keys
                .OrderByDescending(k => k.Length)
                .ThenBy(k => k, StringComparer.OrdinalIgnoreCase)
                .ToList();

            var pattern = string.Join("|", sortedKeys.Select(k => Regex.Escape(k)));
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);

            expression = regex.Replace(expression, match => _operatorAliases[match.Value]);
        }

        private List<string> Tokenize(string expression)
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

        private List<string> ConvertToPostfix(List<string> tokens)
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

        private LENode BuildAST(List<string> postfix)
        {
            var stack = new Stack<LENode>();

            foreach (var token in postfix)
            {
                if (token.ToLower() == "true")
                {
                    stack.Push(new ConstantNode(true));
                }
                else if (token.ToLower() == "false")
                {
                    stack.Push(new ConstantNode(false));
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





        private void ValidateTokenSequence(List<string> tokens)
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

        private bool IsValidBeforeOpenBracket(string prevToken) =>
            _binaryOperators.ContainsKey(prevToken) || prevToken == "(" || _unaryOperators.ContainsKey(prevToken);

        private bool IsValidAfterCloseBracket(string nextToken) =>
            _binaryOperators.ContainsKey(nextToken) || nextToken == ")";

        private bool IsValidAfterUnaryOperator(string nextToken) =>
            nextToken == "(" || IsVariableOrConstant(nextToken);

        private bool IsValidBinaryOperatorContext(string left, string right) =>
            (IsVariableOrConstant(left) || left == ")" || _unaryOperators.ContainsKey(left)) && 
            (IsVariableOrConstant(right) || right == "(" || _unaryOperators.ContainsKey(right));

        private bool IsVariableOrConstant(string token) =>
            !_operatorPrecedence.ContainsKey(token) && token != "(" && token != ")";

        private void ThrowError(string message, int position) 
            => throw new ExpressionParseException(message, position);
    }
}