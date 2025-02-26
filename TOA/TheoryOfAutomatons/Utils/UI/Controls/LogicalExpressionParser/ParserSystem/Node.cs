using Syncfusion.Windows.Forms.Tools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.LogicalExpressionParser.ParserSystem
{
    internal abstract class Node
    {
        public abstract bool Evaluate(bool[] inputs);
        public abstract void CollectVariables(HashSet<string> variables);
        public abstract string ToStringTree();

        #region Кэширование

        private Expression _cachedExpression;
        protected ParameterExpression _cachedParam;
        public Expression GetCachedExpression(ParameterExpression param)
        {
            if (_cachedExpression == null || !ReferenceEquals(_cachedParam, param))
            {
                _cachedExpression = ToExpression(param);
                _cachedParam = param;
            }
            return _cachedExpression;
        }
        protected abstract Expression BuildExpression(ParameterExpression param);
        public abstract Expression ToExpression(ParameterExpression param);

        #endregion
    }

    internal class ConstantNode : Node
    {
        private readonly bool _value;

        public ConstantNode(bool value) => _value = value;

        public override bool Evaluate(bool[] _) => _value;

        public override void CollectVariables(HashSet<string> _) { }

        public override Expression ToExpression(ParameterExpression _)
            => Expression.Constant(_value);

        protected override Expression BuildExpression(ParameterExpression _)
            => Expression.Constant(_value);

        public override string ToStringTree() => _value.ToString().ToLower();
    }

    internal class VariableNode : Node
    {
        public int Index { get; private set; }
        public string Name { get; private set; }

        public VariableNode(string name) => Name = name;

        public void SetIndex(int index) => Index = index;

        public override bool Evaluate(bool[] inputs) => inputs[Index];

        public override void CollectVariables(HashSet<string> variables) => variables.Add(Name);

        public override Expression ToExpression(ParameterExpression param)
            => GetCachedExpression(param);

        protected override Expression BuildExpression(ParameterExpression param)
            => Expression.ArrayAccess(param, Expression.Constant(Index));

        public override string ToStringTree() => Name;
    }

    internal class UnaryNode : Node
    {
        public Node Operand { get; private set; }
        public string Operator { get; private set; }

        public UnaryNode(string op, Node operand)
        {
            Operator = op;
            Operand = operand;
        }

        public override bool Evaluate(bool[] inputs) { 
            switch (Operator)
            {
                case "!": 
                    return !Operand.Evaluate(inputs);
                default: 
                    throw new NotSupportedException($"Унарный оператор '{Operator}' не поддерживается!");
            };
        }

        public override void CollectVariables(HashSet<string> variables) => Operand.CollectVariables(variables);

        public override Expression ToExpression(ParameterExpression param)
            => GetCachedExpression(param);

        protected override Expression BuildExpression(ParameterExpression param)
        {
            switch (Operator)
            {
                case "!":
                    return Expression.Not(Operand.ToExpression(param));
                default:
                    throw new NotSupportedException($"Унарный оператор '{Operator}' не поддерживается!");
            }
        }

        public override string ToStringTree() => $"{Operator}({Operand.ToStringTree()})";
    }

    internal class BinaryNode : Node
    {
        public Node Left { get; private set; }
        public Node Right { get; private set; }
        public string Operator { get; private set; }

        public BinaryNode(string op, Node left, Node right)
        {
            Operator = op;
            Left = left;
            Right = right;
        }

        public override bool Evaluate(bool[] inputs) {
            switch (Operator)
            {
                case "&": 
                    return Left.Evaluate(inputs) && Right.Evaluate(inputs);
                case "|": 
                    return Left.Evaluate(inputs) || Right.Evaluate(inputs);
                case "^": 
                    return Left.Evaluate(inputs) ^ Right.Evaluate(inputs);
                case "=>": 
                    return !Left.Evaluate(inputs) || Right.Evaluate(inputs);
                case "<=>": 
                    return Left.Evaluate(inputs) == Right.Evaluate(inputs);
                case "!&":
                    return !(Left.Evaluate(inputs) && Right.Evaluate(inputs));
                case "!|":
                    return !(Left.Evaluate(inputs) || Right.Evaluate(inputs));
                default: 
                    throw new NotSupportedException($"Бинарный оператор '{Operator}' не поддерживается!");
            };
        }

        public override void CollectVariables(HashSet<string> variables)
        {
            Left.CollectVariables(variables);
            Right.CollectVariables(variables);
        }
        public override Expression ToExpression(ParameterExpression param)
                => GetCachedExpression(param);

        protected override Expression BuildExpression(ParameterExpression param)
        {
            switch (Operator)
            {
                case "&":
                    return Expression.AndAlso(Left.ToExpression(param), Right.ToExpression(param));
                case "|":
                    return Expression.OrElse(Left.ToExpression(param), Right.ToExpression(param));
                case "^":
                    return Expression.ExclusiveOr(Left.ToExpression(param), Right.ToExpression(param));
                case "=>":
                    return Expression.OrElse(Expression.Not(Left.ToExpression(param)), Right.ToExpression(param));
                case "<=>":
                    return Expression.Equal(Left.ToExpression(param), Right.ToExpression(param));
                case "!&":
                    return Expression.Not(Expression.AndAlso(Left.ToExpression(param), Right.ToExpression(param)));
                case "!|":
                    return Expression.Not(Expression.OrElse(Left.ToExpression(param), Right.ToExpression(param)));
                default:
                    throw new NotSupportedException($"Бинарный оператор '{Operator}' не поддерживается!");
            }
        }

        public override string ToStringTree() =>
            $"({Left.ToStringTree()} {Operator} {Right.ToStringTree()})";
    }
}
