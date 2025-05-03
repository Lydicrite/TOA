using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalExpressionParser.Utils.Visitors
{
    /// <summary>
    /// Посетитель, строящий абстрактное синтаксическое дерево, используя классы <see cref="Expression"/> и <seealso cref="ParameterExpression"/>.
    /// </summary>
    internal sealed class ExpressionBuilderVisitor : BaseVisitor
    {
        /// <summary>
        /// Стэк, содержащий выражения и используемый для их обработки.
        /// </summary>
        private readonly Stack<Expression> _expressionStack = new();
        /// <summary>
        /// Объект, содержащий входные параметры выражения.
        /// </summary>
        private readonly ParameterExpression _param;

        /// <summary>
        /// Создаёт новый посетитель, строящий абстрактное синтаксическое дерево.
        /// </summary>
        /// <param name="param">Параметры для дерева выражеиня.</param>
        public ExpressionBuilderVisitor(ParameterExpression param) => _param = param;

        protected override void VisitConstant(ConstantNode node)
        {
            _expressionStack.Push(Expression.Constant(node.Evaluate(null)));
        }

        protected override void VisitVariable(VariableNode node)
        {
            _expressionStack.Push(Expression.ArrayIndex(_param, Expression.Constant(node.Index)));
        }

        protected override void VisitUnary(UnaryNode node)
        {
            node.Operand.Accept(this);
            var expr = _expressionStack.Pop();

            switch (node.Operator)
            {
                case "~": 
                    _expressionStack.Push(Expression.Not(expr));
                    break;
                default:
                    throw new NotSupportedException($"Оператор '{node.Operator}' не поддерживается");
            }
        }

        protected override void VisitBinary(BinaryNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            var right = _expressionStack.Pop();
            var left = _expressionStack.Pop();

            _expressionStack.Push(ProcessBinaryOperator(node.Operator, left, right));
        }

        private static Expression ProcessBinaryOperator(string op, Expression left, Expression right)
        {
            return op switch
            {
                "&" => Expression.AndAlso(left, right),
                "|" => Expression.OrElse(left, right),
                "^" => Expression.ExclusiveOr(left, right),
                "=>" => Expression.OrElse(Expression.Not(left), right),
                "<=>" => Expression.Equal(left, right),
                "!&" => Expression.Not(Expression.AndAlso(left, right)),
                "!|" => Expression.Not(Expression.OrElse(left, right)),
                _ => throw new NotSupportedException($"Оператор '{op}' не поддерживается")
            };
        }

        public Expression GetResult() => _expressionStack.Pop();
    }
}
