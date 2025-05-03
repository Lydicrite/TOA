using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalExpressionParser.Utils.Visitors
{
    /// <summary>
    /// Посетитель, применяющий к узлам выражения законы алгебры логики для их упрощения и нормализации.
    /// </summary>
    internal class NormalizerVisitor : BaseVisitor
    {
        /// <summary>
        /// Стэк, содержащий узлы выражения и используемый для их обработки.
        /// </summary>
        private readonly Stack<LENode> _stack = new Stack<LENode>();

        /// <summary>
        /// Метод, применяющий законы алгебры логики для узла.
        /// </summary>
        /// <param name="node">Узел, для которого нужно применить метод.</param>
        /// <returns></returns>
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

            // Оптимизация: ~(const) → !const
            if (operand is ConstantNode cn)
            {
                _stack.Push(new ConstantNode(!cn.Evaluate(null)));
                return;
            }

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
                                ("|",
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

            #region Склеивание

            // Склеивание для OR: (A & B) | (A & ~B) → A и аналогичные варианты
            {
                if (node.Operator == "|"
                    && left is BinaryNode leftAnd && leftAnd.Operator == "&"
                    && right is BinaryNode rightAnd && rightAnd.Operator == "&")
                {
                    // Вариант 1: (A & B) | (A & ~B)
                    {
                        if (leftAnd.Left.Equals(rightAnd.Left)
                        && rightAnd.Right is UnaryNode rightNot
                        && rightNot.Operator == "~"
                        && rightNot.Operand.Equals(leftAnd.Right))
                        {
                            _stack.Push(leftAnd.Left);
                            return;
                        }
                    }

                    // Вариант 2: (A & ~B) | (A & B)
                    {
                        if (leftAnd.Left.Equals(rightAnd.Left)
                        && leftAnd.Right is UnaryNode leftNot
                        && leftNot.Operator == "~"
                        && leftNot.Operand.Equals(rightAnd.Right))
                        {
                            _stack.Push(leftAnd.Left);
                            return;
                        }
                    }

                    // Вариант 3: (B & A) | (B & ~A)
                    {
                        if (leftAnd.Right.Equals(rightAnd.Right)
                        && rightAnd.Left is UnaryNode rightNot
                        && rightNot.Operator == "~"
                        && rightNot.Operand.Equals(leftAnd.Left))
                        {
                            _stack.Push(leftAnd.Right);
                            return;
                        }
                    }

                    // Вариант 4: (B & ~A) | (B & A)
                    {
                        if (leftAnd.Right.Equals(rightAnd.Right)
                        && leftAnd.Left is UnaryNode leftNot
                        && leftNot.Operator == "~"
                        && leftNot.Operand.Equals(rightAnd.Left))
                        {
                            _stack.Push(leftAnd.Right);
                            return;
                        }
                    }

                    // Вариант 5: (~A & B) | (A & B) → B
                    {
                        if (leftAnd.Left is UnaryNode leftNot5
                        && leftNot5.Operator == "~"
                        && rightAnd.Left.Equals(leftNot5.Operand)
                        && leftAnd.Right.Equals(rightAnd.Right))
                        {
                            _stack.Push(leftAnd.Right);
                            return;
                        }
                    }

                    // Вариант 6: (A & B) | (~A & B) → B
                    {
                        if (rightAnd.Left is UnaryNode rightNot6
                        && rightNot6.Operator == "~"
                        && leftAnd.Left.Equals(rightNot6.Operand)
                        && leftAnd.Right.Equals(rightAnd.Right))
                        {
                            _stack.Push(leftAnd.Right);
                            return;
                        }
                    }

                    // Вариант 7: (~A & ~B) | (A & ~B) → ~B
                    {
                        if (leftAnd.Left is UnaryNode leftNot7
                        && leftNot7.Operator == "~"
                        && rightAnd.Left.Equals(leftNot7.Operand)
                        && leftAnd.Right is UnaryNode leftRightNot7
                        && rightAnd.Right.Equals(leftRightNot7))
                        {
                            _stack.Push(leftAnd.Right);
                            return;
                        }
                    }

                    // Вариант 8: (A & ~B) | (~A & ~B) → ~B
                    {
                        if (rightAnd.Left is UnaryNode rightNot8
                        && rightNot8.Operator == "~"
                        && leftAnd.Left.Equals(rightNot8.Operand)
                        && leftAnd.Right is UnaryNode leftRightNot8
                        && rightAnd.Right.Equals(leftRightNot8))
                        {
                            _stack.Push(leftAnd.Right);
                            return;
                        }
                    }
                }
            }

            // Склеивание для AND: (A | B) & (A | ~B) → A и аналогичные варианты
            {
                if (node.Operator == "&"
                    && left is BinaryNode leftOr && leftOr.Operator == "|"
                    && right is BinaryNode rightOr && rightOr.Operator == "|")
                {
                    // Вариант 1: (A | B) & (A | ~B)
                    {
                        if (leftOr.Left.Equals(rightOr.Left)
                        && rightOr.Right is UnaryNode rightNot
                        && rightNot.Operator == "~"
                        && rightNot.Operand.Equals(leftOr.Right))
                        {
                            _stack.Push(leftOr.Left);
                            return;
                        }
                    }

                    // Вариант 2: (A | ~B) & (A | B)
                    {
                        if (leftOr.Left.Equals(rightOr.Left)
                        && leftOr.Right is UnaryNode leftNot
                        && leftNot.Operator == "~"
                        && leftNot.Operand.Equals(rightOr.Right))
                        {
                            _stack.Push(leftOr.Left);
                            return;
                        }
                    }

                    // Вариант 3: (B | A) & (B | ~A)
                    {
                        if (leftOr.Right.Equals(rightOr.Right)
                        && rightOr.Left is UnaryNode rightNot
                        && rightNot.Operator == "~"
                        && rightNot.Operand.Equals(leftOr.Left))
                        {
                            _stack.Push(leftOr.Right);
                            return;
                        }
                    }

                    // Вариант 4: (B | ~A) & (B | A)
                    {
                        if (leftOr.Right.Equals(rightOr.Right)
                        && leftOr.Left is UnaryNode leftNot
                        && leftNot.Operator == "~"
                        && leftNot.Operand.Equals(rightOr.Left))
                        {
                            _stack.Push(leftOr.Right);
                            return;
                        }
                    }

                    // Вариант 5: (~A | B) & (A | B) → B
                    {
                        if (leftOr.Left is UnaryNode leftNot5
                        && leftNot5.Operator == "~"
                        && rightOr.Left.Equals(leftNot5.Operand)
                        && leftOr.Right.Equals(rightOr.Right))
                        {
                            _stack.Push(leftOr.Right);
                            return;
                        }
                    }

                    // Вариант 6: (A | B) & (~A | B) → B
                    {
                        if (rightOr.Left is UnaryNode rightNot6
                        && rightNot6.Operator == "~"
                        && leftOr.Left.Equals(rightNot6.Operand)
                        && leftOr.Right.Equals(rightOr.Right))
                        {
                            _stack.Push(leftOr.Right);
                            return;
                        }
                    }

                    // Вариант 7: (~A | ~B) & (A | ~B) → ~B
                    {
                        if (leftOr.Left is UnaryNode leftNot7
                        && leftNot7.Operator == "~"
                        && rightOr.Left.Equals(leftNot7.Operand)
                        && leftOr.Right is UnaryNode leftRightNot7
                        && rightOr.Right.Equals(leftRightNot7))
                        {
                            _stack.Push(leftOr.Right);
                            return;
                        }
                    }

                    // Вариант 8: (A | ~B) & (~A | ~B) → ~B
                    {
                        if (rightOr.Left is UnaryNode rightNot8
                        && rightNot8.Operator == "~"
                        && leftOr.Left.Equals(rightNot8.Operand)
                        && leftOr.Right is UnaryNode leftRightNot8
                        && rightOr.Right.Equals(leftRightNot8))
                        {
                            _stack.Push(leftOr.Right);
                            return;
                        }
                    }
                }
            }

            #endregion

            _stack.Push(new BinaryNode(node.Operator, left, right));
        }
    }
}
