using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalExpressionParser
{
    internal interface ILEVisitor
    {
        void Visit(LENode node);
    }

    internal abstract class LENode
    {
        public abstract void Accept(ILEVisitor visitor);
        public abstract bool Evaluate(bool[] inputs);
        public abstract void CollectVariables(HashSet<string> variables);
        public abstract override string ToString();

        #region Кэширование

        private Expression _cachedExpression;
        protected ParameterExpression _cachedParam;

        public virtual void ResetCache()
        {
            _cachedExpression = null;
            _cachedParam = null;
        }

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



    internal sealed class ConstantNode : LENode
    {
        private readonly bool _value;

        public ConstantNode(bool value) 
        {
            _value = value; 
        }

        public override bool Evaluate(bool[] _) => _value;

        public override void CollectVariables(HashSet<string> _) { }

        public override Expression ToExpression(ParameterExpression _)
            => Expression.Constant(_value);

        protected override Expression BuildExpression(ParameterExpression _)
            => Expression.Constant(_value);

        public override void Accept(ILEVisitor visitor) => visitor.Visit(this);

        public override string ToString() => _value.ToString().ToLower();
    }



    internal sealed class VariableNode : LENode
    {
        private int _index;
        public string Name { get; }

        public int Index
        {
            get => _index;
            set
            {
                if (_index != value)
                {
                    _index = value;
                    ResetCache();
                }
            }
        }

        public VariableNode(string name) => Name = name;

        public override bool Evaluate(bool[] inputs) => inputs[Index];

        public override void CollectVariables(HashSet<string> variables) => variables.Add(Name);

        public override Expression ToExpression(ParameterExpression param)
            => GetCachedExpression(param);

        protected override Expression BuildExpression(ParameterExpression param)
            => Expression.ArrayAccess(param, Expression.Constant(Index));

        public override void Accept(ILEVisitor visitor) => visitor.Visit(this);

        public override string ToString() => Name;
    }



    internal sealed class UnaryNode : LENode
    {
        public LENode Operand { get; private set; }
        public string Operator { get; private set; }

        public UnaryNode(string op, LENode operand)
        {
            Operator = op ?? throw new ArgumentNullException(nameof(op));
            Operand = operand ?? throw new ArgumentNullException(nameof(operand));
        }

        public override void ResetCache()
        {
            base.ResetCache();
            Operand.ResetCache();
        }

        public override bool Evaluate(bool[] inputs)
        {
            switch (Operator)
            {
                case "!":
                    return !Operand.Evaluate(inputs);
                default:
                    throw new NotSupportedException($"Унарный оператор '{Operator}' не поддерживается!");
            }
            ;
        }

        public override void CollectVariables(HashSet<string> variables) => Operand.CollectVariables(variables);

        public override Expression ToExpression(ParameterExpression param) => GetCachedExpression(param);

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

        public override void Accept(ILEVisitor visitor) => visitor.Visit(this);

        public override string ToString() => $"{Operator}({Operand.ToString()})";
    }



    internal sealed class BinaryNode : LENode
    {
        public LENode Left { get; private set; }
        public LENode Right { get; private set; }
        public string Operator { get; private set; }

        public BinaryNode(string op, LENode left, LENode right)
        {
            Operator = op ?? throw new ArgumentNullException(nameof(op));
            Left = left ?? throw new ArgumentNullException(nameof(left));
            Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public override void ResetCache()
        {
            base.ResetCache();
            Left.ResetCache();
            Right.ResetCache();
        }

        public override bool Evaluate(bool[] inputs)
        {
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
            }
            ;
        }

        public override void CollectVariables(HashSet<string> variables)
        {
            Left.CollectVariables(variables);
            Right.CollectVariables(variables);
        }
        public override Expression ToExpression(ParameterExpression param) => GetCachedExpression(param);

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

        public override void Accept(ILEVisitor visitor) => visitor.Visit(this);

        public override string ToString() => $"({Left.ToString()} {Operator} {Right.ToString()})";
    }
}