using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.LogicalExpressionParser.ParserSystem
{
    internal class LogicalExpressionParser
    {
        private readonly Dictionary<string, int> _operatorPrecedence;
        private readonly Dictionary<string, Func<LENode, LENode>> _unaryOperators;
        private readonly Dictionary<string, Func<LENode, LENode, LENode>> _binaryOperators;
        private static readonly ConcurrentDictionary<string, LENode> _logicalExpressionsCache 
                                                = new ConcurrentDictionary<string, LENode>();

        public LogicalExpressionParser()
        {
            _operatorPrecedence = new Dictionary<string, int>
            {
                { "!", 5 }, { "&", 4 }, { "^", 3 }, { "|", 2 }, { "=>", 1 }, { "<=>", 0 }
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
            return _logicalExpressionsCache.GetOrAdd(expression, expr =>
            {
                var tokens = Tokenize(expr);
                ValidateTokenSequence(tokens);
                var postfix = ConvertToPostfix(tokens);
                return BuildAst(postfix);
            });
        }

        // Добавление пользовательских операторов
        public void AddCustomOperator
        (
            string symbol, int precedence, bool isUnary,
            Func<LENode, LENode, LENode> binaryFactory = null,
            Func<LENode, LENode> unaryFactory = null
        )
        {
            _operatorPrecedence[symbol] = precedence;

            if (isUnary)
                _unaryOperators[symbol] = unaryFactory;
            else
                _binaryOperators[symbol] = binaryFactory;
        }

        private List<string> Tokenize(string expression)
        {
            var tokens = new List<string>();
            int i = 0;
            expression = expression.Replace(" ", "");

            while (i < expression.Length)
            {
                if (char.IsLetter(expression[i]))
                {
                    int start = i;
                    while (i < expression.Length && (char.IsLetterOrDigit(expression[i]) || expression[i] == '_'))
                        i++;

                    string expr = string.Empty;
                    for (int k = start; k < i; k++) {
                        expr += expression[k];
                    }
                    tokens.Add(expr);
                }
                else if (expression[i] == '(' || expression[i] == ')')
                {
                    tokens.Add(expression[i].ToString());
                    i++;
                }
                else if (expression[i] == '!' || expression[i] == '&' || expression[i] == '|' || expression[i] == '^')
                {
                    tokens.Add(expression[i].ToString());
                    i++;
                }
                else
                {
                    // Обработка многосимвольных операторов
                    string op = null;
                    foreach (var key in _operatorPrecedence.Keys.OrderByDescending(k => k.Length))
                    {
                        if (i + key.Length <= expression.Length &&
                            expression.Substring(i, key.Length) == key)
                        {
                            op = key;
                            i += key.Length;
                            break;
                        }
                    }

                    if (op == null)
                        throw new ExpressionParseException($"Некорректный токен в позиции {i}", i);

                    tokens.Add(op);
                }
            }
            return tokens;
        }

        private void ValidateTokenSequence(List<string> tokens)
        {
            int bracketBalance = 0;
            for (int i = 0; i < tokens.Count; i++)
            {
                var token = tokens[i];
                if (token == "(") bracketBalance++;
                else if (token == ")") bracketBalance--;

                if (bracketBalance < 0)
                    throw new ExpressionParseException("Обнаружена неспаренная закрывающая скобка", i);

                if (_binaryOperators.ContainsKey(token) &&
                    (i == 0 || i == tokens.Count - 1 || tokens[i - 1] == "(" || tokens[i + 1] == ")"))
                    throw new ExpressionParseException($"Бинарный оператор '{token}' в неверной позиции", i);
            }

            if (bracketBalance != 0)
                throw new ExpressionParseException("Обнаружены неспаренные скобки", tokens.Count - 1);
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

        private LENode BuildAst(List<string> postfix)
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



        [Serializable]
        internal class ExpressionParseException : Exception
        {
            public int Position { get; }
            public ExpressionParseException(string message, int pos) : base($"{message} (Позиция: {pos})")
                => Position = pos;
        }
    }
}
