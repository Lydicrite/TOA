using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalExpressionParser.Utils.Visitors
{
    /// <summary>
    /// Посетитель для установки индексов переменных.
    /// </summary>
    internal sealed class VariableIndexerVisitor : BaseVisitor
    {
        /// <summary>
        /// Словарь с ключом в виде имени переменной и значением в виде её индекса.
        /// </summary>
        private readonly Dictionary<string, int> _indices;
        /// <summary>
        /// Стэк, содержащий узлы выражения и используемый для их обработки.
        /// </summary>
        private readonly Stack<LENode> _nodeStack = new();

        /// <summary>
        /// Создаёт новый установщик индексов переменных.
        /// </summary>
        /// <param name="indices">Словарь, используемый для установки и хранения индексов.</param>
        public VariableIndexerVisitor(Dictionary<string, int> indices) => _indices = indices;

        /// <summary>
        /// Обрабатывает узел <paramref name="node"/> и возвращает его модифицированную версию.
        /// </summary>
        /// <param name="node">Узел для обработки.</param>
        /// <returns>Изменённая версия узла.</returns>
        public LENode ProcessNode(LENode node)
        {
            _nodeStack.Clear();
            Visit(node);
            return _nodeStack.Pop();
        }

        protected override void VisitConstant(ConstantNode node)
            => _nodeStack.Push(node);

        protected override void VisitVariable(VariableNode node)
        {
            if (!_indices.TryGetValue(node.Name, out int index))
                throw new InvalidOperationException($"Переменная {node.Name} не найдена в списке");
            _nodeStack.Push(new VariableNode(node.Name, index));
        }

        protected override void VisitUnary(UnaryNode node)
        {
            Visit(node.Operand);
            var newOperand = _nodeStack.Pop();
            _nodeStack.Push(new UnaryNode(node.Operator, newOperand));
        }

        protected override void VisitBinary(BinaryNode node)
        {
            Visit(node.Left);
            Visit(node.Right);
            var newRight = _nodeStack.Pop();
            var newLeft = _nodeStack.Pop();
            _nodeStack.Push(new BinaryNode(node.Operator, newLeft, newRight));
        }
    }
}
