using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOAConsole.LogicalExpressionParser.Utils
{
    internal abstract class BaseVisitor : ILEVisitor
    {
        public void Visit(LENode node)
        {
            switch (node)
            {
                case ConstantNode cn:
                    VisitConstant(cn);
                    break;
                case VariableNode vn:
                    VisitVariable(vn);
                    break;
                case UnaryNode un:
                    VisitUnary(un);
                    break;
                case BinaryNode bn:
                    VisitBinary(bn);
                    break;
                default:
                    throw new NotSupportedException($"Тип узла {node.GetType()} не поддерживается");
            }
        }

        protected abstract void VisitConstant(ConstantNode node);
        protected abstract void VisitVariable(VariableNode node);
        protected abstract void VisitUnary(UnaryNode node);
        protected abstract void VisitBinary(BinaryNode node);
    }
}
