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
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();

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

        public override void Accept(ILEVisitor visitor) => visitor.Visit(this);
        public override bool Evaluate(bool[] _) => _value;
        public override void CollectVariables(HashSet<string> _) { }
        
        public override string ToString() => _value.ToString().ToLower();
        public override bool Equals(object obj)
        {
            return obj is ConstantNode other && _value == other._value;
        }
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public override Expression ToExpression(ParameterExpression _)
            => Expression.Constant(_value);
        protected override Expression BuildExpression(ParameterExpression _)
            => Expression.Constant(_value);
    }



    internal sealed class VariableNode : LENode
    {
        private int _index;
        public string Name { get; }
        public int Index
        {
            get => _index;
        }

        public VariableNode(string name, int index = -1)
        {
            Name = name;
            _index = index;
        }

        public VariableNode WithIndex(int newIndex)
        {
            return new VariableNode(Name, newIndex);
        }



        public override void Accept(ILEVisitor visitor) => visitor.Visit(this);
        public override bool Evaluate(bool[] inputs) => inputs[Index];
        public override void CollectVariables(HashSet<string> variables) => variables.Add(Name);

        public override string ToString() => Name;
        public override bool Equals(object obj)
        {
            if (obj is VariableNode other)
                return Name == other.Name && Index == other.Index;
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Index);
        }

        public override Expression ToExpression(ParameterExpression param)
            => GetCachedExpression(param);
        protected override Expression BuildExpression(ParameterExpression param)
            => Expression.ArrayAccess(param, Expression.Constant(Index));
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

        public override void Accept(ILEVisitor visitor) => visitor.Visit(this);
        public override bool Evaluate(bool[] inputs)
        {
            switch (Operator)
            {
                case "~":
                    return !Operand.Evaluate(inputs);
                default:
                    throw new NotSupportedException($"Унарный оператор '{Operator}' не поддерживается!");
            }
            ;
        }

        public override string ToString() => $"{Operator}({Operand.ToString()})";
        public override bool Equals(object obj)
        {
            if (obj is UnaryNode other)
                return Operator == other.Operator && Operand.Equals(other.Operand);
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Operator, Operand);
        }

        public override void CollectVariables(HashSet<string> variables) => Operand.CollectVariables(variables);
        public override Expression ToExpression(ParameterExpression param) => GetCachedExpression(param);
        protected override Expression BuildExpression(ParameterExpression param)
        {
            switch (Operator)
            {
                case "~":
                    return Expression.Not(Operand.ToExpression(param));
                default:
                    throw new NotSupportedException($"Унарный оператор '{Operator}' не поддерживается!");
            }
        }
        public override void ResetCache()
        {
            base.ResetCache();
            Operand.ResetCache();
        }
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

        public override void Accept(ILEVisitor visitor) => visitor.Visit(this);
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

        public override string ToString() => $"({Left.ToString()} {Operator} {Right.ToString()})";
        public override bool Equals(object obj)
        {
            if (obj is BinaryNode other)
                return Operator == other.Operator &&
                       Left.Equals(other.Left) &&
                       Right.Equals(other.Right);
            return false;
        }
        public override int GetHashCode()
        {
            return HashCode.Combine(Operator, Left, Right);
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
        public override void ResetCache()
        {
            base.ResetCache();
            Left.ResetCache();
            Right.ResetCache();
        }
    }
}