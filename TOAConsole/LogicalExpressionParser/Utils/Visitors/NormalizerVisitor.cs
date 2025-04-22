using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalExpressionParser.Utils.Visitors
{
    internal class NormalizerVisitor : BaseVisitor
    {
        private readonly Stack<LENode> _stack = new Stack<LENode>();

        public LENode Normalize(LENode node)
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

            // Устранение двойного отрицания
            if (node.Operator == "~" && operand is UnaryNode inner && inner.Operator == "~")
            {
                _stack.Push(inner.Operand);
                return;
            }

            // Де Морган для бинарных операций
            if (node.Operator == "~" && operand is BinaryNode binary)
            {
                var left = new UnaryNode("~", binary.Left);
                var right = new UnaryNode("~", binary.Right);

                switch (binary.Operator)
                {
                    case "&":
                        _stack.Push(new BinaryNode("|", left, right));
                        return;
                    case "|":
                        _stack.Push(new BinaryNode("&", left, right));
                        return;
                }
            }

            _stack.Push(new UnaryNode(node.Operator, operand));
        }

        protected override void VisitBinary(BinaryNode node)
        {
            node.Left.Accept(this);
            node.Right.Accept(this);

            var right = _stack.Pop();
            var left = _stack.Pop();

            // Упрощение константных выражений
            {
                if (left is ConstantNode cl && right is ConstantNode cr)
                {
                    bool lv = cl.Evaluate(null);
                    bool rv = cr.Evaluate(null);

                    switch (node.Operator)
                    {
                        case "&": _stack.Push(new ConstantNode(lv && rv)); return;
                        case "|": _stack.Push(new ConstantNode(lv || rv)); return;
                        case "^": _stack.Push(new ConstantNode(lv ^ rv)); return;
                        case "=>": _stack.Push(new ConstantNode(!lv || rv)); return;
                        case "<=>": _stack.Push(new ConstantNode(lv == rv)); return;
                        case "!&": _stack.Push(new ConstantNode(!(lv && rv))); return;
                        case "!|": _stack.Push(new ConstantNode(!(lv || rv))); return;
                    }
                }
            }

            // Оптимизация константы слева
            {
                if (left is ConstantNode cl)
                {
                    bool lv = cl.Evaluate(null);
                    switch (node.Operator)
                    {
                        case "&":
                            _stack.Push(lv ? right : cl);
                            return;

                        case "|":
                            _stack.Push(lv ? cl : right);
                            return;

                        case "^":
                            _stack.Push(lv ? new UnaryNode("~", right) : right);
                            return;

                        case "=>":
                            if (lv)
                                _stack.Push(right);
                            else
                                _stack.Push(new ConstantNode(true));
                            return;

                        case "!&":
                            if (lv)
                                _stack.Push(new UnaryNode("~", right)); // ~B
                            else
                                _stack.Push(new ConstantNode(true));    // ~0 = 1
                            return;

                        case "!|":
                            if (lv)
                                _stack.Push(new ConstantNode(false));  // ~1 = 0
                            else
                                _stack.Push(new UnaryNode("~", right)); // ~B
                            return;
                    }
                }
            }

            // Оптимизация константы справа
            {
                if (right is ConstantNode cr)
                {
                    bool rv = cr.Evaluate(null);
                    switch (node.Operator)
                    {
                        case "&":
                            _stack.Push(rv ? left : cr);
                            return;

                        case "|":
                            _stack.Push(rv ? cr : left);
                            return;

                        case "^":
                            _stack.Push(rv ? new UnaryNode("~", left) : left);
                            return;

                        case "=>":
                            _stack.Push(rv ? new ConstantNode(true) : new UnaryNode("~", left));
                            return;

                        case "!&": // NAND
                            if (rv)
                                _stack.Push(new UnaryNode("~", left)); // ~A
                            else
                                _stack.Push(new ConstantNode(true));   // ~0 = 1
                            return;

                        case "!|": // NOR
                            if (rv)
                                _stack.Push(new ConstantNode(false));  // ~1 = 0
                            else
                                _stack.Push(new UnaryNode("~", left)); // ~A
                            return;
                    }
                }
            }

            // XOR для разных ситуаций
            {
                if (node.Operator == "^")
                {
                    if (left.Equals(right))
                    {
                        _stack.Push(new ConstantNode(false));
                        return;
                    } 
                    else
                    {
                        _stack.Push
                        (
                            new BinaryNode
                                (   "|", 
                                    new BinaryNode("&", left, new UnaryNode("~", right)), 
                                    new BinaryNode("&", new UnaryNode("~", left), right)
                                )
                        );
                        return;
                    }
                }
            }

            // Поглощение: A | (A & B) → A
            {
                if (node.Operator == "|" && right is BinaryNode rb && rb.Operator == "&" && rb.Left.Equals(left))
                {
                    _stack.Push(left);
                    return;
                }
            }

            // Поглощение для =>: (A & B) => A    →    B => A
            {
                if (left is BinaryNode leftBin && leftBin.Operator == "&" && leftBin.Left.Equals(right))
                {
                    _stack.Push(new BinaryNode("=>", leftBin.Right, right)); // (A & B) => A → B => A
                    return;
                }
            }

            // Поглощение для AND: A & (A | B) → A
            {
                if (node.Operator == "&" && left is BinaryNode leftBin &&
                    leftBin.Operator == "|" && leftBin.Left.Equals(right))
                {
                    _stack.Push(leftBin.Left);
                    return;
                }
            }

            // Поглощение для OR: A | (!A & B) → A | B
            {
                if (node.Operator == "|" && right is BinaryNode rightBin &&
                    rightBin.Operator == "&" && rightBin.Left is UnaryNode unary &&
                    unary.Operand.Equals(left))
                {
                    _stack.Push(new BinaryNode("|", left, rightBin.Right));
                    return;
                }
            }

            // Дистрибутивность: A & (B | C) → (A & B) | (A & C)
            {
                if (node.Operator == "&" && right is BinaryNode rDist && rDist.Operator == "|")
                {
                    var newLeft = new BinaryNode("&", left, rDist.Left);
                    var newRight = new BinaryNode("&", left, rDist.Right);
                    _stack.Push(new BinaryNode("|", newLeft, newRight));
                    return;
                }
            }

            // Реинкарнация правды: A | !A → true  и  !A | A → true
            {
                if (node.Operator == "|")
                {
                    if (left is UnaryNode notLeft && notLeft.Operator == "~" && notLeft.Operand.Equals(right))
                    {
                        _stack.Push(new ConstantNode(true));
                        return;
                    }

                    if (right is UnaryNode notRight && notRight.Operator == "~" && notRight.Operand.Equals(left))
                    {
                        _stack.Push(new ConstantNode(true));
                        return;
                    }
                }
            }

            // Реинкарнация лжи: A & !A → false  и  !A & A → false
            {
                if (node.Operator == "&")
                {
                    if (left is UnaryNode notLeft && notLeft.Operator == "~" && notLeft.Operand.Equals(right))
                    {
                        _stack.Push(new ConstantNode(false));
                        return;
                    }

                    if (right is UnaryNode notRight && notRight.Operator == "~" && notRight.Operand.Equals(left))
                    {
                        _stack.Push(new ConstantNode(false));
                        return;
                    }

                }
            }

            // Идемпотентность: A & A → A   и   A | A → A
            {
                if (left.Equals(right) && (node.Operator == "&" || node.Operator == "|"))
                {
                    _stack.Push(left);
                    return;
                }
            }

            // Импликация: A => B → !A + B
            {
                if (node.Operator == "=>")
                {
                    _stack.Push(new BinaryNode("|", new UnaryNode("~", left), right));
                    return;
                }
            }

            // Эквивалентность: A <=> B → !A!B + AB
            {
                if (node.Operator == "<=>")
                {
                    _stack.Push
                        (
                            new BinaryNode
                                ("|",
                                    new BinaryNode("&", new UnaryNode("~", left), new UnaryNode("~", right)),
                                    new BinaryNode("&", left, right)
                                )
                        );
                    return;
                }
            }

            // Преобразование NAND / NOR
            switch (node.Operator)
            {
                case "!&":
                    _stack.Push(new BinaryNode("|", new UnaryNode("~", left), new UnaryNode("~", right)));
                    return;
                case "!|":
                    _stack.Push(new BinaryNode("&", new UnaryNode("~", left), new UnaryNode("~", right)));
                    return;
            }

            _stack.Push(new BinaryNode(node.Operator, left, right));
        }
    }
}
