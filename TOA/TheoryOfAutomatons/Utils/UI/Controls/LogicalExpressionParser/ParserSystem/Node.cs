using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOA.TheoryOfAutomatons.Utils.UI.Controls.LogicalExpressionParser.ParserSystem
{
    internal abstract class Node
    {
        public abstract bool Evaluate(bool[] inputs);
        public abstract void CollectVariables(HashSet<string> variables);
    }

    internal class VariableNode : Node
    {
        private int _index;
        public string Name { get; }

        public VariableNode(string name)
        {
            Name = name;
        }

        public void SetIndex(int index)
        {
            _index = index;
        }

        public override bool Evaluate(bool[] inputs)
        {
            return inputs[_index];
        }

        public override void CollectVariables(HashSet<string> variables)
        {
            variables.Add(Name);
        }
    }

    internal class UnaryNode : Node
    {
        private readonly Func<bool, bool> _operator;
        public Node Operand { get; }

        public UnaryNode(Func<bool, bool> op, Node operand)
        {
            _operator = op;
            Operand = operand;
        }

        public override bool Evaluate(bool[] inputs)
        {
            return _operator(Operand.Evaluate(inputs));
        }

        public override void CollectVariables(HashSet<string> variables)
        {
            Operand.CollectVariables(variables);
        }
    }

    internal class BinaryNode : Node
    {
        private readonly Func<bool, bool, bool> _operator;
        public Node Left { get; }
        public Node Right { get; }

        public BinaryNode(Func<bool, bool, bool> op, Node left, Node right)
        {
            _operator = op;
            Left = left;
            Right = right;
        }

        public override bool Evaluate(bool[] inputs)
        {
            return _operator(Left.Evaluate(inputs), Right.Evaluate(inputs));
        }

        public override void CollectVariables(HashSet<string> variables)
        {
            Left.CollectVariables(variables);
            Right.CollectVariables(variables);
        }
    }
}
