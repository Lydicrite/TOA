using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalExpressionParser.Utils.Visitors
{
    internal class ExpressionBuilderVisitor : BaseVisitor
    {
        private readonly Stack<Expression> _expressionStack = new();
        private readonly ParameterExpression _param;

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
            _expressionStack.Push(Expression.Not(expr));
        }

        protected override void VisitBinary(BinaryNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            var right = _expressionStack.Pop();
            var left = _expressionStack.Pop();

            // Логика для операторов вынесена в отдельный метод
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
