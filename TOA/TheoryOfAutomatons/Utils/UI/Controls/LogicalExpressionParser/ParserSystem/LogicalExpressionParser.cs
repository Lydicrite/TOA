using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.LogicalExpressionParser.ParserSystem
{
    internal class LogicalExpressionParser
    {
        private Dictionary<string, int> _operatorPrecedence;
        private Dictionary<string, Func<Node, Node>> _unaryOperators;
        private Dictionary<string, Func<Node, Node, Node>> _binaryOperators;

        public LogicalExpressionParser()
        {
            _operatorPrecedence = new Dictionary<string, int>
        {
            { "!", 5 }, { "&", 4 }, { "^", 3 }, { "|", 2 }, { "=>", 1 }, { "<=>", 0 }
        };

            _unaryOperators = new Dictionary<string, Func<Node, Node>>
        {
            { "!", operand => new UnaryNode(x => !x, operand) }
        };

            _binaryOperators = new Dictionary<string, Func<Node, Node, Node>>
        {
            { "&", (left, right) => new BinaryNode((a, b) => a && b, left, right) },
            { "|", (left, right) => new BinaryNode((a, b) => a || b, left, right) },
            { "^", (left, right) => new BinaryNode((a, b) => a ^ b, left, right) },
            { "=>", (left, right) => new BinaryNode((a, b) => !a || b, left, right) },
            { "<=>", (left, right) => new BinaryNode((a, b) => a == b, left, right) }
        };
        }

        public Node Parse(string expression)
        {
            var tokens = Tokenize(expression);
            var postfix = ConvertToPostfix(tokens);
            return BuildAst(postfix);
        }

        private List<string> Tokenize(string expression)
        {
            expression = expression.Replace(" ", "");
            var tokens = new List<string>();
            int i = 0;

            while (i < expression.Length)
            {
                if (char.IsLetter(expression[i]))
                {
                    int start = i;
                    while (i < expression.Length && (char.IsLetterOrDigit(expression[i]) || expression[i] == '_'))
                        i++;
                    tokens.Add(expression.Substring(start, i - start));
                }
                else if (expression[i] == '(' || expression[i] == ')')
                {
                    tokens.Add(expression[i].ToString());
                    i++;
                }
                else
                {
                    string op = null;
                    if (i + 1 < expression.Length)
                    {
                        string twoChar = expression.Substring(i, 2);
                        if (_operatorPrecedence.ContainsKey(twoChar))
                        {
                            op = twoChar;
                            i += 2;
                        }
                    }
                    if (op == null)
                    {
                        op = expression[i].ToString();
                        i++;
                    }
                    tokens.Add(op);
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
                else if (_operatorPrecedence.ContainsKey(token))
                {
                    while (stack.Count > 0 && stack.Peek() != "(" &&
                           _operatorPrecedence[stack.Peek()] >= _operatorPrecedence[token])
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

        private Node BuildAst(List<string> postfix)
        {
            var stack = new Stack<Node>();

            foreach (var token in postfix)
            {
                if (_unaryOperators.TryGetValue(token, out var unaryOp))
                {
                    stack.Push(unaryOp(stack.Pop()));
                }
                else if (_binaryOperators.TryGetValue(token, out var binaryOp))
                {
                    var right = stack.Pop();
                    var left = stack.Pop();
                    stack.Push(binaryOp(left, right));
                }
                else
                {
                    stack.Push(new VariableNode(token));
                }
            }

            return stack.Pop();
        }
    }
}
