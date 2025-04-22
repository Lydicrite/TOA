using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalExpressionParser.Utils.Visitors
{
    internal class ExpanderVisitor : BaseVisitor
    {
        private readonly Stack<LENode> _stack = new Stack<LENode>();

        public LENode Expand(LENode node)
        {
            node.Accept(this);
            return _stack.Pop();
        }

        protected override void VisitConstant(ConstantNode node) => _stack.Push(node);

        protected override void VisitVariable(VariableNode node) => _stack.Push(node);

        protected override void VisitUnary(UnaryNode node)
        {
            node.Operand.Accept(this);
            var operand = _stack.Pop();

            // Применяем законы де Моргана для отрицаний над бинарными операциями
            if (operand is BinaryNode binary)
            {
                switch (binary.Operator)
                {
                    case "&":
                        _stack.Push(new BinaryNode("|",
                            new UnaryNode("~", binary.Left),
                            new UnaryNode("~", binary.Right)));
                        return;
                    case "|":
                        _stack.Push(new BinaryNode("&",
                            new UnaryNode("~", binary.Left),
                            new UnaryNode("~", binary.Right)));
                        return;
                }
            }

            _stack.Push(new UnaryNode("~", operand));
        }

        protected override void VisitBinary(BinaryNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);
            var right = _stack.Pop();
            var left = _stack.Pop();

            // Применяем дистрибутивность для раскрытия скобок
            if (node.Operator == "&" && right is BinaryNode rBin && rBin.Operator == "|")
            {
                var newLeft = new BinaryNode("&", left, rBin.Left);
                var newRight = new BinaryNode("&", left, rBin.Right);
                _stack.Push(new BinaryNode("|", newLeft, newRight));
                return;
            }

            if (node.Operator == "&" && left is BinaryNode lBin && lBin.Operator == "|")
            {
                var newLeft = new BinaryNode("&", lBin.Left, right);
                var newRight = new BinaryNode("&", lBin.Right, right);
                _stack.Push(new BinaryNode("|", newLeft, newRight));
                return;
            }

            _stack.Push(new BinaryNode(node.Operator, left, right));
        }
    }
}
