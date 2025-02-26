using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.LogicalExpressionParser.ParserSystem
{
    internal class LogicalExpression
    {
        private LENode _root;
        private List<string> _variables;
        private Dictionary<string, int> _variableIndices;
        private Func<bool[], bool> _compiledDelegate;

        public LogicalExpression(LENode root)
        {
            _root = root;
            UpdateVariableInfo();
        }

        public void SetVariableOrder(IEnumerable<string> order)
        {
            _variables = order.ToList();
            _variableIndices = _variables
                .Select((name, idx) => (name, idx))
                .ToDictionary(x => x.name, x => x.idx);
            UpdateVariableInfo();
        }

        private void UpdateVariableInfo()
        {
            var variables = new HashSet<string>();
            CollectVariables();
            SetVariableIndices();
        }

        private void CollectVariables()
        {
            var variables = new HashSet<string>();
            _root.CollectVariables(variables);
            _variables = variables.OrderBy(v => v).ToList();
            _variableIndices = _variables
                .Select((name, idx) => (name, idx))
                .ToDictionary(x => x.name, x => x.idx);
        }

        private void SetVariableIndices()
        {
            var visitor = new VariableIndexSetter(_variableIndices);
            visitor.Visit(_root);
        }

        public override string ToString() => _root.ToStringTree();

        private void ValidateInputs(bool[] inputs)
        {
            if (inputs.Length != _variables.Count)
                throw new ArgumentException($"Ожидалось {_variables.Count} переменных: {string.Join(", ", _variables)}" +
                                                             $"\nПолучено {inputs.Length}: {string.Join(", ", inputs)}");
        }





        #region Интерпретация AST

        public bool Evaluate(bool[] inputs)
        {
            ValidateInputs(inputs);
            return _root.Evaluate(inputs);
        }

        #endregion



        #region Быстрая компиляция в Expression Tree

        public void CompileFast()
        {
            var param = Expression.Parameter(typeof(bool[]), "inputs");
            var expr = BuildExpression(_root, param);
            _compiledDelegate = Expression.Lambda<Func<bool[], bool>>(expr, param).Compile();
        }

        public bool EvaluateFast(bool[] inputs)
        {
            ValidateInputs(inputs);
            return _compiledDelegate != null
                ? _compiledDelegate(inputs)
                : _root.Evaluate(inputs);
        }

        private Expression BuildExpression(LENode node, ParameterExpression param)
        {
            switch (node)
            {

                case VariableNode vn: 
                    return Expression.ArrayIndex(param, Expression.Constant(vn.Index));
                case ConstantNode cn: 
                    return Expression.Constant(cn.Evaluate(null));
                case UnaryNode un: 
                    return Expression.Not(BuildExpression(un.Operand, param));
                case BinaryNode bn:
                    var left = BuildExpression(bn.Left, param);
                    var right = BuildExpression(bn.Right, param);

                    switch (bn.Operator)
                    {
                        case "&":   // AND
                            return Expression.AndAlso(left, right);

                        case "|":   // OR
                            return Expression.OrElse(left, right);

                        case "^":   // XOR
                            return Expression.ExclusiveOr(left, right);

                        case "=>":  // Импликация (эквивалентно !A | B)
                            return Expression.OrElse(Expression.Not(left), right);

                        case "<=>": // Эквивалентность
                            return Expression.Equal(left, right);

                        case "!&": // NAND (эквивалентно !(A & B))
                            return Expression.Not(Expression.AndAlso(left, right));

                        case "!|": // NOR (эквивалентно !(A | B))
                            return Expression.Not(Expression.OrElse(left, right));

                        default:
                            throw new NotSupportedException($"Оператор '{bn.Operator}' не поддерживается");
                    }
                    ;
                default:
                    throw new NotSupportedException();
            };
        }

        #endregion



        #region Генерация таблицы истинности

        public DataTable GenerateTruthTable()
        {
            if (_variables == null || _variables.Count == 0)
                throw new InvalidOperationException("Переменные выражения должны быть установлены до генерации таблицы.");

            var table = new DataTable();
            table.TableName = $"Таблица истинности f(...) = {this.ToString()}";

            // Добавляем колонки для переменных
            foreach (var varName in _variables)
            {
                table.Columns.Add(varName, typeof(bool));
            }

            // Добавляем колонку для результата
            table.Columns.Add($"f(...)");

            // Генерируем все комбинации значений
            int varCount = _variables.Count;
            int combinations = (int)Math.Pow(2, varCount);

            for (int i = 0; i < combinations; i++)
            {
                var row = table.NewRow();
                bool[] inputs = new bool[varCount];

                // Заполняем значения переменных
                for (int j = 0; j < varCount; j++)
                {
                    bool value = (i & (1 << (varCount - 1 - j))) != 0;
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

            sb.Append($"Таблица истинности\nf(...) = {this.ToString()}\n");

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
                    string value = (item is bool b) ? ((b ? 1 : 0).ToString()) : ((item is string bs) ? ((bs == "True" ? 1 : 0).ToString()) : "NULL");
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

            public void Visit(LENode node)
            {
                switch (node)
                {
                    case VariableNode vn:
                        vn.SetIndex(_indices[vn.Name]);
                        break;
                    case UnaryNode un:
                        Visit(un.Operand);
                        break;
                    case BinaryNode bn:
                        Visit(bn.Left);
                        Visit(bn.Right);
                        break;
                }
            }
        }

        #endregion
    }
}
