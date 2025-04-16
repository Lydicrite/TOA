using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TOAConsole.LogicalExpressionParser.Utils;

namespace TOAConsole.LogicalExpressionParser
{
    internal class LogicalExpression
    {
        private LENode _root;
        private ImmutableArray<string> _variables = ImmutableArray<string>.Empty;
        private Dictionary<string, int> _variableIndices;
        private Func<bool[], bool> _compiledDelegate;
        private bool _isDirty = true;

        public LogicalExpression(LENode root)
        {
            _root = root;
            UpdateVariableInfo();
        }

        public void SetVariableOrder(IEnumerable<string> order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            var uniqueVars = order.Distinct().ToImmutableArray();
            if (uniqueVars.Length != order.Count())
                throw new ArgumentException("Порядок переменных содержит дубликаты.");

            _variables = uniqueVars;
            _variableIndices = _variables
                .Select((name, idx) => (name, idx))
                .ToDictionary(x => x.name, x => x.idx);

            ResetCache();
            UpdateVariableInfo();
        }

        private void ResetCache()
        {
            _isDirty = true;
            _root.ResetCache();
        }

        private void UpdateVariableInfo()
        {
            var variables = new HashSet<string>();
            CollectVariables();
            SetVariableIndices();
            ResetCache();
        }

        private void CollectVariables()
        {
            var variables = new HashSet<string>();
            _root.CollectVariables(variables);
          
            if (!_variables.SequenceEqual(variables.OrderBy(v => v)))
            {
                ResetCache();
            }

            _variables = variables.OrderBy(v => v).ToImmutableArray();
            _variableIndices = _variables
                .Select((name, idx) => (name, idx))
                .ToDictionary(x => x.name, x => x.idx);
        }

        private void SetVariableIndices()
        {
            var visitor = new VariableIndexSetter(_variableIndices);
            visitor.Visit(_root);
        }

        public override string ToString() => _root.ToString();

        private void ValidateInputs(bool[] inputs)
        {
            if (inputs.Length != _variables.Count())
                throw new ArgumentException($"Ожидалось {_variables.Count()} переменных: {string.Join(", ", _variables)}" +
                                                             $"\nПолучено {inputs.Length}: {string.Join(", ", inputs)}");
        }





        #region Компиляция в Expression Tree

        public void Compile()
        {
            if (!_isDirty && _compiledDelegate != null) 
                return;

            var param = Expression.Parameter(typeof(bool[]), "inputs");
            var visitor = new ExpressionBuilderVisitor(param);
            _root.Accept(visitor);
            _compiledDelegate = Expression.Lambda<Func<bool[], bool>>(visitor.GetResult(), param).Compile();
            _isDirty = false;
        }

        public bool Evaluate(bool[] inputs)
        {
            Compile();
            ValidateInputs(inputs);
            return _compiledDelegate?.Invoke(inputs) ?? _root.Evaluate(inputs);
        }

        #endregion





        #region Генерация таблицы истинности

        public DataTable GenerateTruthTable()
        {
            if (_variables == null || _variables.Count() == 0)
                throw new InvalidOperationException("Переменные выражения должны быть установлены до генерации таблицы.");

            var table = new DataTable();
            table.TableName = $"Таблица истинности f(...) = {ToString()}";

            // Добавляем колонки для переменных
            foreach (var varName in _variables)
            {
                table.Columns.Add(varName, typeof(bool));
            }

            // Добавляем колонку для результата
            table.Columns.Add($"f(...)");

            // Генерируем все комбинации значений
            int varCount = _variables.Count();
            int combinations = 1 << varCount;

            for (int i = 0; i < combinations; i++)
            {
                var row = table.NewRow();
                bool[] inputs = new bool[varCount];

                // Заполняем значения переменных
                for (int j = 0; j < varCount; j++)
                {
                    bool value = (i & 1 << varCount - 1 - j) != 0;
                    row[j] = value;
                    inputs[j] = value;
                }

                // Вычисляем результат
                row[varCount] = Evaluate(inputs);
                table.Rows.Add(row);
            }

            return table;
        }

        public string PrintTruthTable()
        {
            int padding = 2;
            var table = GenerateTruthTable();
            StringBuilder sb = new StringBuilder();

            sb.Append($"\nf(...) = {ToString()}\n");

            // Заголовок
            foreach (DataColumn col in table.Columns)
            {
                sb.Append($"| {col.ColumnName.PadRight(padding)}");
            }
            sb.AppendLine();

            // Разделитель
            sb.AppendLine(new string('-', padding * table.Columns.Count * 2 + 4));

            // Данные
            foreach (DataRow row in table.Rows)
            {
                foreach (var item in row.ItemArray)
                {
                    string value = item is bool b ? (b ? 1 : 0).ToString() : item is string bs ? (bs == "True" ? 1 : 0).ToString() : "NULL";
                    sb.Append($"| {value.PadRight(padding)}");
                }
                sb.AppendLine();
            }

            return sb.ToString();
        }

        public DataView GetTruthTableView() => new DataView(GenerateTruthTable());

        #endregion





        #region Вспомогательный класс для установки индексов переменных

        private class VariableIndexSetter
        {
            private readonly Dictionary<string, int> _indices;

            public VariableIndexSetter(Dictionary<string, int> indices) => _indices = indices;

            public void Visit(LENode root)
            {
                var stack = new Stack<LENode>();
                stack.Push(root);

                while (stack.Count > 0)
                {
                    var node = stack.Pop();
                    switch (node)
                    {
                        case VariableNode vn:
                            if (!_indices.TryGetValue(vn.Name, out var newIndex))
                                throw new InvalidOperationException($"Переменная {vn.Name} не найдена в списке");
                            if (vn.Index != newIndex)
                                vn.Index = newIndex;
                            break;

                        case UnaryNode un:
                            stack.Push(un.Operand);
                            break;

                        case BinaryNode bn:
                            stack.Push(bn.Right);
                            stack.Push(bn.Left);
                            break;
                    }
                }
            }
        }

        #endregion
    }
}