using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LogicalExpressionParser.Utils.Visitors;

namespace TOAConsole.LogicalExpressionParser
{
    /// <summary>
    /// Абстрактный класс элемента логического выражения.
    /// </summary>
    internal abstract class LENode
    {
        /// <summary>
        /// Принимает посетителя <see cref="ILEVisitor"/>.
        /// </summary>
        /// <param name="visitor">Принимаемый посетитель.</param>
        public abstract void Accept(ILEVisitor visitor);
        /// <summary>
        /// Вычисляет значение узла по переданным входным параметрам <paramref name="inputs"/>.
        /// </summary>
        /// <param name="inputs">Входные параметры (значения переменных).</param>
        /// <returns>Значение логического выражения по входам <paramref name="inputs"/>.</returns>
        public abstract bool Evaluate(bool[] inputs);
        /// <summary>
        /// Собирает имена переменных в узле в <paramref name="variables"/>.
        /// </summary>
        /// <param name="variables">Хэш-сет, содержащий имена переменных.</param>
        public abstract void CollectVariables(HashSet<string> variables);
        
        public abstract override string ToString();
        public abstract override bool Equals(object obj);
        public abstract override int GetHashCode();

        #region Кэширование

        /// <summary>
        /// Кэшированное <see cref="Expression"/> выражение (узел дерева).
        /// </summary>
        private Expression _cachedExpression;
        /// <summary>
        /// Кэшированное <see cref="Expression"/> выражение параметра.
        /// </summary>
        protected ParameterExpression _cachedParam;

        /// <summary>
        /// Обнуляет кэш узла.
        /// </summary>
        public virtual void ResetCache()
        {
            _cachedExpression = null;
            _cachedParam = null;
        }
        /// <summary>
        /// Возвращает кэшированное выражение узла, а также обновляет кэш, если он пуст.
        /// </summary>
        /// <param name="param">Входные параметры для узла.</param>
        /// <returns>Кэшированное выражение.</returns>
        public Expression GetCachedExpression(ParameterExpression param)
        {
            if (_cachedExpression == null || !ReferenceEquals(_cachedParam, param))
            {
                _cachedExpression = ToExpression(param);
                _cachedParam = param;
            }
            return _cachedExpression;
        }
        /// <summary>
        /// Строит и возвращает выражение узла с учётом переданных параметров.
        /// </summary>
        /// <param name="param">Входные параметры для узла.</param>
        /// <returns>Новое выражение.</returns>
        protected abstract Expression BuildExpression(ParameterExpression param);
        /// <summary>
        /// Возвращает выражение узла с учётом переданных параметров.
        /// </summary>
        /// <param name="param">Входные параметры для узла.</param>
        /// <returns>Новое выражение.</returns>
        public abstract Expression ToExpression(ParameterExpression param);

        #endregion
    }



    /// <summary>
    /// Представляет узел логической константы (<see langword="true"/> или <see langword="false"/>).
    /// </summary>
    internal sealed class ConstantNode : LENode
    {
        /// <summary>
        /// Значение константы.
        /// </summary>
        private readonly bool _value;

        public ConstantNode(bool value) 
        {
            _value = value; 
        }



        public override void Accept(ILEVisitor visitor) => visitor.Visit(this);
        public override bool Evaluate(bool[] _) => _value;
        public override void CollectVariables(HashSet<string> _) { }
        
        public override string ToString() => (_value ? 1 : 0).ToString().ToLower();
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



    /// <summary>
    /// Представляет узел переменной в логическом выражении.
    /// </summary>
    internal sealed class VariableNode : LENode
    {
        /// <summary>
        /// Индекс переменной.
        /// </summary>
        public int Index { get; }
        /// <summary>
        /// Имя переменной.
        /// </summary>
        public string Name { get; }
        
        public VariableNode(string name, int index = -1)
        {
            Name = name;
            Index = index;
        }

        /// <summary>
        /// Возвращает новый узел переменной с новым индексом и прежним именем.
        /// </summary>
        /// <param name="newIndex">Индекс нового узла.</param>
        /// <returns>Новый узел переменной с новым индексом и прежним именем.</returns>
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



    /// <summary>
    /// Представляет узел унарного оператора в логическом выражении.
    /// </summary>
    internal sealed class UnaryNode : LENode
    {
        /// <summary>
        /// Операнд этого оператора.
        /// </summary>
        public LENode Operand { get; private set; }
        /// <summary>
        /// Строковое представление операнда.
        /// </summary>
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

        public override string ToString() => $"{Operator}{Operand.ToString()}";
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
        /// <summary>
        /// Левый операнд этого оператора.
        /// </summary>
        public LENode Left { get; private set; }
        /// <summary>
        /// Правый операнд этого оператора.
        /// </summary>
        public LENode Right { get; private set; }
        /// <summary>
        /// Строковое представление операнда.
        /// </summary>
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